using System;
using System.Linq;
using Microsoft.Graph;
using System.Collections.Generic;
using GraphUser = Microsoft.Graph.User;
using GraphGroup = Microsoft.Graph.Group;
using System.Reflection;

namespace Galactic.Identity.AzureActiveDirectory
{
	public class User : IComparable<User>, IEqualityComparer<User>, IUser
	{
        // ----- CONSTANTS -----

        static protected string[] AttributeNames = { "city", "country", "department", "displayName", "employeeId", "givenName", "accountEnabled", "surname", "userPrincipalName", "manager", "mobilePhone", "companyName" , "passwordPolicies", "passwordProfile", "lastPasswordChangeDateTime", "officeLocation", "streetAddress", "postalCode", "businessPhones", "state", "jobTitle", "createdDateTime", "memberOf", "id", "mail", "proxyAddresses" };

        // ----- VARIABLES -----

        protected AzureActiveDirectoryClient aad = null;

        protected GraphUser graphUser = null;

        // The list of attributes to retrieve when searching for the entry in AD.
        protected List<string> Attributes = new List<string>(AttributeNames);

        // A list of additional attributes that should be retrieved in addition to the basic attributes defined above.
        protected List<string> AdditionalAttributes = new List<string>();

        // ----- PROPERTIES -----

