using System;
using System.IO;
using SystemDirectory = System.IO.Directory;
using SystemFile = System.IO.File;

namespace Galactic.FileSystem
{
    /// <summary>
    /// A utility class for manipulating directories on the file system.
    /// </summary>
    public static class Directory
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTITES -----

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Clones a directory to a new location. Does not copy any files or folders beneath it.
        /// </summary>
        /// <param name="path">The path of the directory to clone.</param>
        /// <param name="newPath">The destination path to clone the directory to.</param>
        /// <returns>True if the directory was cloned. False otherwise.</returns>
        static public bool Clone(string path, string newPath)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(newPath))
            {
                // Determine if the path is a directory.
                if (SystemDirectory.Exists(path))
                {
                    // It is a directory.
                    try
                    {
                        SystemDirectory.CreateDirectory(newPath);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // We do not have the required permissions to create the directory.
                        return false;
                    }
                }
            }
            // The path or newPath were null, or the path to clone did not exist.
            return false;
        }

        /// <summary>
        /// Copies a directory to a new location including all files and folders beneath it.
        /// Creates a new directory if necessary.
        /// </summary>
        /// <param name="path">The path of the directory to copy.</param>
        /// <param name="newPath">The destination path to copy the directory to.</param>
        /// <param name="overwrite">Whether to overwrite any existing files in the directory being copied to.</param>
        /// <returns>True if the directory was copied. False otherwise.</returns>
        static public bool Copy(string path, string newPath, bool overwrite = false)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(newPath))
            {
                // Ensure that the paths end in a slash.
                if (!path.EndsWith(@"\"))
                {
                    // Add the slash.
                    path += @"\";
                }

                if (!newPath.EndsWith(@"\"))
                {
                    // Add the slash.
                    newPath += @"\";
                }

                // Determine if the path is a directory.
                if (SystemDirectory.Exists(path))
                {
                    // It is a directory.
                    try
                    {
                        // Check whether the directory to copy to exists.
                        if (!SystemDirectory.Exists(newPath))
                        {
                            // Clone a new directory to copy the directory into.
                            // This ensures we get the same permissions.
                            if (!Clone(path, newPath))
                            {
                                // The directory couldn't be cloned.
                                return false;
                            }
                        }

                        // Copy the contents of subdirectories.
                        foreach (string directoryName in SystemDirectory.EnumerateDirectories(path))
                        {
                            // Get the name of the subdirectory.
                            string subDirectoryName = directoryName.Substring(path.Length);

                            // Recursively copy the directory.
                            if (!Copy(directoryName, newPath + subDirectoryName + @"\", overwrite))
                            {
                                // The directory couldn't be copied.
                                return false;
                            }
                        }

                        // Copy any files in the directory.
                        foreach (string fileName in SystemDirectory.EnumerateFiles(path))
                        {
                            FileInfo originalFileInfo = new FileInfo(fileName);
                            if (!File.Copy(fileName, newPath, overwrite))
                            {
                                // The file couldn't be copied.
                                return false;
                            }
                        }

                        return true;
                    }
                    catch
                    {
                        // There was an error copying the directory or its contents.
                        return false;
                    }
                }
            }
            // The path or newPath were null, or the path to clone did not exist.
            return false;
        }

        /// <summary>
        /// Creates a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to create.</param>
        /// <returns>True if the directory was created or already exists.
        /// False if an error occured and the directory could not be created.</returns>
        static public bool Create(string path)
        {
            // Check whether the path supplied is valid.
            if (!string.IsNullOrEmpty(path))
            {
                // Check whether the path for the directory exits.
                if (!SystemDirectory.Exists(path))
                {
                    // It does not exist, create it.
                    try
                    {
                        SystemDirectory.CreateDirectory(path);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // We do not have the required permissions to create the directory.
                        return false;
                    }
                    catch
                    {
                        // An error occured.
                        return false;
                    }
                }
                // The directory already exists.
                return true;
            }
            // The path is not valid.
            return false;
        }

        /// <summary>
        /// Deletes a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to delete.</param>
        /// <param name="recursive">Recursively delete the files an subfolders of the directory.</param>
        /// <returns>True if the directory was deleted or did not exist, false if an error occured
        /// and the directory could not be deleted.</returns>
        static public bool Delete(string path, bool recursive)
        {
            // Check whether the path supplied is valid.
            if (!string.IsNullOrEmpty(path))
            {
                // Check whether the path for the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    // It exists, delete it.
                    try
                    {
                        SystemDirectory.Delete(path, recursive);
                        return true;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // We do not have the required permissions to delete the directory.
                        return false;
                    }
                    catch
                    {
                        // An error occurred.
                        return false;
                    }
                }
                else
                {
                    // The directory doesn't exists.
                    return true;
                }
            }
            // The path is not valid.
            return false;
        }

        /// <summary>
        /// Checks whether the directory at the specified path exists on the file system.
        /// </summary>
        /// <param name="path">The path to the directory to check.</param>
        /// <returns>True if it exists, false otherwise.</returns>
        static public bool Exists(string path)
        {
            // Check that the path was provided.
            if (!string.IsNullOrWhiteSpace(path))
            {
                return SystemDirectory.Exists(path);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a list of the contents of a directory at the specified path.
        /// </summary>
        /// <param name="path">The path of the directory to get a list of the contents of.</param>
        /// <param name="getSubDirs">Whether to obtain a list of the contents of all subdirectories as well.</param>
        /// <returns>An array of strings with the names of directories and files under the supplied path.
        /// Null if the path is invalid, does not exist, or the current process does not have permission
        /// to get the directory listing.</returns>
        static public string[] GetListing(string path, bool getSubDirs)
        {
            // Check whether the path supplied is valid.
            if (!string.IsNullOrEmpty(path))
            {
                // Check whether the path for the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.

                    // Check if we need to get subdirectory content as well.
                    if (!getSubDirs)
                    {
                        // We don't need to get subdirectory content.
                        try
                        {
                            return SystemDirectory.GetFileSystemEntries(path);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // We don't have the required permission.
                            return null;
                        }
                    }
                    else
                    {
                        // We need to get subdirectory content as well.
                        return SystemDirectory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories);
                    }
                }
            }
            // The path is not valid.
            return null;
        }

        /// <summary>
        /// Gets the size of a directory's contents in bytes.
        /// </summary>
        /// <param name="path">The path to the directory.</param>
        /// <returns>The size of the directory's contents in bytes, or a negative file size if there was error retrieving this information.</returns>
        static public long GetSizeInBytes(string path)
        {
            // Check whether the path supplied is valid.
            if (!string.IsNullOrEmpty(path))
            {
                // Check whether the path for the directory exits.
                if (SystemDirectory.Exists(path))
                {
                    // The directory exists.
                    long totalSize = 0;

                    string[] directoryContents = GetListing(path, true);
                    foreach (string itemPath in directoryContents)
                    {
                        // Check whether the item is a file.
                        if (File.Exists(itemPath))
                        {
                            // The item is a file.

                            // Get it's size and add it to the total size.
                            long fileSize = File.GetSizeInBytes(itemPath);
                            if (fileSize >= 0)
                            {
                                totalSize += fileSize;
                            }
                        }
                        else if (Exists(itemPath))
                        {
                            // The item is a directory.

                            // Get the size of the subdirectory and add it to the total.
                            totalSize += GetSizeInBytes(itemPath);
                        }
                    }

                    // Return the total size found.
                    return totalSize;
                }
            }
            // The path is not valid.
            return -1;
        }

        /// <summary>
        /// Moves a directory to a new location.
        /// </summary>
        /// <param name="path">The path of the directory to move.</param>
        /// <param name="newPath">The path to the new location to move the directory.</param>
        /// <returns>True if the directory was moved. False otherwise.</returns>
        static public bool Move(string path, string newPath)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(newPath))
            {
                // Determine if the path is a directory.
                if (SystemDirectory.Exists(path))
                {
                    // It is a directory.
                    // Clone the root directory.
                    if (!Clone(path, newPath))
                    {
                        // There was an error cloning the root directory.
                        return false;
                    }

                    // Get the contents of the directory and move them.
                    string[] dirContent = GetListing(path, true);
                    foreach (string name in dirContent)
                    {
                        // Determine if the name is a directory.
                        if (SystemDirectory.Exists(name))
                        {
                            if (!Move(name, newPath + name.Replace(path, "")))
                            {
                                // There was an error moving the directory.
                                return false;
                            }
                        }
                        // Determine if the name is a file.
                        else if (SystemFile.Exists(name))
                        {
                            if (!File.Move(name, newPath))
                            {
                                // There was an error moving the file.
                                return false;
                            }
                        }
                    }
                    // Delete the remaining directories.
                    try
                    {
                        SystemDirectory.Delete(path, true);
                        return true;
                    }
                    catch (IOException)
                    {
                        // A file with the same name and location specified by path exists.
                        // The directory specified by path is read-only or recursive is set to false
                        // and the directory is not empty.
                        // The directory is the application's current working directory.
                        // The directory continas a read-only file.
                        // The directory is being used by another process.
                        return false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // We don't have the required permission to delete the directory.
                        return false;
                    }
                }
            }
            // The path or newPath were empty or null or the path did not exist,
            // or there was an error moving or deleting the directory.
            return false;
        }
    }
}
