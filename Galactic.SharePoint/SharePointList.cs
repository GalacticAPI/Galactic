using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;

namespace Galactic.SharePoint
{
    public class SharePointList : ICollection<SharePointListItem>
    {
        // ---------- CONSTANTS ----------

        // ---------- VARIABLES ----------

        // The collection of list items retrieved from the query that populated the list.
        protected ListItemCollection ListItemCollection = null;

        // The List object that underlies this list.
        protected List List = null;

        // ---------- PROPERTIES ----------

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return List.ItemCount;
            }
        }

        /// <summary>
        /// Indicates whether the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return !List.EffectiveBasePermissions.Has(PermissionKind.AddListItems);
            }
        }

        /// <summary>
        /// The items in the list
        /// </summary>
        public List<SharePointListItem> Items
        {
            get
            {
                List<SharePointListItem> listItems = new List<SharePointListItem>();
                foreach (ListItem item in ListItemCollection)
                {
                    listItems.Add(new SharePointListItem(item));
                }
                return listItems;
            }
        }

        /// <summary>
        /// The date and time that the last item was deleted from the list.
        /// </summary>
        public DateTime LastItemDeleted
        {
            get
            {
                return List.LastItemDeletedDate;
            }
        }

        /// <summary>
        /// The date and time that the last item was modified in the list.
        /// </summary>
        public DateTime LastItemModified
        {
            get
            {
                return List.LastItemModifiedDate;
            }
        }

        // ---------- CONSTRUCTORS ----------

        /// <summary>
        /// Creates a SharePointList object from the underlying List object.
        /// </summary>
        /// <param name="list">The base List object representing this list.</param>
        /// <param name="listItemCollection">A collection of ListItems that represents the items currently queried from the list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if a list or listCollection is not supplied.</exception>
        public SharePointList(List list, ListItemCollection listItemCollection)
        {
            if (list != null && listItemCollection != null)
            {
                List = list;
                ListItemCollection = listItemCollection;
            }
            else
            {
                if (list == null)
                {
                    throw new ArgumentNullException("list");
                }
                else
                {
                    throw new ArgumentException("listItemCollection");
                }
            }
        }

        // ---------- METHODS ----------


        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item">The item to add.</param>
        virtual public void Add(SharePointListItem item)
        {
            if (item != null)
            {
                ListItemCreationInformation creationInfo = new ListItemCreationInformation
                {
                    LeafName = item.DisplayName
                };
                ListItem listItem = List.AddItem(creationInfo);
                List.Update();

                // Add field values.
                foreach (KeyValuePair<string, object> field in item.Fields)
                {
                    listItem[field.Key] = field.Value;
                }
                listItem.Update();

                // Update the server with the changes.
                List.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Deletes all currently loaded items from the list.
        /// </summary>
        public void Clear()
        {
            foreach (ListItem listItem in ListItemCollection)
            {
                Remove(new SharePointListItem(listItem));
            }
        }

        /// <summary>
        /// Determines whether the supplied item is in the list.
        /// </summary>
        /// <param name="item">The item to search for.</param>
        /// <returns>True if the item is in the list, false otherwise.</returns>
        public bool Contains(SharePointListItem item)
        {
            return ListItemCollection.Contains(item.ListItem);
        }

        /// <summary>
        /// Copies the items in the list to an array, starting at the supplied index.
        /// </summary>
        /// <param name="array">The array to copy the items into.</param>
        /// <param name="arrayIndex">The index within the array to place the copied items at.</param>
        public void CopyTo(SharePointListItem[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes an item from the list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item could be removed, false otherwise.</returns>
        public bool Remove(SharePointListItem item)
        {
            if (item != null)
            {
                return item.Delete();
            }
            // The item couldn't be deleted from the server or wasn't provided.
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator that iterates through the list.</returns>
        public IEnumerator<SharePointListItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns>An enumerator that iterates through the list.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
