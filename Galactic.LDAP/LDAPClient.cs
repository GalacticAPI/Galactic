using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Security;
using System.DirectoryServices.Protocols;

namespace Galactic.Ldap
{
    /// <summary>
    /// LDAPClient is a class that allows for the query and manipulation
    /// of LDAP directory objects.
    /// </summary>
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("android")]
    public class LdapClient : IDisposable
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// The default unencrypted port used for LDAP servers.
        /// </summary>
        public const int LDAP_PORT = 389;

        /// <summary>
        /// The default SSL encrypted port for LDAP servers.
        /// </summary>
        public const int LDAP_SSL_PORT = 636;

        /// <summary>
        /// The default size of pages returned when making large queries.
        /// </summary>
        public const int DEFAULT_QUERY_PAGE_SIZE = 500;

        // ----- VARIABLES -----

        // The object that manages the connection with the LDAP server.
        private readonly LdapConnection connection = null;

        // The rootDSE entry for the connected LDAP server.
        private readonly SearchResultEntry rootDSE = null;

        // The base DNs that this server hosts.
        private readonly string[] namingContexts = null;

        // A list of other servers that can fulfill LDAP requests if the one we're connected to becomes unavailable.
        // May be null if there are no other servers available.
        private readonly string[] alternateServers = null;

        // The distinguished name of the directory object where all searches will have as their base. Defaults to the first naming context found in the RootDSE.
        private string searchBaseDN = null;

        // The scope of searches in the directory. Defaults to Subtree which includes the base object and all its child objects.
        private SearchScope searchScope = SearchScope.Subtree;

        // ----- PROPERTIES -----

        /// <summary>
        /// The Naming Contexts (The base DNs that this server hosts.) for the LDAP server as defined in the RootDSE entry.
        /// </summary>
        public string[] NamingContexts
        {
            get
            {
                return namingContexts;
            }
        }

        /// <summary>
        /// A list of other servers that can fulfill LDAP requests if the one we're connected to becomes unavailable.
        /// May be null if there are no other servers available.
        /// </summary>
        public string[] AlternateServers
        {
            get
            {
                return alternateServers;
            }
        }

        /// <summary>
        /// The rootDSE entry for the connected LDAP server.
        /// </summary>
        public SearchResultEntry RootDSE
        {
            get
            {
                return rootDSE;
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Establishes a connection with an LDAP server that can be used to query or modify its contents.
        /// <param name="servers">A list of servers by fully qualified domain name, host name, ip address, or null.</param>
        /// <param name="portNumber">The port number on the LDAP server that is listening for requests.</param>
        /// <param name="authType">(Optional) The type of authentication to use when connecting with the server. By default this is set to Anonymous (i.e. no credentials required).</param>
        /// <param name="userName">(Optional) The user name to use when connecting to the LDAP server.</param>
        /// <param name="password">(Optional) The password to use with the user name provided to connect to the LDAP server.</param>
        /// <param name="domainName">(Optional) The domain or computer name associated with the user credentials provided.</param>
        /// <param name="useLogonCredentials">(Optional) If enabled, the LDAP connection will use the logon credentials from the current session. Disabled by default.</param>
        /// </summary>
        public LdapClient(List<string> servers, int portNumber, AuthType authType = AuthType.Anonymous, string userName = null, SecureString password = null, string domainName = null, bool useLogonCredentials = false)
        {
            if ((servers != null && servers.Count > 0 && portNumber > 0 && !string.IsNullOrWhiteSpace(userName) && password != null) || (servers != null && servers.Count > 0 && portNumber > 0 && useLogonCredentials))
            {
                try
                {
                    // Setup the server information for the connection.
                    LdapDirectoryIdentifier directoryIdentifier = new LdapDirectoryIdentifier(servers.ToArray(), portNumber, false, false);

                    // Setup the credential to use when accessing the server. (Or null for Anonymous.)
                    NetworkCredential credential = null;
                    if (authType != AuthType.Anonymous)
                    {
                        credential = new NetworkCredential(userName, password);
                        if (!string.IsNullOrWhiteSpace(domainName))
                        {
                            // A domain was provided. Use it when creating the credential.
                            credential.Domain = domainName;
                        }
                    }

                    // Create the connection to the server(s).
                    try
                    {
                        if (useLogonCredentials)
                        {
                            connection = new LdapConnection(directoryIdentifier);
                        }
                        else
                        {
                            connection = new LdapConnection(directoryIdentifier, credential, authType);
                        }


                        // Gather information about the LDAP server(s) from the RootDSE entry.
                        SearchResponse rootDSESearchResponse = (SearchResponse)connection.SendRequest(new SearchRequest(null, "(objectClass=*)", SearchScope.Base));
                        if (rootDSESearchResponse != null && rootDSESearchResponse.ResultCode == ResultCode.Success)
                        {
                            // Save the rootDSE for access by API clients.
                            rootDSE = rootDSESearchResponse.Entries[0];
                            SearchResultAttributeCollection attributes = rootDSE.Attributes;

                            // Check that LDAP V3 is supported.
                            if (attributes["supportedLDAPVersion"].GetValues(typeof(string)).Contains("3"))
                            {
                                // Get all of the naming contexts this server(s) supports.
                                namingContexts = (string[])attributes["namingContexts"].GetValues(typeof(string));

                                // Set the base DN for searching to the first naming context in the list.
                                searchBaseDN = namingContexts[0];

                                // Get any alternate servers can complete our requests should this one stop responding.
                                // If there are not other servers to contact this attribute is not available.
                                if (attributes.Contains("altServer"))
                                {
                                    alternateServers = (string[])attributes["altServer"].GetValues(typeof(string));
                                }
                            }
                            else
                            {
                                throw new NotSupportedException("The directory server does not support LDAP v3.");
                            }
                        }

                        // Bind to the ldap server with the connection credentials if supplied.
                        if (connection.AuthType != AuthType.Anonymous)
                        {
                            connection.Bind();
                        }
                    }
                    catch (System.ComponentModel.InvalidEnumArgumentException)
                    {
                        // Thrown when authType is out of range.
                        throw new ArgumentOutOfRangeException("authType");
                    }
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Entries in the servers parameter can not have spaces.");
                }
            }
            else
            {
                if (servers == null || servers.Count == 0)
                {
                    throw new ArgumentNullException("servers", "The list of servers can not be null or empty.");
                }
                if (portNumber <= 0)
                {
                    throw new ArgumentOutOfRangeException("portNumber", "A port number must be positive.");
                }
            }
        }

        // ----- METHODS -----


        /// <summary>
        /// Closes the LDAP connection and frees all resources associated with it.
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }

        /// <summary>
        /// Searches the LDAP directory for entries that match the specified search filter.
        /// </summary>
        /// <param name="filter">The filter that defines the entries to find.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in each entry found. </param>
        /// <param name="baseDn">(Optional)The distinguished name of the base entry where the search will begin. (Typically an OU or the base DN of the directory.) If not supplied, the default values will be used. This base is used only for the duration of this search.</param>
        /// <param name="scope">(Optional) The scope to use while searching. Defaults to Subtree. (Typically Base, just the object with the DN specified; OneLevel, just the child objects of the base object; or Subtree, the base object and all child objects) This scope is used only for the duration of this search.</param>
        /// <param name="queryPageSize">(Optional) The query page size to specify when making large requests. Defaults to DEFAULT_QUERY_PAGE_SIZE.</param>
        /// <param name="chaseReferrals">(Optional) Whether the search should chase object referrals to other servers if necessary. Defaults to true;</param>
        /// <returns>A collection of search result entries found, or null if there was an error with the search.</returns>
        public List<SearchResultEntry> Search(string filter, List<string> attributes = null, string baseDn = null, SearchScope scope = SearchScope.Subtree, int queryPageSize = DEFAULT_QUERY_PAGE_SIZE, bool chaseReferrals = true)
        {
            // Set the search base and scope for the search if provided.
            string previousBase = searchBaseDN;
            SearchScope previousScope = searchScope;
            bool customBaseAndScope = false;
            if (!string.IsNullOrWhiteSpace(baseDn))
            {
                SetSearchBaseAndScope(baseDn, scope);
                customBaseAndScope = true;
            }

            SearchRequest request = null;

            // Check if attributes have been provided.
            if (attributes == null || attributes.Count == 0)
            {
                // No attributes were provided... get them all.
                request = new SearchRequest(searchBaseDN, filter, searchScope);
            }
            else
            {
                // Specific attributes were requested, limit the search to just them.
                request = new SearchRequest(searchBaseDN, filter, searchScope, attributes.ToArray());
            }

            // Add a directory control that makes the search use pages for returning large result sets.
            PageResultRequestControl pageResultRequestControl = new PageResultRequestControl(queryPageSize);
            request.Controls.Add(pageResultRequestControl);

            if (!chaseReferrals)
            {
                // Turn of referral chasing in the session.
                connection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
            }

            // Create a list to hold our results while we request all of the results in pages.
            List<SearchResultEntry> results = new List<SearchResultEntry>();
            try
            {
                while (true)
                {
                    // Add the page request control that manages the paged searched, and send the request for results.
                    SearchResponse response = (SearchResponse)connection.SendRequest(request);

                    // Check that we received a response.
                    if (response != null)
                    {
                        // A response was received.

                        // Get the paging response control to allow us to gather the results in batches.
                        foreach (DirectoryControl control in response.Controls)
                        {
                            if (control is PageResultResponseControl)
                            {
                                PageResultResponseControl pageResultResponseControl =
                                    (PageResultResponseControl)control;

                                // Update the cookie in the request control to gather the next page of the query.
                                pageResultRequestControl.Cookie = pageResultResponseControl.Cookie;

                                // Break out of the loop now that we've copied the cookie.
                                break;
                            }
                        }

                        if (response.ResultCode == ResultCode.Success)
                        {
                            // Add the results to the list.
                            foreach (SearchResultEntry entry in response.Entries)
                            {
                                results.Add(entry);
                            }
                        }
                        else
                        {
                            // There has been an error retrieving the results.

                            // Reset the search base and scope if necessary.
                            if (customBaseAndScope)
                            {
                                SetSearchBaseAndScope(previousBase, previousScope);
                            }

                            return null;
                        }

                        // Check whether the cookies is empty and all the results have been gathered.
                        if (pageResultRequestControl.Cookie.Length == 0)
                        {
                            // The cookie is empty. We're done gathing results.
                            break;
                        }
                    }
                    else
                    {
                        // No response was received.
                        return null;
                    }
                }
                // Return the results found.

                // Reset the search base and scope if necessary.
                if (customBaseAndScope)
                {
                    SetSearchBaseAndScope(previousBase, previousScope);
                }

                return results;
            }
            catch
            {
            }

            // Reset the search base and scope if necessary.
            if (customBaseAndScope)
            {
                SetSearchBaseAndScope(previousBase, previousScope);
            }

            return null;
        }

        /// <summary>
        /// Adds an entry to the LDAP directory with the specified distinguished name and attributes.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to add.</param>
        /// <param name="attributes">The attributes for the entry to add.</param>
        /// <returns>True if added, false otherwise.</returns>
        public bool Add(string dn, DirectoryAttribute[] attributes)
        {
            if (!string.IsNullOrWhiteSpace(dn))
            {
                AddRequest request = new AddRequest(dn, attributes);
                try
                {
                    AddResponse response = (AddResponse)connection.SendRequest(request);

                    // Check that a response was received.
                    if (response != null)
                    {
                        // A response was received.
                        if (response.ResultCode == ResultCode.Success)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // A response was not received.
                        return false;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Adds or Replaces an attribute's value in the specified entry in the directory.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to add or replace an attribute of.</param>
        /// <param name="attributeName">The name of the attribute to add or replace a value for.</param>
        /// <param name="values">The values associated with the attribute to add or replace.</param>
        /// <returns>True if added or replaced, false otherwise.</returns>
        public bool AddOrReplaceAttribute(string dn, string attributeName, object[] values)
        {
            if (!string.IsNullOrWhiteSpace(dn) && !string.IsNullOrWhiteSpace(attributeName))
            {
                ModifyRequest request = new ModifyRequest(dn, DirectoryAttributeOperation.Add, attributeName, values);

                try
                {
                    ModifyResponse response = (ModifyResponse)connection.SendRequest(request);

                    // Check that a response was received.
                    if (response != null)
                    {
                        // A response was received.
                        if (response.ResultCode == ResultCode.Success)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // A response was not received.
                        return false;
                    }
                }
                catch (DirectoryOperationException)
                {
                    // The attribute already has a value.
                    return ReplaceAttribute(dn, attributeName, values);
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes an entry from the LDAP directory with the specified distinguished name.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to delete.</param>
        /// <returns>True if deleted, false otherwise.</returns>
        public bool Delete(string dn)
        {
            if (!string.IsNullOrWhiteSpace(dn))
            {
                DeleteRequest request = new DeleteRequest(dn);
                try
                {
                    DeleteResponse response = (DeleteResponse)connection.SendRequest(request);

                    // Check that a response was received.
                    if (response != null)
                    {
                        // A response was received.
                        if (response.ResultCode == ResultCode.Success)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // A response was not received.
                        return false;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Deletes an attribute's value in the specified entry in the directory.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to delete an attribute from.</param>
        /// <param name="attributeName">The name of the attribute to delete.</param>
        /// <param name="values">Optional: The specific values to delete (if desired). Supplying null will delete all values associated with the attribute.</param>
        /// <returns>True if deleted, false otherwise.</returns>
        public bool DeleteAttribute(string dn, string attributeName, object[] values = null)
        {
            if (!string.IsNullOrWhiteSpace(dn) && !string.IsNullOrWhiteSpace(attributeName))
            {
                ModifyRequest request = null;
                if (values != null)
                {
                    request = new ModifyRequest(dn, DirectoryAttributeOperation.Delete, attributeName, values);
                }
                else
                {
                    request = new ModifyRequest(dn, DirectoryAttributeOperation.Delete, attributeName);
                }

                // This control allows for deletes of values that don't already exist to succeed without throwing an exception.
                // The value is already gone from the attribute so it returns success as if it deleted it.
                request.Controls.Add(new PermissiveModifyControl());

                ModifyResponse response = (ModifyResponse)connection.SendRequest(request);

                // Check that a response was received.
                if (response != null)
                {
                    // A response was received.
                    if (response.ResultCode == ResultCode.Success)
                    {
                        return true;
                    }
                }
                else
                {
                    // A response was not received.
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the first byte array attribute value from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A first byte value held in the attribute, or null array if there was an error retrieving the value or the attribute was empty.</returns>
        public byte[] GetByteAttributeValue(string name, SearchResultEntry entry)
        {
            byte[][] values = GetByteAttributeValues(name, entry);
            if (values.Any())
            {
                return values[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all byte array attribute values from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A list of byte values held in the attribute, or null if there was an error retrieving the values or the attribute was empty.</returns>
        public byte[][] GetByteAttributeValues(string name, SearchResultEntry entry)
        {
            if (!string.IsNullOrWhiteSpace(name) && entry != null)
            {
                if (entry.Attributes.Contains(name))
                {
                    try
                    {
                        return (byte[][])entry.Attributes[name].GetValues(typeof(byte[]));
                    }
                    catch
                    {
                        // There was an error retrieving the value.
                        return null;
                    }
                }
                else
                {
                    // The entry does not contain an attribute with the supplied name.
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets an entry from the directory with the specified distinguished name.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to retrieve.</param>
        /// <param name="attributes">(Optional) The attributes that should be returned in each entry found. If not provided, all attributes are returned.</param>
        /// <param name="baseDn">(Optional)The distinguished name of the base entry where the search will begin. (Typically an OU or the base DN of the directory.) If not supplied, the default values will be used. This base is used only for the duration of this search.</param>
        /// <param name="scope">(Optional) The scope to use while searching. Defaults to Subtree. (Typically Base, just the object with the DN specified; OneLevel, just the child objects of the base object; or Subtree, the base object and all child objects) This scope is used only for the duration of this search.</param>
        /// <returns>The entry with the specified distinguished name or null if the entry does not exist or there was an error in its retrieval.</returns>
        public SearchResultEntry GetEntryByDistinguishedName(string dn, List<string> attributes = null, string baseDn = null, SearchScope scope = SearchScope.Subtree)
        {
            // Check if a distinguished name was provided.
            if (!string.IsNullOrWhiteSpace(dn))
            {
                List<SearchResultEntry> results = Search("(&(distinguishedName=" + dn + "))", attributes, baseDn, searchScope);
                if (results != null && results.Count > 0)
                {
                    return results[0];
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // A distinguished name was not provided.
                return null;
            }
        }

        /// <summary>
        /// Gets the first string attribute value from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A first string value held in the attribute, or null if there was an error retrieving the value or the attribute was empty.</returns>
        public string GetStringAttributeValue(string name, SearchResultEntry entry)
        {
            List<string> values = GetStringAttributeValues(name, entry);
            if (values != null && values.Count > 0)
            {
                return values[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all string attribute values from the supplied entry.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <param name="entry">The SearchResultEntry to get the attribute value from.</param>
        /// <returns>A list of string values held in the attribute, or null if there was an error retrieving the values or the attribute was empty.</returns>
        public List<string> GetStringAttributeValues(string name, SearchResultEntry entry)
        {
            if (!string.IsNullOrWhiteSpace(name) && entry != null)
            {
                if (entry.Attributes.Contains(name))
                {
                    try
                    {
                        return new List<string>((string[])entry.Attributes[name].GetValues(typeof(string)));
                    }
                    catch
                    {
                        // There was an error retrieving the value.
                        return null;
                    }
                }
                else
                {
                    // The entry does not contain an attribute with the supplied name.
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Moves and / or renames an entry in the directory.
        /// </summary>
        /// <param name="distinguishedName">The distinguished name of the entry to move or rename.</param>
        /// <param name="newParentDistinguishedName">The distinguished name of the entry's new parent entry in the directory (if moving), or its current parent entry (if renaming).</param>
        /// <param name="newCommonName">The new common name of entry.</param>
        /// <returns>True if moved or renamed, false otherwise.</returns>
        public bool MoveRenameEntry(string distinguishedName, string newParentDistinguishedName, string newCommonName)
        {
            if (!string.IsNullOrWhiteSpace(distinguishedName) && !string.IsNullOrWhiteSpace(newParentDistinguishedName) && !string.IsNullOrWhiteSpace(newCommonName))
            {
                // Prepend the CN= if not already included.
                if (!newCommonName.StartsWith("CN="))
                {
                    newCommonName = "CN=" + newCommonName;
                }
                ModifyDNRequest request = new ModifyDNRequest(distinguishedName, newParentDistinguishedName, newCommonName);
                try
                {
                    ModifyDNResponse response = (ModifyDNResponse)connection.SendRequest(request);

                    // Check that a response was received.
                    if (response != null)
                    {
                        // A response was received.
                        if (response.ResultCode == ResultCode.Success)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // A response was not received.
                        return false;
                    }
                }
                catch
                {
                }

            }
            return false;
        }

        /// <summary>
        /// Replace an attribute's value in the specified entry in the directory, or replaces all values in a multivalued entry.
        /// </summary>
        /// <param name="dn">The distinguished name of the entry to replace the attribute value of.</param>
        /// <param name="attributeName">The name of the attribute to replace.</param>
        /// <param name="values">The values associated with the attribute to replace.</param>
        /// <returns>True if replaced, false otherwise.</returns>
        private bool ReplaceAttribute(string dn, string attributeName, object[] values)
        {
            if (!string.IsNullOrWhiteSpace(dn) && !string.IsNullOrWhiteSpace(attributeName))
            {
                ModifyRequest request = new ModifyRequest(dn, DirectoryAttributeOperation.Replace, attributeName, values);
                try
                {
                    ModifyResponse response = (ModifyResponse)connection.SendRequest(request);

                    // Check that a response was received.
                    if (response != null)
                    {
                        // A response was received.
                        if (response.ResultCode == ResultCode.Success)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // A response was not received.
                        return false;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Sets the object that is the base for all searches, as well as the scope of the searches.
        /// This only needs to be set if you need to search somewhere other than the base of the directory, or with a scope other than subtree.
        /// </summary>
        /// <param name="distinguishedName">The distinguished name of the object where searches will begin. (Typically an OU or the base DN of the directory.)</param>
        /// <param name="scope">(Optional) The scope to use while searching. Defaults to Subtree. (Typically Base, just the object with the DN specified; OneLevel, just the child objects of the base object; or Subtree, the base object and all child objects)</param>
        /// <returns>True if the search base and scope were set, false otherwise.</returns>
        public bool SetSearchBaseAndScope(string distinguishedName, SearchScope scope = SearchScope.Subtree)
        {
            if (!string.IsNullOrWhiteSpace(distinguishedName))
            {
                searchBaseDN = distinguishedName;
                searchScope = scope;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
