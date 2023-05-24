namespace GoogleReCaptcha.Core.Services
{
    using GoogleReCaptcha.Core.Services.Models;

    /// <summary>
    /// reCAPTCHA verify exception
    /// </summary>
    public class ReCaptchaVerifyException : Exception
    {
        #region Fields

        /// <summary>
        /// Default exception message
        /// </summary>
        private const string DEFAULT_MESSAGE = "Google reCAPTCHA errored or the response contains errors.";

        #endregion

        #region Properties

        /// <summary>
        /// Verify response from reCAPTCHA
        /// </summary>
        public VerifyResponse Response { get; private set; } = null;

        #endregion

        #region Constuctors

        /// <summary>
        /// reCAPTCHA verify exception
        /// </summary>
        public ReCaptchaVerifyException()
            : base(DEFAULT_MESSAGE)
        {
        }

        /// <summary>
        /// reCAPTCHA verify exception
        /// </summary>
        /// <param name="response">The reCAPTCHA verify response object</param>
        public ReCaptchaVerifyException(VerifyResponse response)
            : base(DEFAULT_MESSAGE)
        {
            Response = response;
        }

        /// <summary>
        /// reCAPTCHA verify exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="response">The reCAPTCHA verify response object</param>
        public ReCaptchaVerifyException(string message, VerifyResponse response)
            : base(message)
        {
            Response = response;
        }

        /// <summary>
        /// reCAPTCHA verify exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        /// <param name="response">The reCAPTCHA verify response object</param>
        public ReCaptchaVerifyException(string message, Exception innerException, VerifyResponse response)
            : base(message, innerException)
        {
            Response = response;
        }

        /// <summary>
        /// reCAPTCHA verify exception
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        /// <param name="response">The reCAPTCHA verify response object</param>
        public ReCaptchaVerifyException(Exception innerException, VerifyResponse response)
            : base(DEFAULT_MESSAGE, innerException)
        {
            Response = response;
        }

        #endregion

        #region Methods

        #endregion
    }
}
