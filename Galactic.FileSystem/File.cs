using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using SystemFile = System.IO.File;

namespace Galactic.FileSystem
{
    /// <summary>
    /// A utility class for manipulating files on the file system.
    /// </summary>
    public class File : IDisposable
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // A stream for manipulating the file.
        private readonly FileStream fileStream = null;

        // The path of the file.
        private readonly string path = null;

        // A reader for reading from the file.
        private StreamReader reader = null;

        // The file security object for this file.
        private FileSecurity security = null;

        // ----- PROPERTITES -----

        /// <summary>
        /// The path to the file on the file system.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }
        }

        /// <summary>
        /// Whether the file is opened as read-only.
        /// </summary>
        public bool ReadOnly { get; protected set; }

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
        public File(string path, bool overwrite, bool readOnly = false)
        {
            // Set whether the file is read-only.
            ReadOnly = readOnly;

            // Check that the path was supplied.
            if (!string.IsNullOrWhiteSpace(path))
            {
                // The path was supplied.
                this.path = path;

                // Use Create if a file doesn't exist at the path or overwrite was selected
                // and the file should not be opened as read-only.
                if (!Exists(Path) || overwrite && !ReadOnly)
                {
                    fileStream = Create(Path, true);
                }
                else
                {
                    // The file exists and we don't want to overwrite it. Open it.
                    fileStream = Open(Path, true, ReadOnly);
                }
            }
            else
            {
                // The path was not supplied. Throw an exception.
                throw new ArgumentNullException("path");
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Closes this file.
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Creates or overwrites a file at the supplied path location.
        /// </summary>
        /// <param name="path">The path to the location to create the file at.</param>
        /// <returns>A FileStream of the file created or null if an error occurred.</returns>
        static public FileStream Create(string path)
        {
            return Create(path, false);
        }

        /// <summary>
        /// Creates or overwrites a file at the supplied path location.
        /// </summary>
        /// <param name="path">The path to the location to create the file at.</param>
        /// <param name="throwExceptions">Whether to throw exceptions during file creation / opening.</param>
        /// <returns>A FileStream of the file created or null if an error occurred.</returns>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the caller does not have the required permissions to create or open the file,
        /// or the file is read-only.</exception>
        /// <exception cref="System.IO.PathTooLongException">Thrown if the specified path, file name, or both exceeds the system-defined maximum length.
        /// For example, on Windows-based patforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">Thrown if an I/O error occurred while creating the file.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the path is in an invalid format.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the path was not supplied.</exception>
        static private FileStream Create(string path, bool throwExceptions)
        {
            // Check that the path is supplied.
            if (!string.IsNullOrWhiteSpace(path))
            {
                // The path was supplied.
                try
                {
                    return SystemFile.Create(path);
                }
                catch (UnauthorizedAccessException)
                {
                    // The caller does not have the required permission.
                    // Or the path specified a file that is read-only.
                    if (throwExceptions)
                    {
                        throw new UnauthorizedAccessException("The caller does not have the required permission to open or create the file," +
                            "or the path specified a file that is read-only.");
                    }
                    return null;
                }
                catch (PathTooLongException)
                {
                    // The specified path, file name, or both exceeds the system-defined maximum length. For example,
                    // on Windows-based platforms, paths must be less than 248 characters, and file names must be less
                    // than 260 characters.
                    if (throwExceptions)
                    {
                        throw new PathTooLongException("The specified path, file name, or both exceeds the system-defined maximum length. For example," +
                            "on Windows-based platforms, paths must be less than 248 characters, and file names must be less" +
                            "than 260 characters.");
                    }
                    return null;
                }
                catch (DirectoryNotFoundException)
                {
                    // The specified path is invalid (for example, it is on an unmapped drive).
                    if (throwExceptions)
                    {
                        throw new DirectoryNotFoundException("The specified path is invalid (for example, it is on an unmapped drive).");
                    }
                    return null;
                }
                catch (IOException)
                {
                    // An I/O error occurred while creating the file.
                    if (throwExceptions)
                    {
                        throw new IOException("An I/O error occurred while creating the file.");
                    }
                    return null;
                }
                catch (NotSupportedException)
                {
                    // The path is in an invalid format.
                    if (throwExceptions)
                    {
                        throw new NotSupportedException("The path is in an invalid format.");
                    }
                    return null;
                }
            }
            else
            {
                // The path was not supplied. Throw an exception.
                if (throwExceptions)
                {
                    throw new ArgumentNullException("path");
                }
                return null;
            }
        }

        /// <summary>
        /// Deletes this file.
        /// </summary>
        /// <returns>True if the file was deleted, false otherwise.</returns>
        public bool Delete()
        {
            return Delete(Path);
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">The path to the file to delete.</param>
        /// <returns>True if the file was deleted, false otherwise.</returns>
        static public bool Delete(string path)
        {
            // Check that the path was provided.
            if (!string.IsNullOrWhiteSpace(path))
            {
                try
                {
                    SystemFile.Delete(path);
                    return !Exists(path);
                }
                catch (DirectoryNotFoundException)
                {
                    // The specified path is invalid (for example, it is on an unmapped drive).
                    return false;
                }
                catch (PathTooLongException)
                {
                    // The specified path, file name, or both exceed the system-defined maximum length. For example,
                    // on Windows-based platforms, paths must be less than 248 characters, and file names must be
                    // less than 260 characters.
                    return false;
                }
                catch (IOException)
                {
                    // The specified file is in use.
                    // Or there is an open handle on the file, and the operating system is Windows XP or earlier.
                    // This open handle can result from enumerating directories and files.
                    return false;
                }
                catch (NotSupportedException)
                {
                    // The path is in an invalid format.
                    return false;
                }
                catch (UnauthorizedAccessException)
                {
                    // The caller does not have the required permission.
                    // Or the path is a directory.
                    // Or the path specified is a read-only file.
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Frees the resources used by this file.
        /// </summary>
        public void Dispose()
        {
            if (fileStream != null)
            {
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// Checks whether this file exists on the file system.
        /// </summary>
        /// <returns>True if it exists, false otherwise.</returns>
        public bool Exists()
        {
            return Exists(Path);
        }

        /// <summary>
        /// Checks whether the file at the specified path exists on the file system.
        /// </summary>
        /// <param name="path">The path to the file to check.</param>
        /// <returns>True if it exists, false otherwise.</returns>
        static public bool Exists(string path)
        {
            // Check that the path was provided.
            if (!string.IsNullOrWhiteSpace(path))
            {
                return SystemFile.Exists(path);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Opens and gets a StreamReader for the file at the supplied path.
        /// </summary>
        /// <param name="path">The path to the file to get a StreamReader for.</param>
        /// <returns>A StreamReader for the file.</returns>
        static public StreamReader GetReader(string path)
        {
            // Check that the path is supplied.
            if (!string.IsNullOrWhiteSpace(path))
            {
                // The path was supplied.
                // Check that the file exists.
                if (SystemFile.Exists(path))
                {
                    // The file exists.
                    return new StreamReader(SystemFile.Open(path, FileMode.Open, FileAccess.Read));
                }
                else
                {
                    // The file does not exist. Throw an exception.
                    throw new FileNotFoundException("Can't get a reader for " + path + ". The file could not be found.");
                }
            }
            else
            {
                // The path was not supplied. Throw an exception.
                throw new ArgumentNullException("path");
            }
        }

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
                        return SystemFile.GetAccessControl(path);
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
        /// Moves this file to a new location.
        /// </summary>
        /// <param name="newPath">The destination path to move the file to.</param>
        /// <returns>True if the file was moved. False otherwise.</returns>
        public bool Move(string newPath)
        {
            return Move(Path, newPath);
        }

        /// <summary>
        /// Moves a file to a new location.
        /// </summary>
        /// <param name="path">The path of the file to move.</param>
        /// <param name="newPath">The destination path to move the file to.</param>
        /// <returns>True if the file was moved. False otherwise.</returns>
        static public bool Move(string path, string newPath)
        {
            if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(newPath))
            {
                // Determine if the path is a file.
                if (SystemFile.Exists(path))
                {
                    // It is a file.
                    try
                    {
                        FileInfo info = new FileInfo(path);
                        try
                        {
                            try
                            {
                                info.MoveTo(newPath + @"\" + info.Name);
                                return true;
                            }
                            catch (IOException)
                            {
                                // An I/O error occurs, such as the destination file already exists or the destination device is not ready.
                                return false;
                            }
                            catch (SecurityException)
                            {
                                // We don't have the permissions required to move the file.
                                return false;
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // The destination file name is read-only or is a directory.
                                return false;
                            }
                        }
                        catch (SecurityException)
                        {
                            // We don't have permissions to get information about the file's parent directory.
                            return false;
                        }
                    }
                    catch (SecurityException)
                    {
                        // We don't have the permissions required to get information about the file.
                        return false;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Access to the file was denied.
                        return false;
                    }
                }
            }
            // Null or empty paths were supplied or the file to move does not exist.
            return false;
        }

        /// <summary>
        /// Opens a file at the supplied path location.
        /// </summary>
        /// <param name="path">The path to the location to open the file at.</param>
        /// <param name="readOnly">Whether to open the file as read-only.</param>
        /// <returns>A FileStream of the file created or null if an error occurred.</returns>
        static public FileStream Open(string path, bool readOnly = false)
        {
            return Open(path, false, readOnly);
        }

        /// <summary>
        /// Opens a file at the supplied path location.
        /// </summary>
        /// <param name="path">The path to the location to open the file at.</param>
        /// <param name="readOnly">Whether to open the file as read-only.</param>
        /// <param name="throwExceptions">Whether to throw exceptions while opening.</param>
        /// <returns>A FileStream of the file created or null if an error occurred.</returns>
        /// <exception cref="System.UnauthorizedAccessException">Thrown if the caller does not have the required permissions to create or open the file,
        /// or the file is read-only.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Thrown if a file could not be found to open at the specified path.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown if the path was not supplied.</exception>
        static private FileStream Open(string path, bool throwExceptions, bool readOnly = false)
        {
            // Check that the path is supplied.
            if (!string.IsNullOrWhiteSpace(path))
            {
                // The path was supplied.

                // Check that the file exists.
                if (Exists(path))
                {
                    // The file exists.
                    try
                    {
                        if (readOnly)
                        {
                            return SystemFile.Open(path, FileMode.Open, FileAccess.Read);
                        }
                        else
                        {
                            return SystemFile.Open(path, FileMode.Open);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // The path specified a file that is read-only.
                        // Or the operation is not supported on the current platform.
                        // Or the caller does not have the required permission.
                        if (throwExceptions)
                        {
                            throw new UnauthorizedAccessException("The path specified a file that is read-only, or the operation is not supported" +
                                "on the current platform, or the caller does not have the required permission.");
                        }
                        return null;
                    }
                }
                else
                {
                    // The file does not exist.
                    if (throwExceptions)
                    {
                        throw new FileNotFoundException("A file could not be found to open at " + path + ".");
                    }
                    return null;
                }
            }
            else
            {
                // The path was not supplied. Throw an exception.
                if (throwExceptions)
                {
                    throw new ArgumentNullException("path");
                }
                return null;
            }
        }

        /// <summary>
        /// Reads the entire contents of this text file, and returns it as a string.
        /// </summary>
        /// <returns>The contents of this text file as a string, null if it could not be read or there
        /// was an error while reading.</returns>
        public string ReadAllAsText()
        {
            return ReadAllAsText(fileStream);
        }

        /// <summary>
        /// Reads the entire contents of a text file, and returns it as a string.
        /// </summary>
        /// <param name="path">The path to the file to read.</param>
        /// <returns>The contents of the text file as a string, null if it could not be read or there
        /// was an error while reading.</returns>
        public static string ReadAllAsText(string path)
        {
            // Check that a path is supplied.
            if (!string.IsNullOrEmpty(path))
            {
                // A path was supplied.

                // Check that the file exists.
                if (Exists(path))
                {
                    // The file exists.
                    // Read the contents of the file.
                    return ReadAllAsText(Open(path));
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
        /// Reads the entire contents of a text file, and returns it as a string.
        /// </summary>
        /// <param name="stream">The file stream to read from.</param>
        /// <returns>The text read, null if an error occurred while reading.</returns>
        public static string ReadAllAsText(FileStream stream)
        {
            // Check that the stream was supplied.
            if (stream != null)
            {
                // The stream was supplied.

                // Check that the stream supports reading.
                if (stream.CanRead)
                {
                    // The stream can be read.

                    // Get a reader for the file if one isn't supplied.
                    StreamReader reader = new StreamReader(stream);

                    try
                    {
                        return reader.ReadToEnd();
                    }
                    catch (OutOfMemoryException)
                    {
                        // There is insufficient memory to allocate a buffer for the returned string.
                        return null;
                    }
                    catch (IOException)
                    {
                        // An I/O error occurred.
                        return null;
                    }
                }
                else
                {
                    // The stream does not support reading.
                    return null;
                }
            }
            else
            {
                // The path was not supplied.
                return null;
            }
        }

        /// <summary>
        /// Reads a line of text from this file.
        /// </summary>
        /// <returns>The line of text read, null if an error occurred while reading.</returns>
        public string ReadLine()
        {
            if (reader == null)
            {
                reader = new StreamReader(fileStream);
            }
            return ReadLine(fileStream, ref reader);
        }

        /// <summary>
        /// Reads a line of text from a file's stream.
        /// </summary>
        /// <param name="stream">The file stream to read from.</param>
        /// <param name="reader">A stream reader used when reading from the file. Useful for successive calls to ReadLine.</param>
        /// <returns>The line of text read, null if an error occurred while reading.</returns>
        public static string ReadLine(FileStream stream, ref StreamReader reader)
        {
            // Check that the stream was supplied.
            if (stream != null)
            {
                // The stream was supplied.

                // Check that the stream supports reading.
                if (stream.CanRead)
                {
                    // The stream can be read.

                    // Get a reader for the file if one isn't supplied.
                    if (reader == null)
                    {
                        reader = new StreamReader(stream);
                    }

                    try
                    {
                        return reader.ReadLine();
                    }
                    catch (OutOfMemoryException)
                    {
                        // There is insufficient memory to allocate a buffer for the returned string.
                        return null;
                    }
                    catch (IOException)
                    {
                        // An I/O error occurred.
                        return null;
                    }
                }
                else
                {
                    // The stream does not support reading.
                    return null;
                }
            }
            else
            {
                // The path was not supplied.
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
                                SystemFile.SetAccessControl(path, security);
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

        /// <summary>
        /// Writes a line of text to the file.
        /// </summary>
        /// <param name="line">The line of text to write.</param>
        /// <returns>True if the line was written, false otherwise.</returns>
        public bool WriteLine(string line)
        {
            return WriteLine(fileStream, line);
        }

        /// <summary>
        /// Writes a line of text to the specified file stream.
        /// </summary>
        /// <param name="stream">The file stream to write to.</param>
        /// <param name="line">The line of text to write.</param>
        /// <returns>True if the line was written, false otherwise.</returns>
        public static bool WriteLine(FileStream stream, string line)
        {
            // Check that the file stream was supplied.
            if (stream != null)
            {
                // The stream was supplied.

                // Check that the stream is writeable.
                if (stream.CanWrite)
                {
                    // The stream is writeable.

                    // Create a writer for the file.
                    StreamWriter writer = new StreamWriter(stream);

                    // Write the line to the file.
                    try
                    {
                        writer.WriteLine(line);
                        return true;
                    }
                    catch (ObjectDisposedException)
                    {
                        // The writer is closed.
                        return false;
                    }
                    catch (IOException)
                    {
                        // An I/O error occurred.
                        return false;
                    }
                    finally
                    {
                        // Flush the writer.
                        writer.Flush();
                    }
                }
                else
                {
                    // The stream is not writeable.
                    return false;
                }
            }
            else
            {
                // The stream supplied was null.
                return false;
            }
        }
    }
}
