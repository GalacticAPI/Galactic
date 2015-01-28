using System;
using System.Security.Cryptography;

namespace Galactic.Cryptography
{
    /// <summary>
    /// SaltedHash is a class that generates and verifies salted
    /// hashes for cryptographic purposes.
    /// </summary>
    public class SaltedHash
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Generates a random salt using cryptographically secure algorithms.
        /// </summary>
        /// <returns>A random integer.</returns>
        static public int GenerateSalt()
        {
            // Generate a random number as the salt for secure data.
            RNGCryptoServiceProvider randGen = new RNGCryptoServiceProvider();
            byte[] randArray = new byte[4];
            randGen.GetBytes(randArray);
            return BitConverter.ToInt32(randArray, 0);
        }

        /// <summary>
        /// Generates a salted hash using the supplied string data.
        /// A random integer is generated as the salt.
        /// </summary>
        /// <param name="data">The data to generate a hash off.</param>
        /// <param name="salt">A random integer used as a salt.</param>
        /// <returns></returns>
        static public string GenerateHash(string data, out int salt)
        {
            salt = GenerateSalt();

            // Generate a salted hash from the data and the salt.
            string saltedData = salt + data;
            return SHA256.GetHash(saltedData);
        }

        /// <summary>
        /// Verifies that the supplied data string matches the value in the supplied hash.
        /// </summary>
        /// <param name="data">The string of data to check against the hash.</param>
        /// <param name="salt">The salt for this data.</param>
        /// <param name="hash">The hash to compare against.</param>
        /// <returns>True if the data matches, false otherwise.</returns>
        static public bool VerifyHash(string data, int salt, string hash)
        {
            // Check that the arguments passed are valid.
            if (!string.IsNullOrWhiteSpace(data) && !string.IsNullOrWhiteSpace(hash))
            {
                // Verify the hash.
                return SHA256.VerifyHash(salt + data, hash);
            }
            // The arguments were not valid.
            return false;
        }
    }
}
