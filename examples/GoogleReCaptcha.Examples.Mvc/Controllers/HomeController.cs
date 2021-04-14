using GoogleReCaptcha.Core.Services;
using GoogleReCaptcha.Examples.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GoogleReCaptcha.Examples.Mvc.Controllers
{
	public class HomeController : Controller
	{
		public IReCaptchaService ReCaptchaService { get; }

		public HomeController(IReCaptchaService reCapthcaService)
		{
			ReCaptchaService = reCapthcaService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(HomeModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Validate recaptcha
			if (!await ReCaptchaService.VerifyAsync())
			{
				// Don't do stuff with model because it might be a robot's data

				ViewBag.ErrorMsg = "Invalid form";

				return View(model);
			}

			// Do stuff with model (apply business rules and or save it somewhere)

			ViewBag.Data = model;

			return View("Success");
		}

		[HttpGet]
		public IActionResult Success()
		{
			if (ViewBag.Data == null)
			{
				return RedirectToAction("Index", "Home");
			}

			return View();
		}
	}
}
