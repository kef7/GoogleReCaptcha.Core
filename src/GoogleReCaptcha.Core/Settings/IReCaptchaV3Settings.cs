namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA v3 settings interface
    /// </summary>
    public interface IReCaptchaV3Settings : IReCaptchaSettings
	{
        /// <summary>
        /// reCAPTCHA v3 default passing score value
        /// </summary>
		float DefaultPassingScore { get; }
	}
}
