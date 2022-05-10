namespace Captcha.Core
{
    /// <summary>
    /// Defines Captcha's input names
    /// </summary>
    public class CaptchaComponent
    {
        /// <summary>
        /// The default hidden input name of the captcha.
        /// Its default value is `CaptchaText`.
        /// </summary>
        public string CaptchaHiddenInputName { get; set; } = "CaptchaText";

        /// <summary>
        /// The default hidden input name of the captcha's cookie token.
        /// Its default value is `CaptchaToken`.
        /// </summary>
        public string CaptchaHiddenTokenName { get; set; } = "CaptchaToken";

        /// <summary>
        /// The default input name of the captcha.
        /// Its default value is `CaptchaInputText`.
        /// </summary>
        public string CaptchaInputName { get; set; } = "CaptchaInputText";
    }
}