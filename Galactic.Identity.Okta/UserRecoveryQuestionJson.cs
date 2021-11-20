using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Recovery Question data for JSON formatting.
    /// </summary>
    public record UserRecoveryQuestionJson
    {
        /// <summary>
        /// The answer component of the User's recovery question.
        /// </summary>
        [JsonPropertyName("answer")]
        public string Answer { get; init; } = default!;

        /// <summary>
        /// The question component of the User's recovery question.
        /// </summary>
        [JsonPropertyName("question")]
        public string Question { get; init; } = default!;
    }
}
