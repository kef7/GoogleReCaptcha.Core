namespace GoogleReCaptcha.Core.Services
{
	/// <summary>
	/// reCAPTCHA service interface
	/// </summary>
    public interface IReCaptchaService
	{
		/// <summary>
		/// Get token
		/// </summary>
		/// <returns>Token</returns>
		string GetToken();

		/// <summary>
		/// Verify reCAPTCHA
		/// </summary>
		/// <returns>true if successfully verified; false otherwise</returns>
		bool Verify();

        /// <summary>
        /// Verify reCAPTCHA async
        /// </summary>
        /// <returns>true if successfully verified; false otherwise</returns>
        Task<bool> VerifyAsync();
	}
}
