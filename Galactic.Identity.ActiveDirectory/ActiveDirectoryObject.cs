using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Runtime.Versioning;

namespace Galactic.Identity.ActiveDirectory
{
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class ActiveDirectoryObject : IComparable<ActiveDirectoryObject>, IEqualityComparer<ActiveDirectoryObject>
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The list of Attributes that should be retrieved when searching for the entry in AD.
        /// </summary>
        static protected string[] AttributeNames = { "objectGUID", "distinguishedName", "description", "memberOf", "objectClass", "objectCategory", "displayName", "cn" };

        // ----- VARIABLES -----

        // The client used to query and manipulate Active Directory.
        protected ActiveDirectoryClient AD = null;

        // The date for January 1st, 1601. Used as a start date for intervals in various object attribute properties.
        protected DateTime JAN_01_1601 = new DateTime(1601, 1, 1);

        // The SearchResultEntry that contains the information for the account.
        protected SearchResultEntry Entry = null;

        // The list of attributes to retrieve when searching for the entry in AD.
        protected List<string> Attributes = new List<string>(AttributeNames);

        // A list of additional attributes that should be retrieved in addition to the basic attributes defined above.
        protected List<string> AdditionalAttributes = new List<string>();


        // ----- PROPERTIES -----

        /// <summary>
        /// The Common Name (CN) of the object in Active Directory.
        /// </summary>
        public string CommonName
        {
            get
            {
                return GetStringAttributeValue("cn");
            }
        }

