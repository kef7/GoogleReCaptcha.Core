namespace GoogleReCaptcha.Core.Services
{
	/// <summary>
	/// reCAPTCHA exception
	/// </summary>
    public class ReCaptchaServiceException : Exception
	{
		#region Fields

		/// <summary>
		/// Default exception message
		/// </summary>
		private const string DEFAULT_MESSAGE = "Error in attempt to use configured Google reCAPTCHA services from client.";

		#endregion

		#region Constuctors

		/// <summary>
		/// reCAPTCHA exception
		/// </summary>
		public ReCaptchaServiceException()
			: base(DEFAULT_MESSAGE)
		{
		}

        /// <summary>
        /// reCAPTCHA exception
        /// </summary>
        /// <param name="message">Exception message</param>
        public ReCaptchaServiceException(string message)
			: base(message)
		{
		}

        /// <summary>
        /// reCAPTCHA exception
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        public ReCaptchaServiceException(Exception innerException)
			: base(DEFAULT_MESSAGE, innerException)
		{
		}

        /// <summary>
        /// reCAPTCHA exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ReCaptchaServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		#endregion
	}
}
