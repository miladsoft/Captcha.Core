using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Captcha.Core
{
    /// <summary>
    /// Represents a distributed cache storage to save the captcha tokens.
    /// </summary>
    public class DistributedCacheCaptchaStorageProvider : ICaptchaStorageProvider
    {
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly ILogger<DistributedCacheCaptchaStorageProvider> _logger;
        private readonly IDistributedCache _distributedCache;
        private readonly CaptchaOptions _options;

        /// <summary>
        /// Represents the storage to save the captcha tokens.
        /// </summary>
        public DistributedCacheCaptchaStorageProvider(
            ICaptchaCryptoProvider captchaProtectionProvider,
            IDistributedCache distributedCache,
            ILogger<DistributedCacheCaptchaStorageProvider> logger,
            IOptions<CaptchaOptions> options)
        {
            _captchaProtectionProvider = captchaProtectionProvider ?? throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;

            _logger.LogDebug("Using the DistributedCacheCaptchaStorageProvider.");
        }

        /// <summary>
        /// Adds the specified token and its value to the storage.
        /// </summary>
        public void Add(HttpContext context, string token, string value)
        {
            value = _captchaProtectionProvider.Encrypt($"{value}{context.GetSalt(_captchaProtectionProvider)}");
            _distributedCache.Set(token, Encoding.UTF8.GetBytes(value), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_options.AbsoluteExpirationMinutes)
            });
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
            return _distributedCache.Get(token) != null;
        }

        /// <summary>
        /// Gets the value associated with the specified token.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token">The specified token.</param>
        public string? GetValue(HttpContext context, string token)
        {
            var cookieValueBytes = _distributedCache.Get(token);
            if (cookieValueBytes == null)
            {
                _logger.LogDebug("Couldn't find the captcha cookie in the request.");
                return null;
            }

            _distributedCache.Remove(token);
            var decryptedValue = _captchaProtectionProvider.Decrypt(Encoding.UTF8.GetString(cookieValueBytes));
            return decryptedValue?.Replace(context.GetSalt(_captchaProtectionProvider), string.Empty, StringComparison.Ordinal);
        }

        /// <summary>
        /// Removes the specified token from the storage.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token">The specified token.</param>
        public void Remove(HttpContext context, string token)
        {
            _distributedCache.Remove(token);
        }
    }
}