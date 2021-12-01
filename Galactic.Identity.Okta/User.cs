using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Galactic.Identity.Okta
{
    public class User : IUser
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        /// <summary>
        /// The object used to query and manipulate Okta.
        /// </summary>
        protected OktaClient okta = null;

        /// <summary>
        /// The backing JSON data representing the User in Okta.
        /// </summary>
        protected UserJson json = null;

        // ----- PROPERTIES -----

        /// <summary>
        /// Timestamp when User's transition to ACTIVE status completed.
        /// </summary>
        public DateTime? Activated => json.Activated;

        /// <summary>
        /// The user's city.
        /// </summary>
        public string City
        {
            get => json.Profile.City;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Name of a cost center assigned to the User.
        /// </summary>
        public string CostCenter
        {
            get => json.Profile.CostCenter;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        public string CountryCode
        {
            get => json.Profile.CountryCode;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Timestamp when User was created.
        /// </summary>
        public DateTime? Created
        {
            get => json.Created;
        }

        /// <summary>
        /// The date and time that the object was created.
        /// </summary>
        public DateTime? CreationTime => Created;

        /// <summary>
        /// The user's department.
        /// </summary>
        public string Department
        {
            get => json.Profile.Department;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string DisplayName
        {
            get => json.Profile.DisplayName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The User's division.
        /// </summary>
        public string Division
        {
            get => json.Profile.Division;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The User's primary e-mail address.
        /// </summary>
        public string Email
        {
            get => json.Profile.Email;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public List<string> EmailAddresses
        {
            get
            {
                return new() { Email, SecondEmail };
            }
            set
            {
                if (value != null)
                {
                    if (value.Count > 0)
                    {
                        Email = value[0];
                    }
                    if (value.Count > 1)
                    {
                        SecondEmail = value[1];
                    }
                }
            }
        }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        public string EmployeeNumber
        {
            get => json.Profile.EmployeeNumber;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName
        {
            get => json.Profile.FirstName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The list of groups this object is a member of.
        /// </summary>
        public List<IGroup> Groups => throw new NotImplementedException();

        /// <summary>
        /// The honorific prefix(es) of the User, or title in most Western languages.
        /// </summary>
        public string HonorificPrefix
        {
            get => json.Profile.HonorificPrefix;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The honorific suffix(es) of the User.
        /// </summary>
        public string HonorificSuffix
        {
            get => json.Profile.HonorificSuffix;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The User's unique key.
        /// </summary>
        public string Id => json.Id;


        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public bool IsDisabled => throw new NotImplementedException();

        /// <summary>
        /// Timestamp of User's last login.
        /// </summary>
        public DateTime? LastLogin => json.LastLogin;

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName
        {
            get => json.Profile.LastName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Timestamp when User was last updated.
        /// </summary>
        public DateTime? LastUpdated => json.LastUpdated;

        /// <summary>
        /// The User's default location for purposes of localizing items such as
        /// currency, date time format, numerical representations, etc.
        /// </summary>
        public string Locale
        {
            get => json.Profile.Locale;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        public string Login
        {
            get => json.Profile.Login;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The display name of the User's manager.
        /// </summary>
        public string Manager
        {
            get => json.Profile.Manager;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public string ManagerId
        {
            get => json.Profile.ManagerId;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public string ManagerName => Manager;

        /// <summary>
        /// The user's middle name.
        /// </summary>
        public string MiddleName
        {
            get => json.Profile.MiddleName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public string MobilePhone
        {
            get => json.Profile.MobilePhone;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The casual way to address the User in real life.
        /// </summary>
        public string NickName
        {
            get => json.Profile.NickName;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        public string Organization
        {
            get => json.Profile.Organization;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Timestamp when User's password was last changed.
        /// </summary>
        public DateTime? PasswordChanged => json.PasswordChanged;

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public bool PasswordChangeRequiredAtNextLogin => throw new NotImplementedException();

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public bool PasswordExpired => throw new NotImplementedException();

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public DateTime? PasswordLastSet => PasswordChanged;

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public string PhyscialAddress
        {
            get => StreetAddress;
            set
            {
                StreetAddress = value;
            }
        }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        public string PostalAddress
        {
            get => json.Profile.PostalAddress;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public string PostalCode
        {
            get => ZipCode;
            set
            {
                ZipCode = value;
            }
        }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public string PrimaryEmailAddress
        {
            get => Email;
            set
            {
                Email = value;
            }
        }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        public string PrimaryPhone
        {
            get => json.Profile.PrimaryPhone;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The URL of the User's online profile (e.g. web page).
        /// </summary>
        public string ProfileUrl
        {
            get => json.Profile.ProfileUrl;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The secondary e-mail address of the User, typically used for account
        /// recovery.
        /// </summary>
        public string SecondEmail
        {
            get => json.Profile.SecondEmail;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's state.
        /// </summary>
        public string State
        {
            get => json.Profile.State;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The User's current status.
        /// </summary>
        public string Status => json.Status;

        /// <summary>
        /// Timestamp when User's status last changed.
        /// </summary>
        public DateTime? StatusChanged => json.StatusChanged;

        /// <summary>
        /// The full street address component of the User's address.
        /// </summary>
        public string StreetAddress
        {
            get => json.Profile.StreetAddress;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The User's time zone.
        /// </summary>
        public string TimeZone
        {
            get => json.Profile.TimeZone;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The user's title.
        /// </summary>
        public string Title
        {
            get => HonorificPrefix;
            set
            {
                HonorificPrefix = value;
            }
        }

        /// <summary>
        /// The target status of an in-progress asynchronous status transition
        /// of the User.
        /// </summary>
        public string TransitioningToStatus => json.TransitioningToStatus;

        /// <summary>
        /// The type or category of the object. Empty if unknown.
        /// </summary>
        public string Type => json.Type;

        /// <summary>
        /// The object's unique ID in the system.
        /// </summary>
        public string UniqueId => Id;

        /// <summary>
        /// Used to describe the organization to user relationship such as "Employee"
        /// or "Contractor".
        /// </summary>
        public string UserType
        {
            get => json.Profile.UserType;
            set => throw new NotImplementedException();
        }

        /// <summary>
        /// The ZIP code or postal code component of the User's address.
        /// </summary>
        public string ZipCode
        {
            get => json.Profile.ZipCode;
            set => throw new NotImplementedException();
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Initializes an Okta user from an object representing its JSON properties.
        /// </summary>
        /// <param name="okta">An Okta object used to query and manipulate the user.</param>
        /// <param name="json">An object representing this user's JSON properties.</param>
        public User(OktaClient okta, UserJson json)
        {
            if (okta != null && json != null)
            {
                // Initialize the client.
                this.okta = okta;

                // Initialize the backing JSON data.
                this.json = json;
            }
            else
            {
                if (okta == null)
                {
                    throw new ArgumentNullException(nameof(okta));
                }
                else
                {
                    throw new ArgumentNullException(nameof(json));
                }
            }
        }

        // ----- METHODS -----

        public int CompareTo(IIdentityObject other)
        {
            throw new NotImplementedException();
        }

        public bool Disable()
        {
            throw new NotImplementedException();
        }

        public bool Enable()
        {
            throw new NotImplementedException();
        }

        public bool Equals(IIdentityObject x, IIdentityObject y)
        {
            throw new NotImplementedException();
        }

        public List<IdentityAttribute<object>> GetAttributes(List<string> names)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] IIdentityObject obj)
        {
            throw new NotImplementedException();
        }

        public bool MemberOfGroup(IGroup group, bool recursive)
        {
            throw new NotImplementedException();
        }

        public List<IdentityAttribute<bool>> SetAttributes(List<IdentityAttribute<object>> attributes)
        {
            throw new NotImplementedException();
        }

        public bool SetPassword(string password)
        {
            throw new NotImplementedException();
        }

        public bool Unlock()
        {
            throw new NotImplementedException();
        }
    }
}
