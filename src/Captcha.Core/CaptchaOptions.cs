using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace Captcha.Core
{
    /// <summary>
    /// Defines Captcha's Options
    /// </summary>
    public class CaptchaOptions
    {
        /// <summary>
        /// Defines options of the captcha's noise
        /// </summary>
        public CaptchaNoise CaptchaNoise { get; set; } = new();

        /// <summary>
        /// Gets or sets the value for the SameSite attribute of the cookie.
        /// Its default value is `SameSiteMode.Strict`. If you are using CORS, set it to `None`.
        /// </summary>
        public SameSiteMode SameSiteMode { get; set; } = SameSiteMode.Strict;

        /// <summary>
        /// You can introduce a custom ICaptchaStorageProvider to be used as an StorageProvider.
        /// </summary>
        public Type? CaptchaStorageProvider { get; set; }

        /// <summary>
        /// You can introduce a custom SerializationProvider here.
        /// </summary>
        public Type? CaptchaSerializationProvider { get; set; }

        /// <summary>
        /// You can introduce a custom font here.
        /// </summary>
        public string? CustomFontPath { get; set; }

        /// <summary>
        /// The encryption key
        /// </summary>
        public string? EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets an absolute expiration date for the cache entry.
        /// Its default value is 7.
        /// </summary>
        public int AbsoluteExpirationMinutes { get; set; } = 7;

        /// <summary>
        /// Shows thousands separators such as 100,100,100 in ShowDigits mode.
        /// Its default value is true.
        /// </summary>
        public bool AllowThousandsSeparators { get; set; } = true;

        /// <summary>
        /// Defines Captcha's input names
        /// </summary>
        public CaptchaComponent CaptchaComponent { get; set; } = new CaptchaComponent();

        /// <summary>
        /// The CSS class name of the captcha's DIV.
        /// Its default value is `Captcha`.
        /// </summary>
        public string CaptchaClass { get; set; } = "Captcha";

        /// <summary>
        /// The CSS class name of the captcha's DIV.
        /// Its default value is `Captcha`.
        /// </summary>
        public CaptchaOptions Identifier(string className)
        {
            CaptchaClass = className;

            return this;
        }

        /// <summary>
        /// Defines Captcha's input names.
        /// </summary>
        public CaptchaOptions InputNames(CaptchaComponent component)
        {
            CaptchaComponent = component;

            return this;
        }

        /// <summary>
        /// Shows thousands separators such as 100,100,100 in ShowDigits mode.
        /// Its default value is true.
        /// </summary>
        public CaptchaOptions ShowThousandsSeparators(bool show)
        {
            AllowThousandsSeparators = show;

            return this;
        }

        /// <summary>
        /// Sets an absolute expiration date for the cache entry.
        /// Its default value is 7.
        /// </summary>
        public CaptchaOptions AbsoluteExpiration(int minutes)
        {
            AbsoluteExpirationMinutes = minutes;

            return this;
        }

        /// <summary>
        /// The encryption key.
        /// If you don't specify it, a random value will be used.
        /// </summary>
        public CaptchaOptions WithEncryptionKey(string key)
        {
            EncryptionKey = key;

            return this;
        }

        /// <summary>
        /// Defines options of the captcha's noise.
        /// </summary>
        public CaptchaOptions WithNoise(float pixelsDensity, int linesCount)
        {
            CaptchaNoise = new CaptchaNoise
            {
                NoiseLinesCount = linesCount,
                NoisePixelsDensity = pixelsDensity
            };
            return this;
        }

        /// <summary>
        /// You can introduce a custom font here.
        /// </summary>
        public CaptchaOptions UseCustomFont(string fullPath)
        {
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"`{fullPath}` file not found!");
            }

            CustomFontPath = fullPath;
            return this;
        }

        /// <summary>
        /// You can introduce a custom ICaptchaStorageProvider to be used as an StorageProvider.
        /// </summary>
        /// <typeparam name="T">Implements ICaptchaStorageProvider</typeparam>
        public CaptchaOptions UseCustomStorageProvider<T>() where T : ICaptchaStorageProvider
        {
            CaptchaStorageProvider = typeof(T);
            return this;
        }

        /// <summary>
        /// Using the IDistributedCache
        /// Don't forget to configure your DistributedCache provider such as `services.AddStackExchangeRedisCache()` first.
        /// </summary>
        public CaptchaOptions UseDistributedSerializationProvider()
        {
            CaptchaSerializationProvider = typeof(DistributedSerializationProvider);
            return this;
        }

        /// <summary>
        /// Using the IMemoryCache
        /// </summary>
        public CaptchaOptions UseInMemorySerializationProvider()
        {
            CaptchaSerializationProvider = typeof(InMemorySerializationProvider);
            return this;
        }

        /// <summary>
        /// Introduces the built-in `SessionCaptchaStorageProvider` to be used as an StorageProvider.
        /// Don't forget to add `services.AddSession();` in ConfigureServices() method and `app.UseSession();` in Configure() method.
        /// </summary>
        public CaptchaOptions UseSessionStorageProvider()
        {
            CaptchaStorageProvider = typeof(SessionCaptchaStorageProvider);
            return this;
        }

        /// <summary>
        /// Introduces the built-in `CookieCaptchaStorageProvider` to be used as an StorageProvider.
        /// </summary>
        /// <param name="sameSite">
        /// Sets the value for the SameSite attribute of the cookie.
        /// Its default value is `SameSiteMode.Strict`. If you are using CORS, set it to `None`.
        /// </param>
        public CaptchaOptions UseCookieStorageProvider(SameSiteMode sameSite = SameSiteMode.Strict)
        {
            SameSiteMode = sameSite;
            CaptchaStorageProvider = typeof(CookieCaptchaStorageProvider);
            return this;
        }

        /// <summary>
        /// Introduces the built-in `CookieCaptchaStorageProvider` to be used as an StorageProvider.
        /// </summary>
        public CaptchaOptions UseMemoryCacheStorageProvider()
        {
            CaptchaStorageProvider = typeof(MemoryCacheCaptchaStorageProvider);
            return this;
        }

        /// <summary>
        /// Introduces the built-in `DistributedCacheCaptchaStorageProvider` to be used as an StorageProvider.
        /// Don't forget to configure your DistributedCache provider such as `services.AddStackExchangeRedisCache()` first.
        /// </summary>
        public CaptchaOptions UseDistributedCacheStorageProvider()
        {
            CaptchaStorageProvider = typeof(DistributedCacheCaptchaStorageProvider);
            CaptchaSerializationProvider = typeof(DistributedSerializationProvider);
            return this;
        }
    }
}