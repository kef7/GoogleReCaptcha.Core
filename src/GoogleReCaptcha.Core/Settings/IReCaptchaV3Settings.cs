namespace GoogleReCaptcha.Core.Settings
{
	public interface IReCaptchaV3Settings
	{
		bool Enabled { get; }
		string LibUrl { get; }
		string ApiUrl { get; }
		string SiteKey { get; }
		string SecretKey { get; }
		float DefaultPassingScore { get; }
	}
}
