using System;
using System.Text.Json.Serialization;

namespace Galactic.Identity.Okta
{
    /// <summary>
    /// A record representing Okta User Recovery Question data for JSON formatting.
    /// </summary>
    public record UserRecoveryQuestionJson
    {
        // ----- CONSTANTS -----

        /// <summary>
        /// Answer JSON property name.
        /// </summary>
        public const string ANSWER = "answer";

        /// <summary>
        /// Question JSON property name.
        /// </summary>
        public const string QUESTION = "question";

        // ----- PROPERTIES -----

        /// <summary>
        /// The answer component of the User's recovery question.
        /// </summary>
        [JsonPropertyName(ANSWER)]
        public string Answer { get; init; } = default!;

        /// <summary>
        /// The question component of the User's recovery question.
        /// </summary>
        [JsonPropertyName(QUESTION)]
        public string Question { get; init; } = default!;
    }
}