        /// <summary>
        /// The time the object was created in UTC.
        /// </summary>
        public DateTime? CreateTimeStamp
        {
            get
            {
                string timeStamp = GetStringAttributeValue("createTimeStamp");
                if (!string.IsNullOrWhiteSpace(timeStamp))
                {
                    return ActiveDirectoryClient.GetDateTimeFromUTCCodedTime(timeStamp);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The Distinguished Name (DN) of the object in Active Directory.
        /// </summary>
        public string DistinguishedName
        {
            get
            {
                return GetStringAttributeValue("distinguishedName");
            }
        }

        /// <summary>
        /// The GUID of the object in Active Directory.
        /// </summary>
        public Guid GUID
        {
            get
            {
                return AD.GetGUID(Entry);
            }
        }

        /// <summary>
        /// The distinguished name of the organizational unit or parent object containing the object.
        /// </summary>
        public string OrganizationalUnit
        {
            get
            {
                string ou = DistinguishedName;
                if (!string.IsNullOrWhiteSpace(ou))
                {
                    string[] ouComponents = ou.Split(',');
                    return ou.Substring(ouComponents[0].Length + 1);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The schema class types that identify the type of object this is in Active Directory.
        /// Examples: group, user, etc.
        /// </summary>
        public List<string> SchemaClasses
        {
            get
            {
                return AD.GetStringAttributeValues("objectClass", Entry);
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Gets a directory object from AD with the supplied GUID.
        /// </summary>
        /// <param name="ad">An Active Directory client used to query and manipulate the directory object.</param>
        /// <param name="guid">The GUID of the object.</param>
        public ActiveDirectoryObject(ActiveDirectoryClient ad, Guid guid)
        {
            if (ad != null && guid != Guid.Empty)
            {
                // Set this object's Active Directory object.
                AD = ad;
                Entry = GetEntryFromAD(guid);
                if (Entry == null)
                {
                    throw new ArgumentException("The GUID provided could not be found in active directory.", "guid");
                }
            }
            else
            {
                if (ad == null)
                {
                    throw new ArgumentNullException("ad");
                }
                if (guid == Guid.Empty)
                {
                    throw new ArgumentException("guid");
                }
            }
        }

        /// <summary>
        /// Gets a directory object from a supplied search result entry.
        /// </summary>
        /// <param name="ad">An Active Directory object used to manipulate the directory object.</param>
        /// <param name="entry">The SearchResultEntry object containing attributes necessary to populate the object.</param>
        public ActiveDirectoryObject(ActiveDirectoryClient ad, SearchResultEntry entry)
        {
            // Check that an Active Directory object and search result entry have been provided.
            if (ad == null)
            {
                throw new ArgumentNullException("ad");
            }
            if (entry == null)
            {
                throw new ArgumentNullException("entry");
            }

            // Set this object's Active Directory object.
            AD = ad;
            Entry = entry;
        }

        // ----- METHODS -----

        /// <summary>
        /// Compares this ActiveDirectoryObject to another ActiveDirectoryObject.
        /// </summary>
        /// <param name="other">The other ActiveDirectoryObject to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(ActiveDirectoryObject other)
        {
            if (other != null)
            {
                return string.Compare(GUID.ToString(), other.GUID.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("other");
            }
        }

        /// <summary>
        /// Deletes values from an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute with values to delete.</param>
        /// <param name="values">Optional: The specific values to delete from the object, or if null, will delete all attributes. Defaults to null.</param>
        /// <returns>True if the values were deleted, or false if there was an error or the values could not otherwise be deleted.</returns>
        public bool DeleteAttribute(string attributeName, object[] values = null)
        {
            if (!string.IsNullOrWhiteSpace(attributeName))
            {
                if (AD.DeleteAttribute(attributeName, Entry, values))
                {
                    // Refresh the object to reflect the change.
                    return Refresh();
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first ActiveDirectoryObject to check.</param>
        /// <param name="y">The second ActiveDirectoryObject to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(ActiveDirectoryObject x, ActiveDirectoryObject y)
        {
            if (x != null && y != null)
            {
                return x.GUID.Equals(y.GUID);
            }
            else
            {
                if (x == null)
                {
                    throw new ArgumentNullException("x");
                }
                else
                {
                    throw new ArgumentNullException("y");
                }
            }
        }

        /// <summary>
        /// Gets new data for the object from AD and places the values in the entry variable within the object.
        /// </summary>
        /// <param name="guid">The GUID of the object to get.</param>
        /// <returns>The SearchResultEntry corresponding the GUID supplied, or null if it could not be found in AD.</returns>
        protected SearchResultEntry GetEntryFromAD(Guid guid)
        {
            if (guid != Guid.Empty)
            {
                // Create a list of query attributes that contain the default list and any additional that have been queried for since object creation.
                List<string> queryAttributes = new List<string>();
                queryAttributes.AddRange(Attributes);
                queryAttributes.AddRange(AdditionalAttributes);

                return AD.GetEntryByGUID(guid, queryAttributes);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the first byte array value from the supplied attribute of the object.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retreive.</param>
        /// <returns>The value of the attribute, or null if it could not be found.</returns>
        public byte[] GetByteAttributeValue(string attributeName)
        {
            // Refresh the entry data if needed to get the attribute name specified.
            RefreshEntryIfNeededToRetrieveAttribute(attributeName);

            // Get the value from the attribute.
            return AD.GetByteAttributeValue(attributeName, Entry);
        }

        /// <summary>
        /// Gets all byte array values from the supplied attribute of the object.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retreive.</param>
        /// <returns>The values of the attribute, or null if it could not be found.</returns>
        public byte[][] GetByteAttributeValues(string attributeName)
        {
            // Refresh the entry data if needed to get the attribute name specified.
            RefreshEntryIfNeededToRetrieveAttribute(attributeName);

            // Get all values from the attribute.
            return AD.GetByteAttributeValues(attributeName, Entry);
        }

        /// <summary>
        /// Gets new data for the object from AD and places the values in the entry variable within the object.
        /// </summary>
        /// <param name="guid">The GUID of the object to get.</param>
        /// <param name="additionalAttributes">The names of additional attributes to include in the query.</param>
        /// <returns>The SearchResultEntry corresponding the GUID supplied, or null if it could not be found in AD.</returns>
        protected SearchResultEntry GetEntryFromAD(Guid guid, List<string> additionalAttributes)
        {
            // Check whether additional attributes were supplied.
            if (additionalAttributes != null && additionalAttributes.Count > 0)
            {
                // Add only new additional attributes to the query.
                foreach (string attribute in additionalAttributes)
                {
                    if (!AdditionalAttributes.Contains(attribute))
                    {
                        AdditionalAttributes.Add(attribute);
                    }
                }
            }

            // Get the entry from AD.
            return GetEntryFromAD(guid);
        }

        /// <summary>
        /// Generates a hash code for the ActiveDirectoryObject supplied.
        /// </summary>
        /// <param name="obj">The ActiveDirectoryObject to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(ActiveDirectoryObject obj)
        {
            if (obj != null)
            {
                return obj.GUID.GetHashCode();
            }
            else
            {
                throw new ArgumentNullException("obj");
            }
        }

        /// <summary>
        /// Gets the UTC DateTime from a Interval attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retreive.</param>
        /// <returns>The value of the attribute, or null if it could not be found.</returns>
        public DateTime? GetIntervalAttributeValue(string attributeName)
        {
            // Refresh the entry data if needed to get the attribute name specified.
            RefreshEntryIfNeededToRetrieveAttribute(attributeName);

            // Get the value from the attribute.
            return AD.GetIntervalAttributeValue(attributeName, Entry);
        }

        /// <summary>
        /// Gets the first string value from the supplied attribute of the object.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retreive.</param>
        /// <returns>The value of the attribute, or null if it could not be found.</returns>
        public string GetStringAttributeValue(string attributeName)
        {
            // Refresh the entry data if needed to get the attribute name specified.
            RefreshEntryIfNeededToRetrieveAttribute(attributeName);

            // Get the value from the attribute.
            return AD.GetStringAttributeValue(attributeName, Entry);
        }

        /// <summary>
        /// Gets all string values from the supplied attribute of the object.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retreive.</param>
        /// <returns>The values of the attribute, or null if it could not be found.</returns>
        public List<string> GetStringAttributeValues(string attributeName)
        {
            // Refresh the entry data if needed to get the attribute name specified.
            RefreshEntryIfNeededToRetrieveAttribute(attributeName);

            // Get all values from the attribute.
            return AD.GetStringAttributeValues(attributeName, Entry);
        }

        /// <summary>
        /// Moves and / or renames this object.
        /// </summary>
        /// <param name="newParentObjectGuid">(Optional: Required only if moving) The GUID of the new parent object for the object (if moving).</param>
        /// <param name="newCommonName">(Optional: Required only if renaming) The new common name (if renaming).</param>
        /// <returns>True if the object was moved or renamed, false otherwise.</returns>
        public virtual bool MoveRename(Guid? newParentObjectGuid = null, string newCommonName = null)
        {
            if (AD.MoveRenameObject(GUID, newParentObjectGuid, newCommonName))
            {
                // The object was moved in AD. Refresh the object's attributes.
                return Refresh();
            }
            return false;
        }

        /// <summary>
        /// Refreshes the object to retrieve any changes made to attributes since creation.
        /// </summary>
        /// <returns>True if the refresh was successful, false otherwise.</returns>
        public bool Refresh()
        {
            Entry = GetEntryFromAD(GUID);
            if (Entry != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Refreshed the object's entry variable if required to get a new attribute from the object in AD.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to retrieve if necessary.</param>
        private void RefreshEntryIfNeededToRetrieveAttribute(string attributeName)
        {
            // Check whether we need to do a new query to get the attribute name specified.
            if (!AdditionalAttributes.Contains(attributeName))
            {
                // Do a new query.
                SearchResultEntry queryResult = GetEntryFromAD(GUID, new List<string>() { attributeName });

                // Check that the object was found in AD.
                if (queryResult != null)
                {
                    // The object was found. Replace this object's entry with the new one retrieved.
                    Entry = queryResult;
                }
            }
        }

        /// <summary>
        /// Sets attribute of an object. If a null or empty values object is supplied no modifications will be made.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to set.</param>
        /// <param name="values">The value(s) to set the attribute to.</param>
        /// <returns>True if the attribute was set successfully, false otherwise.</returns>
        public bool SetAttribute(string attributeName, object[] values)
        {
            // Check that an attribute name and associated value(s) are supplied.
            if (!string.IsNullOrWhiteSpace(attributeName) && values != null && values.Length > 0)
            {
                // Check if the attribute already has a value.
                if (AD.AddOrReplaceAttributeValue(attributeName, values, Entry))
                {
                    // Refresh the object to reflect the change.
                    return Refresh();
                }
            }
            return false;
        }

        /// <summary>
        /// Sets multi value attribute of an object. If a null or empty values object is supplied no modifications will be made.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to set.</param>
        /// <param name="values">The value(s) to set the attribute to.</param>
        /// <returns>True if the attribute was set successfully, false otherwise.</returns>
        public bool SetMultiValueAttribute(string attributeName, object[] values)
        {
            // Check that an attribute name and associated value(s) are supplied.
            if (!string.IsNullOrWhiteSpace(attributeName) && values != null && values.Length > 0)
            {
                // Check if the attribute already has a value.
                if (AD.AddAttributeValue(attributeName, values, Entry))
                {
                    // Refresh the object to reflect the change.
                    return Refresh();
                }
            }
            return false;
        }

        /// <summary>
        /// Sets a single value string attribute of an object. If a null or empty value is supplied,
        /// the attribute will be cleared / deleted.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to set.</param>
        /// <param name="value">The string value to set the attribute to.</param>
        /// <returns>True if the attribute was set successfully, false otherwise.</returns>
        public bool SetStringAttribute(string attributeName, string value)
        {
            if (!string.IsNullOrWhiteSpace(attributeName))
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return SetAttribute(attributeName, new object[] { value });
                }
                else
                {
                    return DeleteAttribute(attributeName);
                }
            }
            else
            {
                // No attribute name was supplied.
                return false;
            }
        }
    }
}
