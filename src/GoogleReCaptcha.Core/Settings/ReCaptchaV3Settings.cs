namespace GoogleReCaptcha.Core.Settings
{
	public class ReCaptchaV3Settings : IReCaptchaV3Settings
	{
		public bool Enabled { get; set; } = true;
		public string LibUrl { get; set; }
		public string ApiUrl { get; set; }
		public string SiteKey { get; set; }
		public string SecretKey { get; set; }
		public float DefaultPassingScore { get; set; } = Constants.DEFAULT_V3_PASSING_SCORE;
	}
}
