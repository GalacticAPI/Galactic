using System;
using System.Collections.Generic;

namespace Galactic.Identity
{
    /// <summary>
    /// User is an abstract class that defines base functionality that all user classes
    /// implemented by the Galactic API should support.
    /// </summary>
    public abstract class User : IdentityObject, IMailSupportedObject
    {
        // ----- CONSTANTS -----

        // ----- FIELDS -----

        // ----- PROPERTIES -----

        /// <summary>
        /// The user's city.
        /// </summary>
        public abstract string City { get; set; }

        /// <summary>
        /// The user's country code as defined in ISO 3166-1 alpha-2.
        /// </summary>
        public abstract string CountryCode { get; set; }

        /// <summary>
        /// The user's department.
        /// </summary>
        public abstract string Department { get; set; }

        /// <summary>
        /// The user's display name.
        /// </summary>
        public abstract string DisplayName { get; set; }

        /// <summary>
        /// A list of the object's e-mail addresses.
        /// The object's primary e-mail address will always be first in the list.
        /// </summary>
        public abstract List<string> EmailAddresses { get; set; }

        /// <summary>
        /// An organization assigned identifier for the user.
        /// </summary>
        public abstract string EmployeeNumber { get; set; }

        /// <summary>
        /// The user's first name.
        /// </summary>
        public abstract string FirstName { get; set; }

        /// <summary>
        /// Whether the user is disabled or suspended in the system.
        /// </summary>
        public abstract bool IsDisabled { get; }

        /// <summary>
        /// The user's last name.
        /// </summary>
        public abstract string LastName { get; set; }

        /// <summary>
        /// The login name for the user in the system.
        /// </summary>
        public abstract string Login { get; set; }

        /// <summary>
        /// The unique ID of the user's manager in the system.
        /// </summary>
        public abstract string ManagerId { get; set; }

        /// <summary>
        /// The full name of the user's manager.
        /// </summary>
        public abstract string ManagerName { get; }

        /// <summary>
        /// The user's middle name.
        /// </summary>
        public abstract string MiddleName { get; set; }

        /// <summary>
        /// The user's mobile phone number.
        /// </summary>
        public abstract string MobilePhone { get; set; }

        /// <summary>
        /// The name of the organization the user belong's to.
        /// </summary>
        public abstract string Organization { get; set; }

        /// <summary>
        /// Whether the user has to change their password at their next login.
        /// </summary>
        public abstract bool PasswordChangeRequiredAtNextLogin { get; }

        /// <summary>
        /// Whether the user's password has expired.
        /// </summary>
        public abstract bool PasswordExpired { get; }

        /// <summary>
        /// The date and time that the user's password was last set.
        /// </summary>
        public abstract DateTime? PasswordLastSet { get; }

        /// <summary>
        /// The user's physical address.
        /// </summary>
        public abstract string PhysicalAddress { get; set; }

        /// <summary>
        /// The user's postal (mailing) address.
        /// </summary>
        public abstract string PostalAddress { get; set; }

        /// <summary>
        /// The postal code of the user. (ZIP code in the US.)
        /// </summary>
        public abstract string PostalCode { get; set; }

        /// <summary>
        /// The object's primary e-mail address.
        /// </summary>
        public abstract string PrimaryEmailAddress { get; set; }

        /// <summary>
        /// The user's primary phone number.
        /// </summary>
        public abstract string PrimaryPhone { get; set; }

        /// <summary>
        /// The user's state.
        /// </summary>
        public abstract string State { get; set; }

        /// <summary>
        /// The user's title.
        /// </summary>
        public abstract string Title { get; set; }

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Disables the user's account if it is enabled.
        /// </summary>
        /// <returns>True if the account is disabled successfully or was not enabled. False if the account could not be disabled.</returns>
        public abstract bool Disable();

        /// <summary>
        /// Enables the user's account if it is disabled.
        /// </summary>
        /// <returns>True if the account is enabled successfully or was not disabled. False if the account could not be enabled.</returns>
        public abstract bool Enable();

        /// <summary>
        /// Sets the password of the user.
        /// </summary>
        /// <param name="password">The new password to use for the user.</param>
        /// <returns>True if the password was set, false otherwise.</returns>
        public abstract bool SetPassword(string password);

        /// <summary>
        /// Unlocks the user's account if it is locked.
        /// </summary>
        /// <returns>True if the account is unlocked successfully or was not locked. False if the account could not be unlocked.</returns>
        public abstract bool Unlock();
    }
}
