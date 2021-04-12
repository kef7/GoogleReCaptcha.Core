using GoogleReCaptcha.Core.Services;
using GoogleReCaptcha.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace GoogleReCaptcha.Core
{
	public static class ServiceCollectionExtensions
	{
		private static void AddV3BaseServices(IServiceCollection services, IReCaptchaV3Settings settings)
		{
			// Add IActionContextAccessor for IUrlHelper DI
			services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

			// Add IUrlHelper for DI
			services.AddScoped<IUrlHelper>((serviceProvider) =>
			{
				var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
				var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
				return factory.GetUrlHelper(actionContext);
			});

			// Add IHttpClientFactory for ID
			services.AddHttpClient(Constants.DEFAULT_HTTP_CLIENT_NAME, (httpClient) =>
			{
				if (!string.IsNullOrWhiteSpace(settings.ApiUrl))
				{
					httpClient.BaseAddress = new Uri(settings.ApiUrl);
				}
				httpClient.DefaultRequestHeaders.Add("Accepts", "application/json");
			});

			// Add V3 service
			services.AddScoped<IReCaptchaService, ReCaptchaV3Service>((serviceProvider) =>
			{
				// Get required services
				var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
				var httpContextFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

				// Build v3 service
				var v3Service = new ReCaptchaV3Service(settings, actionContextAccessor, httpContextFactory);
				return v3Service;
			});
		}

		public static void AddGoogleReCaptchaV3(this IServiceCollection @this, IConfiguration config, string settingsKey = null)
		{
			if (config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			// Get V3 settings from config
			settingsKey = settingsKey ?? "GoogleReCaptcha:V3";
			var configSection = config.GetSection(settingsKey);
			var settings = configSection.Get<ReCaptchaV3Settings>();

			// Add settings for DI
			@this.AddScoped<IReCaptchaV3Settings, ReCaptchaV3Settings>((serviceProvider) =>
			{
				return settings;
			});

			// Add V3 services
			AddV3BaseServices(@this, settings);
		}

		public static void AddGoogleReCaptchaV3(this IServiceCollection @this, Func<IReCaptchaV3Settings> getSettingsAction)
		{
			if (getSettingsAction == null)
			{
				throw new ArgumentNullException(nameof(getSettingsAction));
			}

			// Get V3 settings from func
			var settings = getSettingsAction();

			// Add settings for Di
			@this.AddScoped<IReCaptchaV3Settings>((serviceProvider) =>
			{
				return settings;
			});

			// Add V3 services
			AddV3BaseServices(@this, settings);
		}
	}
}
