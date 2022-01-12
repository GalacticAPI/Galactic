using Google.Apis.Admin.Directory.directory_v1;
using System;

namespace Galactic.Identity.GoogleWorkspace
{
    /// <summary>
    /// A record representing the available scopes for requesting access to the Directory API.
    /// Defined in: https://developers.google.com/identity/protocols/oauth2/scopes#admin-directory
    /// </summary>
    public record DirectoryApiScopes
    {
        // ----- CONSTANTS -----


        // ----- PROPERTIES -----

        /// <summary>
        /// View and manage customer related information.
        /// </summary>
        public static string Customer => DirectoryService.Scope.AdminDirectoryCustomer;

        /// <summary>
        /// View customer related information.
        /// </summary>
        public static string CustomerReadOnly => DirectoryService.Scope.AdminDirectoryCustomerReadonly;

        /// <summary>
        /// View and manage your Chrome OS devices' metadata.
        /// </summary>
        public static string DeviceChromeOS => DirectoryService.Scope.AdminDirectoryDeviceChromeos;

        /// <summary>
        /// View your Chrome OS devices' metadata.
        /// </summary>
        public static string DeviceChromeOSReadOnly => DirectoryService.Scope.AdminDirectoryDeviceChromeosReadonly;

        /// <summary>
        /// View and manage your mobile devices' metadata.
        /// </summary>
        public static string DeviceMobile => DirectoryService.Scope.AdminDirectoryDeviceMobile;

        /// <summary>
        /// Manage your mobile devices by performing administrative tasks.
        /// </summary>
        public static string DeviceMobileAction => DirectoryService.Scope.AdminDirectoryDeviceMobileAction;

        /// <summary>
        /// View your mobile devices' metadata.
        /// </summary>
        public static string DeviceMobileReadOnly => DirectoryService.Scope.AdminDirectoryDeviceMobileReadonly;

        /// <summary>
        /// View and manage the provisioning of domains for your customers.
        /// </summary>
        public static string Domain => DirectoryService.Scope.AdminDirectoryDomain;

        /// <summary>
        /// View domains related to your customers.
        /// </summary>
        public static string DomainReadOnly => DirectoryService.Scope.AdminDirectoryDomainReadonly;

        /// <summary>
        /// View and manage the provisioning of groups in your domain.
        /// </summary>
        public static string Group => DirectoryService.Scope.AdminDirectoryGroup;

        /// <summary>
        /// View and manage group subscriptions on your domain.
        /// </summary>
        public static string GroupMember => DirectoryService.Scope.AdminDirectoryGroupMember;

        /// <summary>
        /// View group subscription on your domain.
        /// </summary>
        public static string GroupMemberReadOnly => DirectoryService.Scope.AdminDirectoryGroupMemberReadonly;

        /// <summary>
        /// View groups on your domain.
        /// </summary>
        public static string GroupReadOnly => DirectoryService.Scope.AdminDirectoryGroupReadonly;

        /// <summary>
        /// View and manage organization units on your domain.
        /// </summary>
        public static string OrgUnit => DirectoryService.Scope.AdminDirectoryOrgunit;

        /// <summary>
        /// View organization units on your domain.
        /// </summary>
        public static string OrgUnitReadOnly => DirectoryService.Scope.AdminDirectoryOrgunitReadonly;

        /// <summary>
        /// View and manage the provisioning of calendar resources on your domain.
        /// </summary>
        public static string ResourceCalendar => DirectoryService.Scope.AdminDirectoryResourceCalendar;

        /// <summary>
        /// View calendar resources on your domain.
        /// </summary>
        public static string ResourceCalendarReadOnly => DirectoryService.Scope.AdminDirectoryResourceCalendarReadonly;

        /// <summary>
        /// Manage delegated admin roles for your domain.
        /// </summary>
        public static string RoleManagement => DirectoryService.Scope.AdminDirectoryRolemanagement;

        /// <summary>
        /// View delegated admin roles for your domain.
        /// </summary>
        public static string RoleManagementReadOnly => DirectoryService.Scope.AdminDirectoryRolemanagementReadonly;

        /// <summary>
        /// View and manage the provisioning of users on your domain.
        /// </summary>
        public static string User => DirectoryService.Scope.AdminDirectoryUser;

        /// <summary>
        /// View and manage user aliases on your domain.
        /// </summary>
        public static string UserAlias => DirectoryService.Scope.AdminDirectoryUserAlias;

        /// <summary>
        /// View user aliases on your domain.
        /// </summary>
        public static string UserAliasReadOnly => DirectoryService.Scope.AdminDirectoryUserAliasReadonly;

        /// <summary>
        /// View users on your domain.
        /// </summary>
        public static string UserReadOnly => DirectoryService.Scope.AdminDirectoryUserReadonly;

        /// <summary>
        /// Manage data access permissions for users on your domain.
        /// </summary>
        public static string UserSecurity => DirectoryService.Scope.AdminDirectoryUserSecurity;

        /// <summary>
        /// View and manage the provisioning of user schemas on your domain.
        /// </summary>
        public static string UserSchema => DirectoryService.Scope.AdminDirectoryUserschema;

        /// <summary>
        /// View user schemas on your domain.
        /// </summary>
        public static string UserSchemaReadOnly => DirectoryService.Scope.AdminDirectoryUserschemaReadonly;

        /// <summary>
        /// View and manage your data across Google Cloud Platform services.
        /// </summary>
        public static string CloudPlatform => DirectoryService.Scope.CloudPlatform;
    }
}

