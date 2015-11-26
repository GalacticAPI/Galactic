using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    public class SharePointListItem
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The ListItem object that underlies this list item.
        protected ListItem listItem = null;

        // ---------- PROPERTIES ----------

        public string DisplayName
        {
            get
            {
                return listItem.DisplayName;
            }
        }

        /// <summary>
        /// A SharePoint ListItem object representing this SharePointListItem.
        /// </summary>
        public ListItem ListItem
        {
            get
            {
                return listItem;
            }
        }

        /// <summary>
        /// A list of all fields defined in the list item.
        /// </summary>
        public Dictionary<string, object> Fields
        {
            get
            {
                return listItem.FieldValues;
            }
        }

        /// <summary>
        /// The list item's identifier.
        /// </summary>
        public int Id
        {
            get
            {
                return listItem.Id;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointListItem object from the underlying ListItem object.
        /// </summary>
        /// <param name="listItem">The base ListItem object representing this list item.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a list item is not supplied.</exception>
        public SharePointListItem(ListItem listItem)
        {
            if (listItem != null)
            {
                this.listItem = listItem;
            }
            else
            {
                throw new ArgumentNullException("listItem");
            }
        }

        // ---------- METHODS ----------

        /// <summary>
        /// Deletes the item from the server.
        /// </summary>
        /// <returns>True if the item was deleted from the server, false if the item did not exist on the server,
        /// or there was an error deleting it.</returns>
        public bool Delete()
        {
            try
            {
                // This throws an exception if it it's not in the list or there is an error retrieving it.
                listItem.DeleteObject();
                return true;
            }
            catch
            {
                // The item was not in the list, or there was an error retrieving it.
                return false;
            }
        }

        /// <summary>
        /// Gets the value contained in a lookup field.
        /// </summary>
        /// <param name="fieldName">The name of the lookup field to get the value of.</param>
        /// <returns>A KeyValuePair with the key equal to the ID of the lookup value, and the value equal to its value.
        /// An pair with a negative ID is returned if the value could not be retrieved, or the field name was invalid.</returns>
        public KeyValuePair<int, string> GetLookupFieldValue(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                try
                {
                    FieldLookupValue lookupValue = (FieldLookupValue)Fields[fieldName];
                    return new KeyValuePair<int, string>(lookupValue.LookupId, lookupValue.LookupValue);
                }
                catch
                {
                    // There was an error retrieving the value.
                    return new KeyValuePair<int,string>(-1, "");
                }
            }
            // The field name was not supplied.
            return new KeyValuePair<int, string>(-1, "");
        }

        /// <summary>
        /// Converts this SharePointListItem to a SharePointDocument object.
        /// </summary>
        /// <returns>The SharePointDocument object corresponding to this list item.</returns>
        public SharePointDocument ToSharePointDocument()
        {
            return new SharePointDocument(listItem);
        }

        /// <summary>
        /// Updates the a field within a SharePointListItem with the specified value.
        /// </summary>
        /// <param name="fieldName">The name of the field to update.</param>
        /// <param name="fieldValue">The value to assign to the field.</param>
        /// <returns>True if the item was updated, false otherwise.</returns>
        public bool UpdateField(string fieldName, object fieldValue)
        {
            if (!string.IsNullOrWhiteSpace(fieldName) || fieldValue != null)
            {
                try
                {
                    // Update the field's value on the server.
                    listItem[fieldName] = fieldValue;
                    listItem.Update();
                    listItem.Context.ExecuteQuery();
                }
                catch
                {
                    // There was an error updating the field values.
                    return false;
                }
            }
            // The values were changed, or no updates were necessary.
            return true;
        }
    }
}
