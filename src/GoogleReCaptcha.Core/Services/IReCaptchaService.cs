using System.Threading.Tasks;

namespace GoogleReCaptcha.Core.Services
{
	public interface IReCaptchaService
	{
		string GetToken();
		bool Verify();
		Task<bool> VerifyAsync();
	}
}
