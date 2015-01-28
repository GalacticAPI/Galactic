using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    /// <summary>
    /// A class representing a folder within SharePoint.
    /// </summary>
    public class SharePointFolder
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The Folder object that underlies this folder.
        private readonly Folder folder = null;

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The server relative URL of the folder.
        /// </summary>
        public string Url
        {
            get
            {
                return folder.ServerRelativeUrl;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointFolder object from the underlying Folder object.
        /// </summary>
        /// <param name="folder">The base Folder object representing this document.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a folder is not supplied.</exception>
        public SharePointFolder(Folder folder)
        {
            if (folder != null)
            {
                this.folder = folder;
            }
            else
            {
                throw new ArgumentNullException("folder");
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Uploads a file to the folder.
        /// </summary>
        /// <param name="name">The name of the file to upload.</param>
        /// <param name="stream">The IO stream object representing the file's contents.</param>
        /// <param name="fileLength">The length of the file in bytes.</param>
        /// <param name="client">The SharePoint client that will be used to upload the file.</param>
        /// <param name="requiredFieldValues">Optional: A dictionary with values for fields that are required to be supplied
        /// when uploading the file to the folder in SharePoint.</param>
        /// <returns>True if the file could be uploaded, false otherwise.</returns>
        public bool UploadFile(string name, Stream stream, int fileLength, SharePointClient client, Dictionary<string, object> requiredFieldValues = null)
        {
            if (!string.IsNullOrWhiteSpace(name) && stream != null && fileLength > 0 && client != null)
            {
                try
                {
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect((ClientContext)folder.Context, Url + @"/" + name, stream, true);
                    Microsoft.SharePoint.Client.File file = client.GetFileByUrl(Url + @"/" + name);

                    // Add required field values if supplied.
                    if (requiredFieldValues != null)
                    {
                        ListItem listItem = file.ListItemAllFields;
                        foreach (string fieldName in requiredFieldValues.Keys)
                        {
                            listItem[fieldName] = requiredFieldValues[fieldName];
                        }
                        listItem.Update();
                    }

                    // Update the server with the changes.
                    folder.Context.ExecuteQuery();

                    // The file is automatically checked out. Check it back in.
                    file.CheckIn("", CheckinType.OverwriteCheckIn);

                    return true;
                }
                catch
                {
                    // There was an error uploading the file.
                    return false;
                }
            }
            else
            {
                // Parameters were not supplied.
                return false;
            }
        }
    }
}
