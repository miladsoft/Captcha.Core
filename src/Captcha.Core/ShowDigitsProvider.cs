using System;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace Captcha.Core
{
    /// <summary>
    /// display a numeric value using the equivalent text
    /// </summary>
    public class ShowDigitsProvider : ICaptchaTextProvider
    {
        private readonly CaptchaOptions _captchaOptions;

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        public ShowDigitsProvider(IOptions<CaptchaOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _captchaOptions = options.Value;
        }

        /// <summary>
        /// display a numeric value using the equivalent text
        /// </summary>
        /// <param name="number">input number</param>
        /// <param name="language">local language</param>
        /// <returns>the equivalent text</returns>
        public string GetText(long number, Language language)
        {
            var text = _captchaOptions.AllowThousandsSeparators ?
                            string.Format(CultureInfo.InvariantCulture, "{0:N0}", number) :
                            number.ToString(CultureInfo.InvariantCulture);
            return language == Language.Persian ? text.ToPersianNumbers() : text;
        }
    }
}