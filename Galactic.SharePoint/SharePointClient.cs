using System;
using System.IO;
using System.Net;
using Microsoft.SharePoint.Client;
using Galactic.Configuration;
using GalFile = Galactic.FileSystem.File;

namespace Galactic.SharePoint
{
    public class SharePointClient : IDisposable
    {
        // ---------- CONSTANTS ----------

        // A CAML query that will retrieve all items from a list.
        private const string CAML_GET_ALL_ITEMS = "<View><Query><OrderBy><FieldRef Name='ID' /></OrderBy></Query></View>";

        // The default size of NTFS file clusters in bytes.
        private const int NTFS_FILE_CLUSTER_SIZE_IN_BYTES = 16384;

        // ---------- VARIABLES ----------

        // The username of the account to use when connecting to SharePoint.
        private readonly string userName = "";
        // The password of the account to use when connecting to SharePoint.
        private readonly string password = "";
        // The Active Directory domain name of the account used to connect with SharePoint.
        private readonly string domain = "";
        // The absolute URL to the SharePoint site this client is connected to.
        private readonly string contextUrl = "";

        // The SharePoint client context to use when connecting with SharePoint.
        private readonly ClientContext clientContext = null;

        // ---------- PROPERTIES ----------

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Connects the client to SharePoint using the configuration in the specified configuration item.
        /// </summary>
        /// <param name="configurationItemDirectoryPath">The physical path to the directory where configuration item files can be found.</param>
        /// <param name="configurationItemName">The name of the configuration item containing the SharePoint client configuration.</param>
        public SharePointClient(string configurationItemDirectoryPath, string configurationItemName)
        {
            if (!string.IsNullOrWhiteSpace(configurationItemDirectoryPath) && !string.IsNullOrWhiteSpace(configurationItemName))
            {
                // Get the configuration item with the connection data from a file.
                ConfigurationItem configItem = new ConfigurationItem(configurationItemDirectoryPath, configurationItemName, true);

                // Read the credentials from the configuration item.
                if (!string.IsNullOrWhiteSpace(configItem.Value))
                {
                    StringReader reader = new StringReader(configItem.Value);
                    userName = reader.ReadLine();
                    password = reader.ReadLine();
                    domain = reader.ReadLine();
                    contextUrl = reader.ReadLine();
                }

                // Initialize the client context.
                clientContext = new ClientContext(contextUrl)
                {
                    // Add the credentials to the SharePoint context.
                    Credentials = new NetworkCredential(userName, password, domain)
                };
            }
            else
            {
                throw new ArgumentException("Unable to establish connection to SharePoint.");
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Closes the client connection to the server and frees up resources.
        /// </summary>
        public void Dispose()
        {
            clientContext.Dispose();
        }

        /// <summary>
        /// Downloads a file from the SharePoint server with the specified relative URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL of the file to retrieve from the base of the web application containing it.</param>
        /// <param name="downloadPath">The path to the directory that the file should be downloaded to.</param>
        /// <returns>True if the file was downloaded, false otherwise.</returns>
        public bool DownloadFileByUrl(string relativeUrl, string downloadPath)
        {
            if (!string.IsNullOrWhiteSpace(relativeUrl) && Directory.Exists(downloadPath))
            {
                try
                {
                    // Get information about the file from SharePoint.
                    FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, relativeUrl);

                    // Get a reader for the file's contents.
                    BinaryReader reader = new BinaryReader(fileInfo.Stream);

                    // Get the segments of the relative path supplied.
                    string[] pathParts = relativeUrl.Split('/');

                    // Create a new file and get a stream of it for writing.
                    FileStream fileStream = GalFile.Create(downloadPath + pathParts[pathParts.Length - 1]);

                    // Get a writer from the file stream created above.
                    BinaryWriter writer = new BinaryWriter(fileStream);

                    // Create a buffer for reading / writing data that is the default size of NTFS file clusters.
                    byte[] buffer = new byte[NTFS_FILE_CLUSTER_SIZE_IN_BYTES];

                    // Tracks the number of bytes read from the current read.
                    int numBytesRead = reader.Read(buffer, 0, buffer.Length);

                    // Keep reading while there are bytes left in the file to read.
                    while (numBytesRead > 0)
                    {
                        // Write to the new file.
                        writer.Write(buffer, 0, numBytesRead);

                        // Read more bytes.
                        numBytesRead = reader.Read(buffer, 0, buffer.Length);
                    }

                    // The file was successfully downloaded.
                    return true;
                }
                catch
                {
                    // There was an error downloading the file.
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Populates a SharePointDocumentLibrary object from the contents of SharePoint Document Library with the supplied title on the server.
        /// </summary>
        /// <param name="title">The title of the document library with the documents to retrieve.</param>
        /// <param name="camlQuery">Optional: The XML representing the CAML query you'd like to make of the documents in
        /// the document library. If not supplied, all documents will be retrieved.</param>
        /// <returns>A SharePointDocumentLibrary representing the library on the server, or null if the library could not be found, the
        /// user does not have sufficient permissions to access the library, or library's contents could otherwise not
        /// be retrieved.</returns>
        public SharePointDocumentLibrary GetDocumentLibraryByTitle(string title, string camlQuery = CAML_GET_ALL_ITEMS)
        {
            try
            {
                // Get the library from SharePoint.
                List list = clientContext.Web.Lists.GetByTitle(title);


                // Query the library.
                CamlQuery query = new CamlQuery
                {
                    ViewXml = camlQuery
                };
                ListItemCollection listItemCollection = list.GetItems(query);
                Load(clientContext, listItemCollection);

                // Create a SharePointDocumentLibrary object to populate.
                SharePointDocumentLibrary documentLibrary = new SharePointDocumentLibrary(list, listItemCollection);

                return documentLibrary;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Populates a SharePointList object from the contents of SharePoint List with the supplied title on the server.
        /// </summary>
        /// <param name="title">The title of the list with the list items to retrieve.</param>
        /// <param name="camlQuery">Optional: The XML representing the CAML query you'd like to make of the list items in
        /// the list. If not supplied, all items will be retrieved.</param>
        /// <returns>A SharePointList representing the list on the server, or null if the list could not be found, the
        /// user does not have sufficient permissions to access the list, or list's contents could otherwise not
        /// be retrieved.</returns>
        public SharePointList GetListByTitle(string title, string camlQuery = CAML_GET_ALL_ITEMS)
        {
            try
            {
                // Get the list from SharePoint.
                List list = clientContext.Web.Lists.GetByTitle(title);
                Load(clientContext, list);

                // Query the list.
                CamlQuery query = new CamlQuery
                {
                    ViewXml = camlQuery
                };
                ListItemCollection listItemCollection = list.GetItems(query);
                Load(clientContext, listItemCollection);

                // Create a SharePointList object to populate.
                SharePointList sharepointList = new SharePointList(list, listItemCollection);

                return sharepointList;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a File object representing a file on the SharePoint server with the specified relative URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL of the file to retrieve from the base of the web application containing it.</param>
        /// <returns>The File object representing the file at the specified URL or null if it does not
        /// exist or otherwise couldn't be retrieved.</returns>
        public Microsoft.SharePoint.Client.File GetFileByUrl(string relativeUrl)
        {
            if (!string.IsNullOrWhiteSpace(relativeUrl))
            {
                Microsoft.SharePoint.Client.File file = clientContext.Web.GetFileByServerRelativeUrl(relativeUrl);
                Load(clientContext, file);
                return file;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a Folder object representing a folder on the SharePoint server with the specified relative URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL of the folder to retrieve from the base of the web application containing it.</param>
        /// <returns>The Folder object representing the folder at the specified URL or null if it does not
        /// exist or otherwise couldn't be retrieved.</returns>
        public Folder GetFolderByUrl(string relativeUrl)
        {
            if (!string.IsNullOrWhiteSpace(relativeUrl))
            {
                Folder folder = clientContext.Web.GetFolderByServerRelativeUrl(relativeUrl);
                Load(clientContext, folder);
                return folder;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Loads values within a specified SharePoint object from the server.
        /// </summary>
        /// <param name="context">The Client Rutime Context object representing the client connection with the server.</param>
        /// <param name="obj">The object to populate the values of.</param>
        static public void Load(ClientRuntimeContext context, ClientObject obj)
        {
            if (context != null && obj != null)
            {
                context.Load(obj);
                context.ExecuteQuery();
            }
        }
    }
}
