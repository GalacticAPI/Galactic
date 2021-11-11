using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.IO;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using SystemDirectory = System.IO.Directory;

namespace Galactic.FileSystem
{
    /// <summary>
    /// A Windows-specific utility class for manipulating directories on the file system. 
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class WindowsDirectory
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Valid access rights that may be applied to directories.
        /// </summary>
        public enum AccessRights
        {
            /// <summary>
            /// Create files/write data.
            /// </summary>
            CreateFilesWriteData,

            /// <summary>
            /// Full control.
            /// </summary>
            FullControl,

            /// <summary>
            /// List folder/read data.
            /// </summary>
            ListFolderReadData,

            /// <summary>
            /// Modify.
            /// </summary>
            Modify,

            /// <summary>
            /// Read.
            /// </summary>
            Read,

            /// <summary>
            /// Read attributes.
            /// </summary>
            ReadAttributes,

            /// <summary>
            /// Read extended attributes.
            /// </summary>
            ReadExtendedAttributes,

            /// <summary>
            /// Read permissions.
            /// </summary>
            ReadPermissions,

            /// <summary>
            /// Traverse folder / execute file permissions.
            /// </summary>
            TraverseFolderExecuteFile
        }

        // ----- VARIABLES -----

        // ----- PROPERTITES -----

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Adds an access rule to the directory at the supplied path.
        /// </summary>
        /// <param name="path">The path to the directory to add the rule to.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="rule">The rule to add to the directory.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if the rule was added. False if the directory does not exist, the rule is null, or the process does not have access to
        /// the specified path, or does not have sufficient access to change the ACL entry of the directory, or the operating system is not Windows
        /// 2000 or later.</returns>
        static public bool AddAccessRule(string path, ref DirectorySecurity security, FileSystemAccessRule rule, bool commitChanges)
        {
            // Check that a path, security object, and rule are supplied.
            if (!string.IsNullOrEmpty(path) && security != null && rule != null)
            {
                // A path, security object, and rule are supplied.
                // Check whether the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    // Add the access rule to the directory.
                    security.AddAccessRule(rule);

                    // Commit the changes if necessary.
                    if (commitChanges)
                    {
                        try
                        {
                            FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // The current process does not have access to the directory specified by the path.
                            // Or the current process does not have sufficient privilege to set the ACL entry.
                            return false;
                        }
                        catch (PlatformNotSupportedException)
                        {
                            // The current operating system is not Windows 2000 or later.
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            else
            {
                // A path, security object, and rule were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Blocks inheritance on this directory.
        /// </summary>
        /// <param name="path">The path to the directory to block inheritance on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="addInheritedPermissions">If true, copies the directory's inherited permissions as explicit permissions on the directory.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this entry. Useful when combining multiple commands.</param>
        /// <returns>True if inheritance was blocked on the directory, false if the directory does not exist, or inheritance could not be
        /// blocked.</returns>
        static public bool BlockInheritance(string path, ref DirectorySecurity security, bool addInheritedPermissions, bool commitChanges)
        {
            // Check whether a path and security object were supplied.
            if (!string.IsNullOrEmpty(path) && security != null)
            {
                // A path and security object were supplied.
                // Check whether the directory exists.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.
                    // Remove inheritance from the directory and copy inherited permissions if necessary.
                    try
                    {
                        security.SetAccessRuleProtection(true, addInheritedPermissions);
                    }
                    catch (InvalidOperationException)
                    {
                        // This method attempted to remove inherited rules from a non-canonical Discretionary Access Control List (DACL).
                        return false;
                    }

                    // Commit the changes if necessary.
                    if (commitChanges)
                    {
                        try
                        {
                            FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // The current process does not have access to the directory specified by path.
                            // Or the current process does not have sufficient privilege to set the ACL entry.
                            return false;
                        }
                        catch (PlatformNotSupportedException)
                        {
                            // The current operating system is not Windows 2000 or later.
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            else
            {
                // A path or security object were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Commits any pending changes to the directory specified by the supplied path.
        /// </summary>
        /// <param name="path">The path to the directory to commit changes on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <returns>True if the changes were commited. False if the directory does not exist,
        /// or the current process does not have sufficient access to the specified path, or the
        /// current operating system in not Windows 2000 or later.</returns>
        static public bool CommitChanges(string path, ref DirectorySecurity security)
        {
            // Check that a path and security object were supplied.
            if (!string.IsNullOrEmpty(path) && security != null)
            {
                // Check whether the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    try
                    {
                        FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // The current process does not have access to the directory specified by path.
                        // Or the current process does not have sufficient privilege to set the ACL entry.
                        return false;
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // The current operating system is not Windows 2000 or later.
                        return false;
                    }
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            // The path or security object were not supplied.
            return false;
        }

        /// <summary>
        /// Gets the DirectorySecurity object for the directory specified by the supplied path.
        /// </summary>
        /// <param name="path">The path to the directory to retrieve the security object for.</param>
        /// <returns>The security object for the directory specified. Null if the directory does not exist,
        /// an I/O Error occurred, the current operating system is not Windows 2000 or later,
        /// the path specified is read-only, or the process does not have permission to complete the operation.</returns>
        static public DirectorySecurity GetSecurityObject(string path)
        {
            // Check that a path is supplied.
            if (!string.IsNullOrEmpty(path))
            {
                // A path is supplied.

                // Check whether the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.
                    try
                    {
                        return FileSystemAclExtensions.GetAccessControl(new DirectoryInfo(path));
                    }
                    catch (IOException)
                    {
                        // An I/O error occurred while opening the directory.
                        return null;
                    }
                    catch (PlatformNotSupportedException)
                    {
                        // The current operating system is not Windows 2000 or later.
                        return null;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // The path parameter specified a directory that is read-only.
                        // Or this operation is not supported on the current platform.
                        // Or the caller does not have the required permission.
                        return null;
                    }
                }
                else
                {
                    // The directory does not exist.
                    return null;
                }
            }
            else
            {
                // A path was not supplied.
                return null;
            }
        }

        /// <summary>
        /// Gives access to the account with the supplied security identifier on the target directory.
        /// </summary>
        /// <param name="accountSid">The security identifier (SID) of the account that should be given access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="rights">The access rights to grant on the target directory.</param>
        /// <param name="applyToSubfolders">Indicates whether this directory's permissions apply to subfolders beneath it.</param>
        /// <param name="applyToFiles">Indicates whether this directory's permissions apply to files beneath it.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        static public bool GiveAccess(byte[] accountSid, string path, ref DirectorySecurity security, FileSystemRights rights,
            bool applyToSubfolders, bool applyToFiles, bool commitChanges)
        {
            // Check whether the accountSid, path, and security object are supplied.
            if (accountSid != null && !string.IsNullOrEmpty(path) && security != null)
            {
                // An accountSid, path, and security object are supplied.

                // Check whether the directory exists.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.
                    // Get the security identifier (SID) of the entry.
                    SecurityIdentifier sid = new SecurityIdentifier(accountSid, 0);

                    // Create allow rules for the sid.

                    // Check if permissions should be inherited by subfolders.
                    if (applyToSubfolders)
                    {
                        // Permissions should be inherited by subfolders.
                        FileSystemAccessRule containerRule = new FileSystemAccessRule(sid, rights, InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);

                        // Check if there was an error applying the rule.
                        if (!AddAccessRule(path, ref security, containerRule, commitChanges))
                        {
                            return false;
                        }
                    }
                    // Check if permissions should be inherited by files.
                    if (applyToFiles)
                    {
                        // Permissions should be inherited by files.
                        FileSystemAccessRule objectRule = new FileSystemAccessRule(sid, rights, InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);

                        // Check if there was an error applying the rule.
                        if (!AddAccessRule(path, ref security, objectRule, commitChanges))
                        {
                            return false;
                        }
                    }
                    // The rules were successfully applied.
                    return true;
                }
                // The directory does not exist.
                return false;
            }
            else
            {
                // An entry, path, and security object were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Gives access to the entry supplied on the target directory.
        /// </summary>
        /// <param name="entry">The entry to give access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="rights">The access rights to grant on the target directory.</param>
        /// <param name="applyToSubfolders">Indicates whether this directory's permissions apply to subfolders beneath it.</param>
        /// <param name="applyToFiles">Indicates whether this directory's permissions apply to files beneath it.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        static public bool GiveAccess(DirectoryEntry entry, string path, ref DirectorySecurity security, FileSystemRights rights, bool applyToSubfolders, bool applyToFiles, bool commitChanges)
        {
            // Check whether the entry was supplied.
            if (entry != null)
            {
                // The entry was supplied.
                // Get the SID of from the supplied entry.
                byte[] sid = (byte[])entry.Properties["objectSid"].Value;

                return GiveAccess(sid, path, ref security, rights, applyToSubfolders, applyToFiles, commitChanges);
            }
            else
            {
                // An entry was not supplied.
                return false;
            }
        }

        /// <summary>
        /// Gives the supplied data access rights to the account with the supplied security identifier on the target directory.
        /// </summary>
        /// <param name="rights">The rights to apply to the target directory.</param>
        /// <param name="accountSid">The security identifier (SID) of the account that should be given access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="applyToSubfolders">Indicates whether this directory's permissions apply to subfolders beneath it.</param>
        /// <param name="applyToFiles">Indicates whether this directory's permissions apply to files beneath it.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        public static bool GiveAccess(AccessRights rights, byte[] accountSid, string path, ref DirectorySecurity security, bool applyToSubfolders, bool applyToFiles, bool commitChanges)
        {
            // Rights were supplied.

            // Get the type(s) of FileSystemRights to apply from the AccessRights supplied.
            List<FileSystemRights> fileSystemRights = new List<FileSystemRights>();
            switch (rights)
            {
                case AccessRights.CreateFilesWriteData:
                    fileSystemRights.Add(FileSystemRights.CreateFiles);
                    fileSystemRights.Add(FileSystemRights.WriteData);
                    break;
                case AccessRights.FullControl:
                    fileSystemRights.Add(FileSystemRights.FullControl);
                    break;
                case AccessRights.ListFolderReadData:
                    fileSystemRights.Add(FileSystemRights.ListDirectory);
                    fileSystemRights.Add(FileSystemRights.ReadData);
                    break;
                case AccessRights.Modify:
                    fileSystemRights.Add(FileSystemRights.Modify);
                    break;
                case AccessRights.Read:
                    fileSystemRights.Add(FileSystemRights.Read);
                    break;
                case AccessRights.ReadAttributes:
                    fileSystemRights.Add(FileSystemRights.ReadAttributes);
                    break;
                case AccessRights.ReadExtendedAttributes:
                    fileSystemRights.Add(FileSystemRights.ReadExtendedAttributes);
                    break;
                case AccessRights.ReadPermissions:
                    fileSystemRights.Add(FileSystemRights.ReadPermissions);
                    break;
                case AccessRights.TraverseFolderExecuteFile:
                    fileSystemRights.Add(FileSystemRights.Traverse);
                    break;
                default:
                    // Incorrect access rights specified.
                    return false;
            }

            // Give the desired access rights to the directory.
            foreach (FileSystemRights right in fileSystemRights)
            {
                if (!GiveAccess(accountSid, path, ref security, right, applyToSubfolders, applyToFiles, commitChanges))
                {
                    // The access right could not be applied.
                    return false;
                }
            }
            // All rights were applied successfully.
            return true;
        }

        /// <summary>
        /// Gives the supplied data access rights to the account with the supplied security
        /// identifier on the target directory. Applies inherited permissions to folders and
        /// files beneath the target directory.
        /// </summary>
        /// <param name="rights">The rights to apply to the target directory.</param>
        /// <param name="accountSid">The security identifier (SID) of the account that should be given access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        public static bool GiveAccess(AccessRights rights, byte[] accountSid, string path, ref DirectorySecurity security, bool commitChanges)
        {
            return GiveAccess(rights, accountSid, path, ref security, true, true, commitChanges);
        }

        /// <summary>
        /// Gives the supplied data access rights to the entry supplied on the target directory.
        /// Applies inherited permissions to folders and files beneath the target directory.
        /// </summary>
        /// <param name="rights">The rights to apply to the target directory.</param>
        /// <param name="entry">The entry to give data access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        public static bool GiveAccess(AccessRights rights, DirectoryEntry entry, string path, ref DirectorySecurity security, bool commitChanges)
        {
            return GiveAccess(rights, entry, path, ref security, true, true, commitChanges);
        }

        /// <summary>
        /// Gives the supplied data access rights to the entry supplied on the target directory.
        /// </summary>
        /// <param name="rights">The rights to apply to the target directory.</param>
        /// <param name="entry">The entry to give data access.</param>
        /// <param name="path">The path to the directory to have access granted on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="applyToSubfolders">Indicates whether this directory's permissions apply to subfolders beneath it.</param>
        /// <param name="applyToFiles">Indicates whether this directory's permissions apply to files beneath it.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was granted. False otherwise.</returns>
        public static bool GiveAccess(AccessRights rights, DirectoryEntry entry, string path, ref DirectorySecurity security, bool applyToSubfolders, bool applyToFiles, bool commitChanges)
        {
            // Check whether the entry was supplied.
            if (entry != null)
            {
                // The entry was supplied.
                // Get the SID of from the supplied entry.
                byte[] sid = (byte[])entry.Properties["objectSid"].Value;

                return GiveAccess(rights, sid, path, ref security, applyToSubfolders, applyToFiles, commitChanges);
            }
            else
            {
                // An entry was not supplied.
                return false;
            }
        }

        /// <summary>
        /// Removes all access rules from the supplied directory.
        /// </summary>
        /// <param name="path">The path to the directory to remove all access rules from.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if all rules were removed. False if an error occurred.</returns>
        static public bool RemoveAllAccessRules(string path, ref DirectorySecurity security, bool commitChanges)
        {
            // Check whether a path and security object were supplied.
            if (!string.IsNullOrEmpty(path) && security != null)
            {
                // A path and security object were supplied.
                // Check whether the path exists.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.
                    try
                    {
                        // Get all the authorization rules for the directory.
                        AuthorizationRuleCollection ruleCollection = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

                        // Remove all the authorization rules for the entry.
                        foreach (FileSystemAccessRule rule in ruleCollection)
                        {
                            security.RemoveAccessRuleSpecific(rule);
                        }

                        // Commit the changes if necessary.
                        if (commitChanges)
                        {
                            try
                            {
                                FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // The current process does not have access to the directory specified by path.
                                // Or the current process does not have sufficient privilege to set the ACL entry.
                                return false;
                            }
                            catch (PlatformNotSupportedException)
                            {
                                // The current operating system is not Windows 2000 or later.
                                return false;
                            }
                        }
                        return true;
                    }
                    catch
                    {
                        // There was an error removing the rules.
                        return false;
                    }
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            else
            {
                // An directory or security object were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Removes all explicit access rules from the supplied directory.
        /// </summary>
        /// <param name="path">The path to the directory to have access removed on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this directory. Useful when combining multiple commands.</param>
        /// <returns>True if access was removed. False otherwise.</returns>
        static public bool RemoveAllExplicitAccessRules(string path, ref DirectorySecurity security, bool commitChanges)
        {
            // Check whether the path and security object are supplied.
            if (!string.IsNullOrEmpty(path) && security != null)
            {
                // Check whether the directory exists.
                if (SystemDirectory.Exists(path))
                {
                    // A path and security object are supplied.
                    // Remove existing explicit permissions.
                    security = GetSecurityObject(path);
                    AuthorizationRuleCollection rules = security.GetAccessRules(true, false, typeof(SecurityIdentifier));
                    foreach (AuthorizationRule rule in rules)
                    {
                        security.RemoveAccessRule((FileSystemAccessRule)rule);
                    }
                    // Commit the changes if necessary.
                    if (commitChanges)
                    {
                        try
                        {
                            FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // The current process does not have access to the directory specified by path.
                            // Or the current process does not have sufficient privilege to set the ACL entry.
                            return false;
                        }
                        catch (PlatformNotSupportedException)
                        {
                            // The current operating system is not Windows 2000 or later.
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            else
            {
                // A path and security object were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Sets the owner of a directory.
        /// </summary>
        /// <param name="path">The path to the directory to have the ownership set on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="ownerSid">The security identifier (SID) of the account that should take ownership of the entry.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this entry. Useful when combining multiple commands.</param>
        /// <returns>True if the ownership could be set. False otherwise.</returns>
        static public bool SetOwner(string path, ref DirectorySecurity security, byte[] ownerSid, bool commitChanges)
        {
            // Check whether a path, security object, and owner were supplied.
            if (!string.IsNullOrEmpty(path) && security != null && ownerSid != null)
            {
                // A path, security object, and owner were supplied.
                // Check whether the directory exists.
                if (SystemDirectory.Exists(path))
                {
                    try
                    {
                        // Get the security identifier (SID) of the owner.
                        SecurityIdentifier sid = new SecurityIdentifier(ownerSid, 0);

                        // Set the owner of the directory to the SID of the owner entry.
                        security.SetOwner(sid);

                        // Commit the changes if necessary.
                        if (commitChanges)
                        {
                            try
                            {
                                FileSystemAclExtensions.SetAccessControl(new DirectoryInfo(path), security);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // The current process does not have access to the directory specified by path.
                                // Or the current process does not have sufficient privilege to set the ACL entry.
                                return false;
                            }
                            catch (PlatformNotSupportedException)
                            {
                                // The current operating system is not Windows 2000 or later.
                                return false;
                            }
                        }
                        return true;
                    }
                    catch
                    {
                        // There was an error changing the owner of the directory.
                        return false;
                    }
                }
                else
                {
                    // The directory does not exist.
                    return false;
                }
            }
            else
            {
                // A path, security object, and owner were not supplied.
                return false;
            }
        }

        /// <summary>
        /// Sets the owner of a directory.
        /// </summary>
        /// <param name="path">The path to the directory to have the ownership set on.</param>
        /// <param name="security">The DirectorySecurity object of the directory that will be changed.</param>
        /// <param name="owner">The directy entry that should take ownership of the entry.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this entry. Useful when combining multiple commands.</param>
        /// <returns>True if the ownership could be set. False otherwise.</returns>
        static public bool SetOwner(string path, ref DirectorySecurity security, DirectoryEntry owner, bool commitChanges)
        {
            // Check whether an owner was supplied.
            if (owner != null)
            {
                // An owner was supplied.

                // Get the value of the owner's security identifier (SID).
                byte[] sid = (byte[])owner.Properties["objectSid"].Value;

                // Set the owner.
                return SetOwner(path, ref security, sid, commitChanges);
            }
            else
            {
                // An owner was not supplied.
                return false;
            }
        }
    }
}
