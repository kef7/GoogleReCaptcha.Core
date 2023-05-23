namespace GoogleReCaptcha.Core.Settings
{
    public interface IReCaptchaSettings
	{
		bool Enabled { get; }
		string LibUrl { get; }
		string ApiUrl { get; }
		string SiteKey { get; }
		string SecretKey { get; }
	}
}
