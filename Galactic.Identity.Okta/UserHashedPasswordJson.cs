using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Hashed Password data for JSON formatting.
    /// </summary>
    public record UserHashedPasswordJson
    {
        /// <summary>
        /// The algorithm used to generate the hash using the password (and salt,
        /// when applicable). Must be set to BCRYPT, SHA-512, SHA-256, SHA-1 or
        /// MD5.
        /// </summary>
        [JsonPropertyName("algorithm")]
        public string Algorithm { get; init; } = default!;

        /// <summary>
        /// For SHA-512, SHA-256, SHA-1, MD5, This is the actual base64-encoded
        /// hash of the password (and salt, if used). This is the Base64 encoded
        /// value of the SHA-512/SHA-256/SHA-1/MD5 digest that was computed by
        /// either pre-fixing or post-fixing the salt to the password, depending
        /// on the saltOrder. If a salt was not used in the source system, then
        /// this should just be the the Base64 encoded value of the password's
        /// SHA-512/SHA-256/SHA-1/MD5 digest. For BCRYPT, This is the actual
        /// radix64-encoded hashed password.
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; init; } = default!;

        /// <summary>
        /// Only required for salted hashes. For BCRYPT, this specifies the
        /// radix64-encoded salt used to generate the hash, which must be 22
        /// characters long. For other salted hashes, this specifies the
        /// base64-encoded salt used to generate the hash.
        /// </summary>
        [JsonPropertyName("salt")]
        public string Salt { get; init; } = default!;

        /// <summary>
        /// Specifies whether salt was pre- or postfixed to the password before
        /// hashing. Only required for salted algorithms.
        /// </summary>
        [JsonPropertyName("saltOrder")]
        public string SaltOrder { get; init; } = default!;

        /// <summary>
        /// Governs the strength of the hash and the time required to compute it.
        /// Only required for BCRYPT algorithm. Minimum value is 1, and maximum
        /// is 20.
        /// </summary>
        [JsonPropertyName("workFactor")]
        public int? WorkFactor { get; init; } = default!;
    }
}
