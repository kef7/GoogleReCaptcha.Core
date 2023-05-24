namespace GoogleReCaptcha.Core.Settings
{
    /// <summary>
    /// reCAPTCHA settings
    /// </summary>
    public class ReCaptchaSettings : IReCaptchaSettings
	{
		/// <inheritdoc />
		public bool Enabled { get; set; } = true;

        /// <inheritdoc />
        public string LibUrl { get; set; }

        /// <inheritdoc />
        public string ApiUrl { get; set; }

        /// <inheritdoc />
        public string SiteKey { get; set; }

        /// <inheritdoc />
        public string SecretKey { get; set; }
	}
}
