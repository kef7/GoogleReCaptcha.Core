using System.Text.Json.Serialization;

namespace GoogleReCaptcha3.Core.Services.Models
{
	/// <summary>
	/// Model of Google's ReCaptcha verify request
	/// </summary>
	public class VerifyRequest
	{
		/// <summary>
		/// Required: The shared key between your site and ReCaptcha
		/// </summary>
		public string Secret { get; set; }

		/// <summary>
		/// The user's reponse token provided by the ReCaptcha client-side integration on your site
		/// </summary>
		public string Response { get; set; }

		/// <summary>
		/// Optional: The user's IP address
		/// </summary>
		[JsonPropertyName("remoteip")]
		public string RemoteIp { get; set; }
	}
}
