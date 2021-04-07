using GoogleReCaptcha3.Core.Services;
using GoogleReCaptcha3.Examples.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoogleReCaptcha3.Examples.Mvc.Controllers
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
		public IActionResult Index(HomeModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			// Validate recaptcha
			if (!ReCaptchaService.Verify())
			{
				// TODO: Don't do stuff with model because it might be a robot

				ViewBag.Data = model;

				return View("Success");
			}

			// TODO: Do stuff with model (apply business rules and or save it somewhere)

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
