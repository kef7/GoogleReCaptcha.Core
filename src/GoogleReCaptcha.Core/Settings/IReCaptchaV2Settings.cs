namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA v2 settings interface
    /// </summary>
    public interface IReCaptchaV2Settings : IReCaptchaSettings
	{
        /// <summary>
        /// reCAPTCHA v2 theme
        /// </summary>
		V2Theme? Theme { get; }

        /// <summary>
        /// reCAPTCHA v2 size
        /// </summary>
		V2Size? Size { get; }
	}
}
