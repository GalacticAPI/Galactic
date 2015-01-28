using System;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    /// <summary>
    /// A class representing a Document within SharePoint.
    /// </summary>
    public class SharePointDocument : SharePointListItem
    {
        // ---------- CONSTANTS ----------

        // Field names for various common document fields in SharePoint.
        private const string FILE_TYPE_FIELD_NAME = "File_x0020_Type";
        private const string FILE_SIZE_FIELD_NAME = "File_x0020_Size";

        // ---------- VARIABLES ----------

        // ---------- PROPERTIES ----------

        /// <summary>
        /// The date and time the document was created.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                return listItem.File.TimeCreated;
            }
        }

        /// <summary>
        /// The file extension of the document.
        /// </summary>
        public string FileExtension
        {
            get
            {
                return (string)listItem[FILE_TYPE_FIELD_NAME];
            }
        }

        /// <summary>
        /// The size of the file in bytes.
        /// </summary>
        public int FileSize
        {
            get
            {
                return Int32.Parse((string)listItem[FILE_SIZE_FIELD_NAME]);
            }
        }

        /// <summary>
        /// The date and time the document was last modified.
        /// </summary>
        public DateTime LastModifiedTime
        {
            get
            {
                return listItem.File.TimeLastModified;
            }
        }

        /// <summary>
        /// The user who last modified the document.
        /// </summary>
        public SharePointUser LastModifiedBy
        {
            get
            {
                return new SharePointUser(listItem.File.ModifiedBy);
            }
        }

        /// <summary>
        /// The name of the document.
        /// </summary>
        public string Name
        {
            get
            {
                return listItem.File.Name;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointDocument object from the underlying ListItem object.
        /// </summary>
        /// <param name="listItem">The base ListItem object representing this document.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a list item is not supplied.</exception>
        public SharePointDocument(ListItem listItem) : base(listItem)
        {
            // Load various the objects and parameters needed into the context.
            SharePointClient.Load(listItem.Context, listItem.File);
            SharePointClient.Load(listItem.Context, listItem.File.ModifiedBy);
        }

        // ---------- METHODS ----------
    }
}
