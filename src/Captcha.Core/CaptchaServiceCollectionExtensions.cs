using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Captcha.Core
{
    /// <summary>
    ///  Captcha ServiceCollection Extensions
    /// </summary>
    public static class CaptchaServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default Captcha providers.
        /// </summary>
        public static void AddCaptcha(
            this IServiceCollection services,
            Action<CaptchaOptions>? options = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            configOptions(services, options);

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAntiforgery();
            services.AddMvcCore().AddCookieTempDataProvider();

            services.TryAddSingleton<HumanReadableIntegerProvider>();
            services.TryAddSingleton<ShowDigitsProvider>();
            services.TryAddSingleton<SumOfTwoNumbersProvider>();
            services.TryAddSingleton<SumOfTwoNumbersToWordsProvider>();
            services.TryAddSingleton<Func<DisplayMode, ICaptchaTextProvider>>(serviceProvider => key =>
            {
                return key switch
                {
                    DisplayMode.NumberToWord => serviceProvider.GetRequiredService<HumanReadableIntegerProvider>(),
                    DisplayMode.ShowDigits => serviceProvider.GetRequiredService<ShowDigitsProvider>(),
                    DisplayMode.SumOfTwoNumbers => serviceProvider.GetRequiredService<SumOfTwoNumbersProvider>(),
                    DisplayMode.SumOfTwoNumbersToWords => serviceProvider.GetRequiredService<SumOfTwoNumbersToWordsProvider>(),
                    _ => throw new InvalidOperationException($"Service of type {key} is not implemented."),
                };
            });

            services.TryAddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            services.TryAddSingleton<ICaptchaImageProvider, CaptchaImageProvider>();
            services.TryAddSingleton<ICaptchaCryptoProvider, CaptchaCryptoProvider>();
            services.TryAddTransient<CaptchaTagHelper>();
            services.TryAddTransient<ICaptchaValidatorService, CaptchaValidatorService>();
            services.TryAddScoped<ICaptchaApiProvider, CaptchaApiProvider>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddScoped<IUrlHelper>(serviceProvider =>
            {
                var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
                if (actionContext is null)
                {
                    throw new InvalidOperationException("actionContext is null");
                }
                var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });
        }

        private static void configOptions(IServiceCollection services, Action<CaptchaOptions>? options)
        {
            var captchaOptions = new CaptchaOptions();
            options?.Invoke(captchaOptions);
            setCaptchaStorageProvider(services, captchaOptions);
            setSerializationProvider(services, captchaOptions);
            services.TryAddSingleton(Options.Create(captchaOptions));
        }

        private static void setSerializationProvider(IServiceCollection services, CaptchaOptions captchaOptions)
        {
            if (captchaOptions.CaptchaSerializationProvider == null)
            {
                services.TryAddSingleton<ISerializationProvider, InMemorySerializationProvider>();
            }
            else
            {
                services.TryAddSingleton(typeof(ISerializationProvider), captchaOptions.CaptchaSerializationProvider);
            }
        }

        private static void setCaptchaStorageProvider(IServiceCollection services, CaptchaOptions captchaOptions)
        {
            if (captchaOptions.CaptchaStorageProvider == null)
            {
                services.TryAddSingleton<ICaptchaStorageProvider, CookieCaptchaStorageProvider>();
            }
            else
            {
                services.TryAddSingleton(typeof(ICaptchaStorageProvider), captchaOptions.CaptchaStorageProvider);
            }
        }
    }
}