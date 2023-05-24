namespace GoogleReCaptcha.Core.Services.Models
{
	using System.Text.Json.Serialization;

	/// <summary>
	/// Model of Google's reCAPTCHA verify request
	/// </summary>
	public class VerifyRequest
	{
		/// <summary>
		/// Required: The shared key between your site and reCAPTCHA
		/// </summary>
		public string Secret { get; set; } = null!;

        /// <summary>
        /// The user's response token provided by the reCAPTCHA client-side integration on your site
        /// </summary>
        public string Response { get; set; } = null!;

        /// <summary>
        /// Optional: The user's IP address
        /// </summary>
        [JsonPropertyName("remoteip")]
		public string RemoteIp { get; set; } = null!;
    }
}
