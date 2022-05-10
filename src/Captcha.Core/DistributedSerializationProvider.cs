using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Captcha.Core
{
    /// <summary>
    /// Distributed serialization provider
    /// </summary>
    public class DistributedSerializationProvider : ISerializationProvider
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ICaptchaCryptoProvider _captchaProtectionProvider;
        private readonly CaptchaOptions _options;

        /// <summary>
        /// Serialization Provider
        /// </summary>
        public DistributedSerializationProvider(
            IDistributedCache distributedCache,
            ICaptchaCryptoProvider captchaProtectionProvider,
            ILogger<DistributedSerializationProvider> logger,
            IOptions<CaptchaOptions> options)
        {
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            _captchaProtectionProvider = captchaProtectionProvider ?? throw new ArgumentNullException(nameof(captchaProtectionProvider));
            _options = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
            logger.LogDebug("Using the DistributedSerializationProvider.");
        }

        /// <summary>
        /// Serialize the given data to an string.
        /// </summary>
        public string Serialize(object data)
        {
            var resultBytes = JsonSerializer.SerializeToUtf8Bytes(data,
                    new JsonSerializerOptions { WriteIndented = false, IgnoreNullValues = true });
            var token = _captchaProtectionProvider.Hash(Encoding.UTF8.GetString(resultBytes)).HashString;
            _distributedCache.Set(token, resultBytes, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(_options.AbsoluteExpirationMinutes)
            });
            return token;
        }

        /// <summary>
        /// Deserialize the given string to an object.
        /// </summary>
        public T? Deserialize<T>(string data)
        {
            var resultBytes = _distributedCache.Get(data);
            if (resultBytes == null)
            {
                return default;
            }

            _distributedCache.Remove(data);
            return JsonSerializer.Deserialize<T>(new ReadOnlySpan<byte>(resultBytes));
        }
    }
}