using Galactic.Cryptography;
using Galactic.EventLog;
using System;
using System.IO;
using Directory = Galactic.FileSystem.Directory;
using File = Galactic.FileSystem.File;

namespace Galactic.Configuration
{
    /// <summary>
    /// Manages files containing strings used as an alternative to placing configuration information
    /// in an application's web.config or application.config files.
    /// This approaches allows for easier management of these items as they don't require a restart of the
    /// application to manipulate and save the configuration information.
    /// The strings can be (optionally) encrypted.
    /// </summary>
    public class ConfigurationItem
    {
        // ----- CONSTANTS -----

        // The file extension for configuration items.
        private const string FILE_EXTENSION = ".config";

        // ----- VARIABLES -----

        // The path to the folder containing the configuration item.
        private string folderPath = "";

        // The name of the configuration item.
        private string name = "";

        // The event log to use for logging errors (if desired).
        private readonly EventLog.EventLog eventLog = null;

        // Whether the configuration file is opened in read-only mode.
        private readonly bool readOnly = true;

        // ----- PROPERTIES -----

        /// <summary>
        /// The full path to the file containing the configuration item.
        /// </summary>
        public string FilePath
        {
            get
            {
                return FolderPath + Name + FILE_EXTENSION;
            }
        }

        /// <summary>
        /// The path to the folder containing the configuration item.
        /// </summary>
        public string FolderPath
        {
            get
            {
                return folderPath;
            }
            set
            {
                if (Directory.Exists(value))
                {
                    folderPath = value;

                    // Check if the path ends with a backslash.
                    if (!folderPath.EndsWith(@"\"))
                    {
                        // The path does not end with a backslash. Tack one on.
                        folderPath = folderPath + @"\";
                    }
                }
                else
                {
                    throw new System.IO.DirectoryNotFoundException();
                }
            }
        }

        /// <summary>
        /// The name of the configuration item.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    name = value;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// Whether the configuration item should be encrypted.
        /// </summary>
        public Boolean Encrypted { get; set; }

        /// <summary>
        /// The value of the configuration item.
        /// </summary>
        public String Value
        {
            get
            {
                // Reads the value from its configuration file.
                // Check whether the value to retrieve is encrypted.
                if (Encrypted)
                {
                    return SecureGet(eventLog);
                }
                else
                {
                    return Get(eventLog);
                }
            }
            set
            {
                // Saves the value supplied to a configuration file.

                // If the value is null set it to an empty string.
                if (value == null)
                {
                    value = "";
                }

                // Check whether to save the value in an encrypted format.
                if (Encrypted)
                {
                    SecureWrite(value, eventLog);
                }
                else
                {
                    Write(value, eventLog);
                }
            }
        }

        // ----- CONSTRUCTORS -----

        /// <summary>
        /// Loads a configuration item with the specified name from its file located
        /// in a folder at the supplied path.
        /// </summary>
        /// <param name="folderPath">The path of the folder containing the configuration item.</param>
        /// <param name="name">The name of the configuration item.</param>
        /// <param name="encrypted">Whether to store the item in an encrypted format.</param>
        /// <param name="value">(Optional) The value of the configuration item. (Supplying a null value will retrieve the value stored
        /// in the item if it already exists).</param>
        /// <param name="log">(Optional) The event log to log exceptions to. May be null for no logging.</param>
        /// <param name="readOnly">Whether the configuration item should be opened for reading only. Default: true</param>
        public ConfigurationItem(String folderPath, String name, Boolean encrypted, String value = null, EventLog.EventLog log = null, Boolean readOnly = true)
        {
            if (!String.IsNullOrWhiteSpace(folderPath) && !String.IsNullOrWhiteSpace(name))
            {
                FolderPath = folderPath;
                Name = name;
                Encrypted = encrypted;
                eventLog = log;
                this.readOnly = readOnly;

                // If the value is not null, set the value.
                if (value != null)
                {
                    // Initialize the value.
                    Value = value;
                }
            }
        }

        // ----- METHODS -----

        /// <summary>
        /// Creates a new configuration file that can then be loaded normally.
        /// Note: This will NOT overwrite an existing file.
        /// </summary>
        /// <param name="folderPath">The path of the folder to create the configuration item.</param>
        /// <param name="name">The name of the configuration item to create.</param>
        /// <returns>True if the file was created or already exists, false if an error prevented it from being created.</returns>
        public static bool Create(string folderPath, string name)
        {
            string fullPath = folderPath + name + FILE_EXTENSION;
            if (!File.Exists(fullPath))
            {
                // Create the file.
                try
                {
                    // Create and close the file.
                    File file = new File(fullPath, false);
                    file.Close();
                    return true;
                }
                catch
                {
                    // There was an error creating the file.
                    return false;
                }
            }
            else
            {
                // The file already exists.
                return true;
            }
        }

        /// <summary>
        /// Deletes the configuration item.
        /// </summary>
        /// <returns>True if the item was deleted, false otherwise.</returns>
        public bool Delete()
        {
            // Check if the file exists.
            if (File.Exists(FilePath))
            {
                // The file exists.
                // Delete the file.
                return File.Delete(FilePath);
            }
            else
            {
                // The file doesn't exist.
                return true;
            }
        }

        /// <summary>
        /// Retrieves the value of a configuration item from its file.
        /// </summary>
        /// <param name="log">The event log to log exceptions to. May be null for no logging.</param>
        /// <returns>The value of the configuration item or null if not found.</returns>
        protected string Get(EventLog.EventLog log)
        {
            File file = null;
            try
            {
                // Get the file.
                file = new File(FilePath, false, readOnly);

                // Check that the file exists.
                if (file.Exists())
                {
                    // Return the value of the item from the file.
                    return file.ReadAllAsText();
                }
                else
                {
                    // The file doesn't exist. Return null.
                    return null;
                }
            }
            catch (Exception e)
            {
                // There was an error opening or reading from the file.
                LogException(e, log);
                return null;
            }
            finally
            {
                // Close the file.
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// Logs an exception to the event log.
        /// </summary>
        /// <param name="e">The exception to log.</param>
        /// <param name="log">The event log to log the exception to.</param>
        /// <returns>True if the exception was logged successfully. False otherwise.</returns>
        private static bool LogException(Exception e, IEventLog log)
        {
            if (log != null)
            {
                log.Log(new Event(typeof(ConfigurationItem).FullName, DateTime.Now, Event.SeverityLevels.Error, e.GetType().FullName,
                    "Description:\n" +
                    e.Message + "\n" +
                    "Stack Trace:\n" +
                    e.StackTrace));
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes a configuration item's value to a file.
        /// </summary>
        /// <param name="value">The value to write to the file.</param>
        /// <param name="log">The event log to log exceptions to. May be null for no logging.</param>
        /// <returns>True if the write was successful, false otherwise.</returns>
        protected bool Write(string value, EventLog.EventLog log)
        {
            // Write the value to the file.
            File file = null;
            try
            {
                file = new File(FilePath, false);
                return file.WriteLine(value);
            }
            catch (Exception e)
            {
                // There was an error opening the file.
                LogException(e, log);
                return false;
            }
            finally
            {
                // Close the file.
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        /// <summary>
        /// Retrieves an AES 256 encrypted value from the configuration file.
        /// </summary>
        /// <param name="log">The event log to log exceptions to. May be null for no logging.</param>
        /// <returns>The value or null if not found or it could not be decrypted.</returns>
        protected string SecureGet(EventLog.EventLog log)
        {
            // Get the raw value of the encrypted string.
            string value = Get(log);

            // Decrypt the string if the raw value contains one.
            if (!string.IsNullOrWhiteSpace(value))
            {
                // Get the encrypted string containing the key, initialization vector, value, and key and
                // initialization vector lengths from the file.
                // Read only the first line of the file, as this is all that is necessary for the encrypted
                // format.
                StringReader reader = new StringReader(Get(log));
                string encryptedValue = reader.ReadLine();

                // Check that an encrypted value was retrieved.
                if (!string.IsNullOrWhiteSpace(encryptedValue))
                {
                    // The encrypted value was retrieved.

                    // Decrypt the encrypted value.
                    return AES256.DecryptConsolidatedString(encryptedValue);
                }
            }
            // Could not retrieve the encrypted value.
            return null;
        }

        /// <summary>
        /// Writes an AES 256 encrypted value to its configuration file.
        /// </summary>
        /// <param name="value">The value to write to the file.</param>
        /// <param name="log">The event log to log execptions to. May be null for no logging.</param>
        /// <returns>True if the write was successful, false otherwise.</returns>
        protected bool SecureWrite(string value, EventLog.EventLog log)
        {
            // Create a consolidated encrypted string value for storage in the file.
            string consolidatedString = AES256.CreateConsolidatedString(value);

            // Check that the consolidated string value was created successfully.
            if (!string.IsNullOrWhiteSpace(consolidatedString))
            {
                // The consolidated string value was created successfully.

                // Write the consolidated string value containing the encrypted value to a file.
                return Write(consolidatedString, log);
            }
            else
            {
                // The consolidated string value was not created.
                return false;
            }
        }
    }
}
