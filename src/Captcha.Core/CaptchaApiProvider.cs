﻿using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static System.FormattableString;

namespace Captcha.Core
{
    /// <summary>
    /// Captcha Api
    /// </summary>
    public class CaptchaApiProvider : ICaptchaApiProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ICaptchaStorageProvider _captchaStorageProvider;
        private readonly Func<DisplayMode, ICaptchaTextProvider> _captchaTextProvider;
        private readonly IRandomNumberProvider _randomNumberProvider;
        private readonly ISerializationProvider _serializationProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;
        private readonly CaptchaOptions _captchaOptions;

        /// <summary>
        /// Captcha Api
        /// </summary>
        public CaptchaApiProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            IRandomNumberProvider randomNumberProvider,
            Func<DisplayMode, ICaptchaTextProvider> captchaTextProvider,
            ICaptchaStorageProvider captchaStorageProvider,
            ISerializationProvider serializationProvider,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelper urlHelper,
            IOptions<CaptchaOptions> options)
        {
            _captchaProtectionProvider = captchaProtectionProvider ?? throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _randomNumberProvider = randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
            _captchaTextProvider = captchaTextProvider ?? throw new ArgumentNullException(nameof(captchaTextProvider));
            _captchaStorageProvider = captchaStorageProvider ?? throw new ArgumentNullException(nameof(captchaStorageProvider));
            _serializationProvider = serializationProvider ?? throw new ArgumentNullException(nameof(serializationProvider));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _urlHelper = urlHelper ?? throw new ArgumentNullException(nameof(urlHelper));
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }

        /// <summary>
        /// Creates Captcha
        /// </summary>
        /// <param name="captchaAttributes">captcha attributes</param>
        public CaptchaApiResponse CreateCaptcha(CaptchaTagHelperHtmlAttributes captchaAttributes)
        {
            if (captchaAttributes == null)
            {
                throw new ArgumentNullException(nameof(captchaAttributes));
            }

            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("`_httpContextAccessor.HttpContext` is null.");
            }

            var number = _randomNumberProvider.NextNumber(captchaAttributes.Min, captchaAttributes.Max);
            var randomText = _captchaTextProvider(captchaAttributes.DisplayMode).GetText(number, captchaAttributes.Language);
            var encryptedText = _captchaProtectionProvider.Encrypt(randomText);
            var captchaImageUrl = getCaptchaImageUrl(captchaAttributes, encryptedText);
            var captchaDivId = Invariant($"{_captchaOptions.CaptchaClass}{Guid.NewGuid():N}{_randomNumberProvider.NextNumber(captchaAttributes.Min, captchaAttributes.Max)}");
            var cookieToken = $".{captchaDivId}";
            var hiddenInputToken = _captchaProtectionProvider.Encrypt(cookieToken);

            _captchaStorageProvider.Add(_httpContextAccessor.HttpContext, cookieToken, randomText);

            return new CaptchaApiResponse
            {
                CaptchaImgUrl = captchaImageUrl,
                CaptchaId = captchaDivId,
                CaptchaTextValue = encryptedText,
                CaptchaTokenValue = hiddenInputToken
            };
        }

        private string getCaptchaImageUrl(CaptchaTagHelperHtmlAttributes captchaAttributes, string encryptedText)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new InvalidOperationException("`_httpContextAccessor.HttpContext` is null.");
            }

            var values = new CaptchaImageParams
            {
                Text = encryptedText,
                RndDate = DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture),
                ForeColor = captchaAttributes.ForeColor,
                BackColor = captchaAttributes.BackColor,
                FontSize = captchaAttributes.FontSize,
                FontName = captchaAttributes.FontName
            };
            var encryptSerializedValues = _captchaProtectionProvider.Encrypt(_serializationProvider.Serialize(values));
            var actionUrl = captchaAttributes.UseRelativeUrls ?
                _urlHelper.Action(action: nameof(CaptchaImageController.Show),
                            controller: nameof(CaptchaImageController).Replace("Controller", string.Empty, StringComparison.Ordinal),
                            values: new { data = encryptSerializedValues, area = "" }) :
                _urlHelper.Action(action: nameof(CaptchaImageController.Show),
                            controller: nameof(CaptchaImageController).Replace("Controller", string.Empty, StringComparison.Ordinal),
                            values: new { data = encryptSerializedValues, area = "" },
                            protocol: _httpContextAccessor.HttpContext.Request.Scheme);

            if (string.IsNullOrWhiteSpace(actionUrl))
            {
                throw new InvalidOperationException("It's not possible to determine the URL of the `CaptchaImageController.Show` method. Please register the `services.AddControllers()` and `endpoints.MapControllerRoute(...)`.");
            }

            return actionUrl;
        }
    }
}