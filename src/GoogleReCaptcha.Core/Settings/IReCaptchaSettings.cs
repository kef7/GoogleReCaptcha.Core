namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA settings interface
    /// </summary>
    public interface IReCaptchaSettings
	{
		/// <summary>
		/// Enabled flag
		/// </summary>
		bool Enabled { get; }

        /// <summary>
        /// reCAPTCHA script URL
        /// </summary>
        string LibUrl { get; }

        /// <summary>
        /// reCAPTCHA API script URL
        /// </summary>
        string ApiUrl { get; }

        /// <summary>
        /// reCAPTCHA site key
        /// </summary>
		string SiteKey { get; }

        /// <summary>
        /// reCAPTCHA secret key
        /// </summary>
		string SecretKey { get; }
	}
}
