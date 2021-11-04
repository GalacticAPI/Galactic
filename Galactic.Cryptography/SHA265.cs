using System;
using System.Text;

namespace Galactic.Cryptography
{
    /// <summary>
    /// Handles getting and verifying SHA256 hashes.
    /// Based on Microsoft's example code.
    /// </summary>
    public class SHA256
    {
        // ----- CONSTANTS -----

        // ----- VARIABLES -----

        // ----- PROPERTIES -----

        // ----- CONSTRUCTORS -----

        // ----- METHODS -----

        /// <summary>
        /// Hash in input string and return the hash as a 32 character hexadecimal string.
        /// </summary>
        /// <param name="input">String to create the hash of.</param>
        /// <returns>The hash of the string.</returns>
        static public string GetHash(string input)
        {
            // Creates a new instance of the SHA256CryptoServiceProvider object.
            System.Security.Cryptography.SHA256 shaHasher = System.Security.Cryptography.SHA256.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new StringBuilder to collect the bytes and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string.
            foreach (byte b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        /// <summary>
        /// Verify a hash against a string.
        /// </summary>
        /// <param name="input">The input string to verify.</param>
        /// <param name="hash">The hash to verify against.</param>
        /// <returns>True if the input string matches the hash.</returns>
        static public bool VerifyHash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetHash(input);

            // Create a StringComparer and compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            return false;
        }
    }
}
