using GoogleReCaptcha.Core.Services;
using GoogleReCaptcha.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace GoogleReCaptcha.Core
{
	/// <summary>
	/// Service collection extensions for ReCaptcah V3 usage
	/// </summary>
	public static class ServiceCollectionExtensions
	{
		#region Shared

		/// <summary>
		/// Add services shared between V2 & V3
		/// </summary>
		/// <param name="services">Service collection object used to add services</param>
		/// <param name="settings">ReCaptcah root settings object</param>
		private static void AddSharedServices(IServiceCollection services, IReCaptchaSettings settings)
		{
			// Add logging
			services.AddLogging();

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

			// Add root settings for DI
			services.AddScoped<IReCaptchaSettings>((serviceProvider) =>
			{
				return settings;
			});
		}

		#endregion

		#region V2 Service Methods

		/// <summary>
		/// Process and validate <see cref="ReCaptchaV2Settings"/>
		/// </summary>
		/// <param name="settings">Instance of settings that will be used for processing</param>
		private static void ProcessValidateV2Settings(ReCaptchaV2Settings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			// Lib url
			if (!string.IsNullOrWhiteSpace(settings.LibUrl))
			{
				if (!Uri.IsWellFormedUriString(settings.LibUrl, UriKind.RelativeOrAbsolute))
				{
					throw new UriFormatException($"Invalid URI property {nameof(settings.LibUrl)}");
				}
			}
			else
			{
				settings.LibUrl = Constants.DEFAULT_V2_LIBURL;
			}

			// Api url
			if (!string.IsNullOrWhiteSpace(settings.ApiUrl))
			{
				if (!Uri.IsWellFormedUriString(settings.ApiUrl, UriKind.RelativeOrAbsolute))
				{
					throw new UriFormatException($"Invalid URI property {nameof(settings.ApiUrl)}");
				}

				// Fix missing trailing slash
				if (!settings.ApiUrl.EndsWith("/"))
				{
					settings.ApiUrl = settings.ApiUrl + "/";
				}
			}
			else
			{
				settings.ApiUrl = Constants.DEFAULT_V2_APIURL;
			}

			// Site key
			if (string.IsNullOrWhiteSpace(settings.SiteKey))
			{
				throw new ArgumentException($"Invalid property {nameof(settings.SiteKey)}");
			}

			// Secret key
			if (string.IsNullOrWhiteSpace(settings.SecretKey))
			{
				throw new ArgumentException($"Invalid property {nameof(settings.SecretKey)}");
			}
		}

		private static void AddV2BaseServices(IServiceCollection services, ReCaptchaV2Settings settings)
		{
			// Process settings
			ProcessValidateV2Settings(settings);

			// Add shared services
			AddSharedServices(services, settings);

			// Add settings for DI
			services.AddScoped<IReCaptchaV2Settings>((serviceProvider) =>
			{
				return settings;
			});

			// Add V2 service
			services.AddScoped<IReCaptchaV2Service, ReCaptchaV2Service>((serviceProvider) =>
			{
				// Get required services
				var logger = serviceProvider.GetRequiredService<ILogger<ReCaptchaV2Service>>();
				var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
				var httpContextFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

				// Build v2 service
				var v2Service = new ReCaptchaV2Service(logger, actionContextAccessor, httpContextFactory, settings);
				return v2Service;
			});
		}

		public static void AddGoogleReCaptchaV2(this IServiceCollection @this, IConfiguration config, string settingsKey = null)
		{
			if (config == null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			// Get V2 settings from config
			settingsKey = settingsKey ?? "GoogleReCaptcha:V2";
			var configSection = config.GetSection(settingsKey);
			var settings = configSection.Get<ReCaptchaV2Settings>();

			// Add V3 services
			AddV2BaseServices(@this, settings);
		}

		public static void AddGoogleReCaptchaV2(this IServiceCollection @this, Func<IReCaptchaV2Settings> getSettingsAction)
		{
			if (getSettingsAction == null)
			{
				throw new ArgumentNullException(nameof(getSettingsAction));
			}

			// Get V2 settings from func
			var settings = getSettingsAction() as ReCaptchaV2Settings;

			// Add V2 services
			AddV2BaseServices(@this, settings);
		}

		#endregion

		#region V3 Service Methods

		/// <summary>
		/// Process and validate ReCaptchaV3Settings
		/// </summary>
		/// <param name="settings">Instance of settings that will be used for processing</param>
		private static void ProcessValidateV3Settings(ReCaptchaV3Settings settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			// Lib url
			if (!string.IsNullOrWhiteSpace(settings.LibUrl))
			{
				if (!Uri.IsWellFormedUriString(settings.LibUrl, UriKind.RelativeOrAbsolute))
				{
					throw new UriFormatException($"Invalid URI property {nameof(settings.LibUrl)}");
				}
			}
			else
			{
				settings.LibUrl = Constants.DEFAULT_V3_LIBURL;
			}

			// Api url
			if (!string.IsNullOrWhiteSpace(settings.ApiUrl))
			{
				if (!Uri.IsWellFormedUriString(settings.ApiUrl, UriKind.RelativeOrAbsolute))
				{
					throw new UriFormatException($"Invalid URI property {nameof(settings.ApiUrl)}");
				}

				// Fix missing trailing slash
				if (!settings.ApiUrl.EndsWith("/"))
				{
					settings.ApiUrl = settings.ApiUrl + "/";
				}
			}
			else
			{
				settings.ApiUrl = Constants.DEFAULT_V3_APIURL;
			}

			// Site key
			if (string.IsNullOrWhiteSpace(settings.SiteKey))
			{
				throw new ArgumentException($"Invalid property {nameof(settings.SiteKey)}");
			}

			// Secret key
			if (string.IsNullOrWhiteSpace(settings.SecretKey))
			{
				throw new ArgumentException($"Invalid property {nameof(settings.SecretKey)}");
			}

			// Default passing score
			if (settings.DefaultPassingScore < 0)
			{
				settings.DefaultPassingScore = Constants.DEFAULT_V3_PASSING_SCORE;
			}
		}

		private static void AddV3BaseServices(IServiceCollection services, ReCaptchaV3Settings settings)
		{
			// Process settings
			ProcessValidateV3Settings(settings);

			// Add shared services
			AddSharedServices(services, settings);

			// Add settings for DI
			services.AddScoped<IReCaptchaV3Settings>((serviceProvider) =>
			{
				return settings;
			});

			// Add V3 service using base interface; default service to use is V3
			services.AddScoped<IReCaptchaService, ReCaptchaV3Service>((serviceProvider) =>
			{
				// Get required services
				var logger = serviceProvider.GetRequiredService<ILogger<ReCaptchaV3Service>>();
				var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
				var httpContextFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

				// Build v3 service
				var v3Service = new ReCaptchaV3Service(logger, actionContextAccessor, httpContextFactory, settings);
				return v3Service;
			});

			// Add V3 service
			services.AddScoped<IReCaptchaV3Service, ReCaptchaV3Service>((serviceProvider) =>
			{
				// Get required services
				var logger = serviceProvider.GetRequiredService<ILogger<ReCaptchaV3Service>>();
				var actionContextAccessor = serviceProvider.GetRequiredService<IActionContextAccessor>();
				var httpContextFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

				// Build v3 service
				var v3Service = new ReCaptchaV3Service(logger, actionContextAccessor, httpContextFactory, settings);
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
			var settings = getSettingsAction() as ReCaptchaV3Settings;

			// Add V3 services
			AddV3BaseServices(@this, settings);
		}

		#endregion
	}
}