        /// <summary>
        /// The user's city.
        /// </summary>
        [GraphPropertyName("city")]
        public string City
        {
            get
            {
                return graphUser.City;
            }
            set
            {
                GraphUser user = new()
                {
                    City = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        [GraphPropertyName("country")]
        public string CountryCode
        {
            get
            {
                return graphUser.Country;
            }
            set
            {
                GraphUser user = new()
                {
                    Country = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's department.
        /// </summary>
        [GraphPropertyName("department")]
        public string Department
        {
            get
            {
                return graphUser.Department;
            }
            set
            {
                GraphUser user = new()
                {
                    Department = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        [GraphPropertyName("displayName")]
        public string DisplayName
        {
            get
            {
                return graphUser.DisplayName;
            }
            set
            {
                GraphUser user = new()
                {
                    DisplayName = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        [GraphPropertyName("employeeId")]
        public string EmployeeNumber
        {
            get
            {
                return graphUser.EmployeeId;
            }
            set
            {
                GraphUser user = new()
                {
                    EmployeeId = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        [GraphPropertyName("givenName")]
        public string FirstName
        {
            get
            {
                return graphUser.GivenName;
            }
            set
            {
                GraphUser user = new()
                {
                    GivenName = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        [GraphPropertyName("accountEnabled")]
        public bool IsDisabled
        {
            get
            {
                return (bool)graphUser.AccountEnabled;
            }
        }

        /// <summary>
        /// The user's last name.
        /// </summary>
        [GraphPropertyName("surname")]
        public string LastName
        {
            get
            {
                return graphUser.Surname;
            }
            set
            {
                GraphUser user = new()
                {
                    Surname = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        [GraphPropertyName("userPrincipalName")]
        public string Login
        {
            get
            {
                return graphUser.UserPrincipalName;
            }
            set
            {
                GraphUser user = new()
                {
                    UserPrincipalName = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public string ManagerId
        {
            get
            {
                var manager = aad.GetUserManager(UniqueId);

                return manager.Id;
            }
            set
            {
                // Not supported.
            }
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public string ManagerName
        {
            get
            {
                var manager = aad.GetUserManager(UniqueId);

                return manager.DisplayName;
            }
        }

        /// <summary>
        /// The user's middle name.
        /// </summary>
        public string MiddleName
        {
            get
            {
                // Middle name not supported.
                return "";
            }
            set
            {
                // Not supported.
            }
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        [GraphPropertyName("mobilePhone")]
        public string MobilePhone
        {
            get
            {
                return graphUser.MobilePhone;
            }
            set
            {
                GraphUser user = new()
                {
                    MobilePhone = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        [GraphPropertyName("companyName")]
        public string Organization
        {
            get
            {
                return graphUser.CompanyName;
            }
            set
            {
                GraphUser user = new()
                {
                    CompanyName = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public bool PasswordChangeRequiredAtNextLogin
        {
            get
            {
                return false;
                // Not implemented. Graph is not returning the "PasswordProfile" property.
                //return (bool)graphUser.PasswordProfile.ForceChangePasswordNextSignIn;
            }
        }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public bool PasswordExpired
        {
            get
            {
                // Not implemented.
                return false;
            }
        }

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        [GraphPropertyName("lastPasswordChangeDateTime")]
        public DateTime? PasswordLastSet
        {
            get
            {
                return graphUser.LastPasswordChangeDateTime.Value.DateTime;
            }
        }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        [GraphPropertyName("officeLocation")]
        public string PhyscialAddress
        {
            get
            {
                return graphUser.OfficeLocation;
            }
            set
            {
                GraphUser user = new()
                {
                    OfficeLocation = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        [GraphPropertyName("streetAddress")]
        public string PostalAddress
        {
            get
            {
                return graphUser.StreetAddress;
            }
            set
            {
                GraphUser user = new()
                {
                    StreetAddress = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        [GraphPropertyName("postalCode")]
        public string PostalCode
        {
            get
            {
                return graphUser.PostalCode;
            }
            set
            {
                GraphUser user = new()
                {
                    PostalCode = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        [GraphPropertyName("businessPhones")]
        public string PrimaryPhone
        {
            get
            {
                return graphUser.BusinessPhones.First();
            }
            set
            {

            }
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        [GraphPropertyName("state")]
        public string State
        {
            get
            {
                return graphUser.State;
            }
            set
            {
                GraphUser user = new()
                {
                    State = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        [GraphPropertyName("jobTitle")]
        public string Title
        {
            get
            {
                return graphUser.JobTitle;
            }
            set
            {
                GraphUser user = new()
                {
                    JobTitle = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        [GraphPropertyName("createdDateTime")]
        public DateTime? CreationTime
        {
            get
            {
                return graphUser.CreatedDateTime.Value.DateTime;
            }
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public List<IGroup> Groups
        {
            get
            {
                return aad.GetGroupMembership(UniqueId, true);
            }
        }

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        [GraphPropertyName("city")]
        public string Type
        {
            get
            {
                return "User";
            }
        }

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        [GraphPropertyName("id")]
        public string UniqueId
        {
            get
            {
                return graphUser.Id;
            }
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        [GraphPropertyName("proxyAddresses")]
        public List<string> EmailAddresses
        {
            get
            {
                return (List<string>)graphUser.ProxyAddresses;
            }
            set
            {

            }
        }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        [GraphPropertyName("mail")]
        public string PrimaryEmailAddress
        {
            get
            {
                return graphUser.Mail;
            }
            set
            {
                GraphUser user = new()
                {
                    Mail = value
                };

                aad.UpdateUser(UniqueId, user);
            }
        }

        // ----- CONSTRUCTORS -----

        public User(AzureActiveDirectoryClient aad, GraphUser graphUser)
        {
            if (aad != null && graphUser != null)
            {
                // Initialize the client.
                this.aad = aad;

                // Initialize the source GraphUser data.
                this.graphUser = graphUser;
            }
            else
            {
                if (aad == null)
                {
                    throw new ArgumentNullException(nameof(aad));
                }
                else
                {
                    throw new ArgumentNullException(nameof(graphUser));
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Refresh user properties with updated data.
        /// </summary>
        public void Refresh()
        {
            graphUser = aad.GetGraphUser(UniqueId);
        }

        /// <summary>
        /// Disables the user's account if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public bool Disable()
        {
            if(!IsDisabled)
            {
                GraphUser user = new()
                {
                    AccountEnabled = false
                };

                return aad.UpdateUser(UniqueId, user);
            }
            else
            {
                // Account is already disabled, nothing to do.
                return true;
            }
        }

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public bool Enable()
        {
            if(IsDisabled)
            {
                GraphUser user = new()
                {
                    AccountEnabled = true
                };

                return aad.UpdateUser(UniqueId, user);
            }
            else
            {
                // Account is already active, nothing to do.
                return true;
            }
        }

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public bool SetPassword(string password)
        {
            GraphUser user = new()
            {
                PasswordProfile = new()
                {
                    ForceChangePasswordNextSignIn = false,
                    Password = password
                }
            };

            return aad.UpdateUser(UniqueId, user);
        }

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public bool Unlock()
        {
            return Enable();
        }

        /// <summary>
        /// Adds this principal to the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to add the principal to.</param>
        /// <returns>True if the principal was added, false otherwise.</returns>
        public bool AddToGroup(Group group)
        {
            if (group != null)
            {
                if (!aad.AddObjectToGroup(UniqueId, group.UniqueId))
                {
                    return false;
                }

                return true;
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Gets the values of the attributes associated with the supplied names.
        /// </summary>
        /// <param name="names">The names of the attributes to get the values of.</param>
        /// <returns>A list of identity attributes that contain the attribute's name and value, or null if no values could be returned.</returns>
        public List<IdentityAttribute<Object>> GetAttributes(List<string> names)
        {
            // Create a list of IdentityAttributes to return.
            List<IdentityAttribute<object>> attributes = new();

            if (names != null)
            {
                // Create a dictionary of properties in this class keyed by name.
                PropertyInfo[] propertyInfoList = typeof(User).GetProperties();
                Dictionary<string, PropertyInfo> properties = new();
                foreach (PropertyInfo propertyInfo in propertyInfoList)
                {
                    foreach (GraphPropertyNameAttribute attribute in propertyInfo.GetCustomAttributes<GraphPropertyNameAttribute>())
                    {
                        properties.Add(attribute.Name, propertyInfo);
                    }
                }

                // Fill the list of IdentityAttributes with the name and value of the attribute with the supplied name.
                foreach (string name in names)
                {
                    if (properties.ContainsKey(name))
                    {
                        attributes.Add(new(name, properties[name].GetValue(this)));
                    }
                }
            }

            // Return the attributes found.
            return attributes;
        }

        /// <summary>
        /// Checks if the identity object is a member of the supplied group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="recursive">Whether to do a recursive lookup of all sub groups that this object might be a member of.</param>
        /// <returns>True if the object is a member, false otherwise.</returns>
        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            if (group != null)
            {
                IList<string> results = aad.CheckGroupMembership(UniqueId, new List<string> { group.UniqueId });

                if(results != null && results.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(group));
            }
        }

        /// <summary>
        /// Removes this principal from the supplied group.
        /// </summary>
        /// <param name="guid">The GUID of the group to add the principal to.</param>
        /// <returns>True if the principal was removed, false otherwise.</returns>
        public bool RemoveFromGroup(Group group)
        {
            if (group != null)
            {
                return aad.DeleteObjectFromGroup(UniqueId, group.UniqueId);
            }
            return false;
        }

        /// <summary>
        /// Sets attribute values of an identity object. If null or empty values are supplied the attribute's value will be deleted.
        /// </summary>
        /// <param name="attributes">The attribute to set.</param>
        /// <returns>A list of identity attributes that have values of true if the attribute was set successfully, or false otherwise.</returns>
        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<Object>> attributes)
        {
            List<IdentityAttribute<bool>> results = new List<IdentityAttribute<bool>>();

            foreach (var attribute in attributes)
            {
                results.Add(new IdentityAttribute<bool>(attribute.Name, aad.UpdateUser(UniqueId, new List<IdentityAttribute<object>> { attribute })));
            }

            return results;
        }

        /// <summary>
        /// Compares this identity object to another identity object.
        /// </summary>
        /// <param name="other">The other identity object to compare this one to.</param>
        /// <returns>1 iif the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order.</returns>
        public int CompareTo(IIdentityObject other)
        {
            return ((IIdentityObject)this).CompareTo(other);
        }

        /// <summary>
        /// Compares this User to another User.
        /// </summary>
        /// <param name="other">The other User to compare this one to.</param>
        /// <returns>-1 if the object supplied comes before this one in the sort order, 0 if they occur at the same position, 1 if the object supplied comes after this one in the sort order</returns>
        public int CompareTo(User other)
        {
            if (other != null)
            {
                return string.Compare(UniqueId.ToString(), other.UniqueId.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                throw new ArgumentNullException("other");
            }
        }

        /// <summary>
        /// Checks whether x and y are equal (have the same UniqueIds).
        /// </summary>
        /// <param name="x">The first identity object to check.</param>
        /// <param name="y">The second identity object to check.</param>
        /// <returns>True if the identity objects are equal, false otherwise.</returns>
        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            return ((IIdentityObject)this).Equals(x, y);
        }

        /// <summary>
        /// Checks whether x and y are equal (using GUIDs).
        /// </summary>
        /// <param name="x">The first User to check.</param>
        /// <param name="y">The second User to check against.</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public bool Equals(User x, User y)
        {
            if (x != null && y != null)
            {
                return x.UniqueId.Equals(y.UniqueId);
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
        /// Generates a hash code for the identity object supplied.
        /// </summary>
        /// <param name="obj">The identity object to generate a hash code for.</param>
        /// <returns>An integer hash code for the identity object.</returns>
        public int GetHashCode(IIdentityObject obj) => IIdentityObject.GetHashCode(obj);

        /// <summary>
        /// Generates a hash code for the User supplied.
        /// </summary>
        /// <param name="obj">The User to generate a hash code for.</param>
        /// <returns>An integer hash code for the object.</returns>
        public int GetHashCode(User obj)
        {
            if (obj != null)
            {
                return obj.UniqueId.GetHashCode();
            }
            else
            {
                throw new ArgumentNullException("obj");
            }
        }
    }

}

