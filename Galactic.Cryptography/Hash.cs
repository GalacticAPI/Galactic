using System;
using System.Security.Cryptography;
using System.Text;

namespace Galactic.Cryptography
{
    /// <summary>
    /// Hash is a class that generates and verifies hashes for cryptographic purposes.
    /// Note: The hashes provided in this class are not salted, and are really only
    /// intended for use in concert with other more secure encryption methods.
    /// </summary>
    public class Hash
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Generates a hash of the supplied input string using the the designated algorithm.
        /// </summary>
        /// <param name="input">String to create the hash of.</param>
        /// <param name="algorithmName">The name of the algorithm to use for the hash. Defaults to SHA3-256.</param>
        /// <returns>A hash of the string.</returns>
        static public string GetHash(string input, HashAlgorithmName algorithmName)
        {
            if (input != null)
            {
                // Generate a hash from the data using the supplied algorithm.
                HashAlgorithm hasher = null;
                if (algorithmName.Equals(HashAlgorithmName.MD5))
                {
                    hasher = MD5.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA1))
                {
                    hasher = SHA1.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA256))
                {
                    hasher = SHA256.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA384))
                {
                    hasher = SHA384.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA3_256))
                {
                    hasher = SHA3_256.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA3_384))
                {
                    hasher = SHA3_384.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA3_512))
                {
                    hasher = SHA3_512.Create();
                }
                else if (algorithmName.Equals(HashAlgorithmName.SHA512))
                {
                    hasher = SHA512.Create();
                }
                else
                {
                    // Default algorithm.
                    hasher = SHA3_256.Create();
                }
                

                // Convert the input string to a byte array and compute the hash.
                byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(input));

                // Create a new StringBuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string.
                foreach (byte b in data)
                {
                    sBuilder.Append(b.ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
            else
            {
                throw new ArgumentNullException(nameof(input));
            }
        }

        /// <summary>
        /// Verify a hash against a string.
        /// </summary>
        /// <param name="input">The input string to verify.</param>
        /// <param name="hash">The hash to verify against.</param>
        /// <returns>True if the input string matches the hash.</returns>
        static public bool VerifyHash(string input, string hash, HashAlgorithmName algorithmName)
        {
            if (input != null && hash != null)
            {
                // Hash the input.
                string hashOfInput = GetHash(input, algorithmName);

                // Create a StringComparer and compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (input == null)
                {
                    throw new ArgumentNullException(nameof(input));
                }
                else
                {
                    throw new ArgumentNullException(nameof(hash));
                }
            }
        }
    }
}
