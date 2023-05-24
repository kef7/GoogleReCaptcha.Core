namespace GoogleReCaptcha.Core
{
    using GoogleReCaptcha.Core.Services;
    using GoogleReCaptcha.Core.Settings;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service collection extensions for reCAPTCHA v3 usage
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Static Fields

        /// <summary>
        /// Static flag to indicate that shared previous services method as been called
        /// </summary>
        private static bool s_sharedPrevCalled = false;

        #endregion

        #region Shared

        /// <summary>
        /// Add services shared between reCAPTCHA v2 and v3
        /// </summary>
        /// <param name="services">Service collection object used to add services</param>
        /// <param name="settings">reCAPTCHA root settings object</param>
        private static void AddSharedServices(IServiceCollection services, IReCaptchaSettings settings)
        {
            // Add root settings for DI
            services.AddScoped<IReCaptchaSettings>((serviceProvider) =>
            {
                return settings;
            });

            if (!s_sharedPrevCalled)
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
                    return factory.GetUrlHelper(actionContext!);
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

                // Flag to to call again
                s_sharedPrevCalled = true;
            }
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

        /// <summary>
        /// Adds base reCAPTCHA v2 services
        /// </summary>
        /// <param name="services">Service collection to use to add support too</param>
        /// <param name="settings">reCAPTCHA settings to use for service</param>
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

        /// <summary>
        /// Add reCAPTCHA v2 services support
        /// </summary>
        /// <param name="serviceCollection">Service collection to use to add support too</param>
        /// <param name="config">Current configuration</param>
        /// <param name="settingsKey">Settings configuration key where reCAPTCHA settings are in <paramref name="config"/></param>
        /// <remarks>
        /// Last `AddGoogleReCaptchaV#` called will set settings IReCaptchaSettings DI with its own settings object
        /// </remarks>
        public static void AddGoogleReCaptchaV2(this IServiceCollection serviceCollection, IConfiguration config, string? settingsKey = null)
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
            AddV2BaseServices(serviceCollection, settings);
        }

        /// <summary>
        /// Add reCAPTCHA v2 services support
        /// </summary>
        /// <param name="serviceCollection">Service collection to use to add support too</param>
        /// <param name="getSettingsFunc">Function called that will return reCAPTCHA settings to use</param>
        /// <remarks>
        /// Last `AddGoogleReCaptchaV#` called will set settings IReCaptchaSettings DI with its own settings object
        /// </remarks>
        public static void AddGoogleReCaptchaV2(this IServiceCollection serviceCollection, Func<IReCaptchaV2Settings> getSettingsFunc)
        {
            if (getSettingsFunc == null)
            {
                throw new ArgumentNullException(nameof(getSettingsFunc));
            }

            // Get V2 settings from func
            var settings = getSettingsFunc() as ReCaptchaV2Settings
                ?? throw new InvalidOperationException($"Could not obtain settings when invoking function {nameof(getSettingsFunc)}");

            // Add V2 services
            AddV2BaseServices(serviceCollection, settings);
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

        /// <summary>
        /// Adds base reCAPTCHA v3 services
        /// </summary>
        /// <param name="services">Service collection to use to add support too</param>
        /// <param name="settings">reCAPTCHA settings to use for service</param>
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

        /// <summary>
        /// Add reCAPTCHA v3 services support
        /// </summary>
        /// <param name="serviceCollection">Service collection to use to add support too</param>
        /// <param name="config">Current configuration</param>
        /// <param name="settingsKey">Settings configuration key where reCAPTCHA settings are in <paramref name="config"/></param>
        /// <remarks>
        /// Last `AddGoogleReCaptchaV#` called will set settings IReCaptchaSettings DI with its own settings object
        /// </remarks>
        public static void AddGoogleReCaptchaV3(this IServiceCollection serviceCollection, IConfiguration config, string? settingsKey = null)
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
            AddV3BaseServices(serviceCollection, settings);
        }

        /// <summary>
        /// Add reCAPTCHA v3 services support
        /// </summary>
        /// <param name="serviceCollection">Service collection to use to add support too</param>
        /// <param name="getSettingsFunc">Function called that will return reCAPTCHA settings to use</param>
        /// <remarks>
        /// Last `AddGoogleReCaptchaV#` called will set settings IReCaptchaSettings DI with its own settings object
        /// </remarks>
        public static void AddGoogleReCaptchaV3(this IServiceCollection serviceCollection, Func<IReCaptchaV3Settings> getSettingsFunc)
        {
            if (getSettingsFunc == null)
            {
                throw new ArgumentNullException(nameof(getSettingsFunc));
            }

            // Get V3 settings from func
            var settings = getSettingsFunc() as ReCaptchaV3Settings
                ?? throw new InvalidOperationException($"Could not obtain settings when invoking function {nameof(getSettingsFunc)}");

            // Add V3 services
            AddV3BaseServices(serviceCollection, settings);
        }

        #endregion
    }
}
