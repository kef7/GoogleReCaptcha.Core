﻿namespace GoogleReCaptcha.Core.Services.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model of Google's reCAPTCHA verify response
    /// </summary>
    public class VerifyResponse
    {
        /// <summary>
        /// Whether the request was a valid reCAPTCHA token
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The score for the request (0.0 - 1.0)
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// The action name for the request
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Challenge timestamp
        /// </summary>
        [JsonPropertyName("challenge_ts")]
        public DateTime? ChallengeTs { get; set; }

        /// <summary>
        /// Hostname of the site where the reCAPTCHA was solved
        /// </summary>
        public string? Hostname { get; set; }

        /// <summary>
        /// Android APK package name where the reCAPTCHA was solved; optional or N/A unless in APK (Xamarin/MAUI maybe)
        /// </summary>
        [JsonPropertyName("apk_package_name")]
        public string? ApkPackageName { get; set; }

        /// <summary>
        /// List of error codes during verify process; optional
        /// </summary>
        [JsonPropertyName("error-codes")]
        public string?[]? ErrorCodes { get; set; } = null;
    }
}
