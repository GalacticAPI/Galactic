using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using SystemFile = System.IO.File;

namespace Galactic.FileSystem
{
    /// <summary>
    /// A utility class for manipulating files on the file system. Adds additional Windows-specific functionality.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class WindowsFile : File
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES ----

        // The file security object for this file.
        private FileSecurity security = null;

        // ----- PROPERTITES -----

        /// <summary>
        /// The FileSecurity object for this file.
        /// </summary>
        public FileSecurity Security
        {
            get
            {
                // Updates the security object with the latest information from the file.
                security = GetSecurityObject(Path);
                return security;
            }
            set
            {
                if (value != null)
                {
                    security = value;
                }
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Creates or opens a file at the supplied path location.
        /// </summary>
        /// <param name="path">The path to the location to create or open the file at.</param>
        /// <param name="overwrite">Whether to overwrite the contents of an existing file.</param>
        /// <param name="readOnly">Whether the file should be opened as read-only.</param>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the caller does not have the required permissions to create or open the file,
        /// or the file is read-only.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown if the specified path, file name, or both exceeds the system-defined maximum length.
        /// For example, on Windows-based patforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">Thrown if an I/O error occurred while creating the file.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if opening an existing file and it could not be found at the specified path.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the path is in an invalid format.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the path was not supplied.</exception>
        public WindowsFile(string path, bool overwrite, bool readOnly = false) : base(path, overwrite, readOnly)
        {
        }

        // ----- METHODS -----

        /// <summary>
        /// Gets the FileSecurity object for the file specified by the supplied path.
        /// </summary>
        /// <param name="path">The path to the file to retrieve the security object for.</param>
        /// <returns>The security object for the file specified. Null if the file does not exist,
        /// an I/O error occurred, or the process does not have the permissions required to
        /// complete the operation.</returns>
        static public FileSecurity GetSecurityObject(string path)
        {
            // Check that a path is supplied.
            if (!string.IsNullOrEmpty(path))
            {
                // A path is supplied.

                // Check whether the file exits.
                if (SystemFile.Exists(path))
                {
                    // The file exists.
                    try
                    {
                        return FileSystemAclExtensions.GetAccessControl(new FileInfo(path));
                    }
                    catch (IOException)
                    {
                        // An I/O error occurred while opening the file.
                        return null;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // The path parameter specified a file that is read-only.
                        // Or this operation is not supported on the current platform.
                        // Or the caller does not have the required permission.
                        return null;
                    }
                }
                else
                {
                    // The file does not exist.
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
        /// Removes all explicit access rules from this file.
        /// </summary>
        /// <param name="commitChanges">Indicates whether changes should be commited to this file. Useful when combining multiple commands.</param>
        /// <returns>True if access was removed. False otherwise.</returns>
        public bool RemoveAllExplicitAccessRules(bool commitChanges)
        {
            return RemoveAllExplicitAccessRules(Path, out security, commitChanges);
        }

        /// <summary>
        /// Removes all explicit access rules from the supplied file.
        /// </summary>
        /// <param name="path">The path to the file to have access removed on.</param>
        /// <param name="security">The FileSecurity object of the file once changed.</param>
        /// <param name="commitChanges">Indicates whether changes should be commited to this file. Useful when combining multiple commands.</param>
        /// <returns>True if access was removed. False otherwise.</returns>
        static public bool RemoveAllExplicitAccessRules(string path, out FileSecurity security, bool commitChanges)
        {
            // Check that a path was supplied.
            if (!string.IsNullOrEmpty(path))
            {
                // The path was supplied.

                // Check whether the file exists.
                if (SystemFile.Exists(path))
                {
                    // The file exists.

                    // Remove existing explicit permissions.
                    security = GetSecurityObject(path);
                    if (security != null)
                    {
                        AuthorizationRuleCollection rules = security.GetAccessRules(true, false, typeof(System.Security.Principal.SecurityIdentifier));
                        foreach (AuthorizationRule rule in rules)
                        {
                            security.RemoveAccessRule((FileSystemAccessRule)rule);
                        }
                        // Commit the changes if necessary.
                        if (commitChanges)
                        {
                            try
                            {
                                FileSystemAclExtensions.SetAccessControl(new FileInfo(path), security);
                            }
                            catch (IOException)
                            {
                                // An I/O error occurred while opening the file.
                                return false;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // The path parameter specified a file that is read-only.
                                // The operation is not supported on the current platform.
                                // Or the current process does not have the required permission.
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        // Unable to get the file's security object.
                        return false;
                    }
                }
                else
                {
                    // The file does not exist.
                    security = null;
                    return false;
                }
            }
            else
            {
                // A path was not supplied.
                security = null;
                return false;
            }
        }
    }
}
