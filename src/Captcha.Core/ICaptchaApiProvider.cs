namespace Captcha.Core
{
    /// <summary>
    /// Captcha Api
    /// </summary>
    public interface ICaptchaApiProvider
    {
        /// <summary>
        /// Creates Captcha
        /// </summary>
        /// <param name="captchaAttributes">captcha attributes</param>
        CaptchaApiResponse CreateCaptcha(CaptchaTagHelperHtmlAttributes captchaAttributes);
    }
}