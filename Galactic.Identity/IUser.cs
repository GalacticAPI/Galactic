using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// IUser is an interface that defines functionality that all user classes
    /// implemented by the Galactic API should support.
    /// </summary>
    public interface IUser : IIdentityObject, IMailSupportedObject
    {
        // ----- STATIC CONSTANTS -----

        // ----- STATIC FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The user's city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// The user's department.
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public bool IsDisabled { get; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public string ManagerId { get; set; }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public string ManagerName { get; set; }

        /// <summary>
        /// The user's middle name.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public string MobilePhone { get; set; }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public bool PasswordChangeRequiredAtNextLogin { get; }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public bool PasswordExpired { get; }

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public DateTime? PasswordLastSet { get; }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public string PhyscialAddress { get; set; }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        public string PostalAddress { get; set; }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        public string PrimaryPhone { get; set; }

        /// <summary>
        /// The user's state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// The user's title.
        /// </summary>
        public string Title { get; set; }

        // ----- STATIC CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Disables the user's account if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public bool Disable();

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public bool Enable();

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public bool SetPassword(string password);

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public bool Unlock();
    }
}
