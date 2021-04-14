using System.Threading.Tasks;

namespace GoogleReCaptcha.Core.Services
{
	public interface IReCaptchaV3Service : IReCaptchaService
	{
		/// <summary>
		/// Verify reCAPTCHA with a passing value that is equal to or greater then the response score value
		/// </summary>
		/// <param name="passing">Passing value in which the response score should be equal to or greater than</param>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		bool Verify(float passing);

		/// <summary>
		/// Verify reCAPTCHA with a passing value that is equal to or greater then the response score value
		/// </summary>
		/// <param name="passing">Passing value in which the response score should be equal to or greater than</param>
		/// <returns>True if passing value is equal to or greater than the response score</returns>
		Task<bool> VerifyAsync(float passing);
	}
}
