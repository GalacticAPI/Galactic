using System.Collections.Generic;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    public class SharePointDocumentLibrary : SharePointList, ICollection<SharePointDocument>
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The list of SharePoint documents in the library.
        private readonly List<SharePointDocument> documents = new List<SharePointDocument>();

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The documents in the list
        /// </summary>
        public List<SharePointDocument> Documents
        {
            get
            {   
                return documents;
            }
        }

        /// <summary>
        /// The SharePointFolder object for library's contents.
        /// </summary>
        public SharePointFolder Folder { get; protected set; }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointDocumentLibrary object from the underlying List object.
        /// </summary>
        /// <param name="list">The base List object representing this document library.</param>
        /// <param name="listItemCollection">A collection of ListItems that represents the items currently queried from the list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a list or folder is not supplied.</exception>
        public SharePointDocumentLibrary(List list, ListItemCollection listItemCollection) : base(list, listItemCollection)
        {
            Folder = new SharePointFolder(list.RootFolder);
            SharePointClient.Load(list.RootFolder.Context, list.RootFolder);

            // Create the list of documents.
            foreach (ListItem item in listItemCollection)
            {
                documents.Add(new SharePointDocument(item));
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Adds an item to the library.
        /// </summary>
        /// <param name="item">The item to add.</param>
        override public void Add(SharePointListItem item)
        {
            Add(item.ToSharePointDocument());
        }

        /// <summary>
        /// Adds a document to the library.
        /// </summary>
        /// <param name="document">The document to add.</param>
        public void Add(SharePointDocument document)
        {
            if (document != null)
            {
                ListItemCreationInformation creationInfo = new ListItemCreationInformation
                {
                    LeafName = document.Name,
                    FolderUrl = Folder.Url,
                    UnderlyingObjectType = FileSystemObjectType.File
                };
                ListItem listItem = List.AddItem(creationInfo);
                List.Update();

                // Add field values.
                foreach (KeyValuePair<string, object> field in document.Fields)
                {
                    listItem[field.Key] = field.Value;
                }
                listItem.Update();

                // Update the server with the changes.
                List.Context.ExecuteQuery();

                // Add the document to the list of documents.
                documents.Add(document);
            }
        }

        /// <summary>
        /// Determines whether the supplied document is in the library.
        /// </summary>
        /// <param name="document">The document to search for.</param>
        /// <returns>True if the document is in the list, false otherwise.</returns>
        public bool Contains(SharePointDocument document)
        {
            return Contains((SharePointListItem)document);
        }

        /// <summary>
        /// Determines whether a document with the supplied name is already in the library.
        /// </summary>
        /// <param name="name">The name of the file to search for in the library.</param>
        /// <returns></returns>
        public bool ContainsDocumentWithName(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                foreach (SharePointDocument document in Documents)
                {
                    if (document.Name == name)
                    {
                        // Found a match.
                        return true;
                    }
                }
            }
            // No document had the supplied name, or a name was not supplied.
            return false;
        }

        /// <summary>
        /// Copies the documents in the library to an array, starting at the supplied index.
        /// </summary>
        /// <param name="array">The array to copy the items into.</param>
        /// <param name="arrayIndex">The index within the array to place the copied documents at.</param>
        public void CopyTo(SharePointDocument[] array, int arrayIndex)
        {
            Documents.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a document from the library.
        /// </summary>
        /// <param name="document">The document to remove.</param>
        /// <returns>True if the document could be removed, false otherwise.</returns>
        public bool Remove(SharePointDocument document)
        {
            if (document != null)
            {
                documents.Remove(document);
                return Remove((SharePointListItem)document);
            }
            // The document wasn't provided.
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the library.
        /// </summary>
        /// <returns>An enumerator that iterates through the library.</returns>
        public new IEnumerator<SharePointDocument> GetEnumerator()
        {
            return Documents.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the library.
        /// </summary>
        /// <returns>An enumerator that iterates through the library.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Documents.GetEnumerator();
        }
    }
}
