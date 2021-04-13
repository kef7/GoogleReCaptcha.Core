namespace GoogleReCaptcha.Core.Settings
{
	public class ReCaptchaV3Settings : IReCaptchaV3Settings
	{
		public bool Enabled { get; set; }
		public string LibUrl { get; set; }
		public string ApiUrl { get; set; }
		public string SiteKey { get; set; }
		public string SecretKey { get; set; }
	}
}
