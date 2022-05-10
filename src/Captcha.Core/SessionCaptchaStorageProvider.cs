using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Captcha.Core
{
    /// <summary>
    /// Represents a session storage to save the captcha tokens.
    /// </summary>
    public class SessionCaptchaStorageProvider : ICaptchaStorageProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ILogger<SessionCaptchaStorageProvider> _logger;

        /// <summary>
        /// Represents the storage to save the captcha tokens.
        /// </summary>
        public SessionCaptchaStorageProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            ILogger<SessionCaptchaStorageProvider> logger)
        {
            _captchaProtectionProvider = captchaProtectionProvider ?? throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogDebug("Using the SessionCaptchaStorageProvider.");
        }

        /// <summary>
        /// Adds the specified token and its value to the storage.
        /// </summary>
        public void Add(HttpContext context, string token, string value)
        {
            value = _captchaProtectionProvider.Encrypt($"{value}{context.GetSalt(_captchaProtectionProvider)}");
            context.Session.SetString(token, value);
        }

        /// <summary>
        /// Determines whether the <see cref="ICaptchaStorageProvider" /> contains a specific token.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token">The specified token.</param>
        /// <returns>
        /// <c>True</c> if the value is found in the <see cref="ICaptchaStorageProvider" />; otherwise <c>false</c>.
        /// </returns>
        public bool Contains(HttpContext context, string token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Session.Keys.Any(key => string.Equals(key, token, StringComparison.Ordinal));
        }

        /// <summary>
        /// Gets the value associated with the specified token.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token">The specified token.</param>
        public string? GetValue(HttpContext context, string token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var value = context.Session.GetString(token);
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug("Couldn't find the captcha's session value in the request.");
                return null;
            }

            Remove(context, token);

            var decryptedValue = _captchaProtectionProvider.Decrypt(value);
            return decryptedValue?.Replace(context.GetSalt(_captchaProtectionProvider), string.Empty, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes the specified token from the storage.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token">The specified token.</param>
        public void Remove(HttpContext context, string token)
        {
            if (Contains(context, token))
            {
                context.Session.Remove(token);
            }
        }
    }
}