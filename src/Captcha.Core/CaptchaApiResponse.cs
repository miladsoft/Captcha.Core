namespace Captcha.Core
{
    /// <summary>
    /// ApiProvider Response
    /// </summary>
    public class CaptchaApiResponse
    {
        /// <summary>
        /// The captach's image url
        /// </summary>
        /// <value></value>
        public string CaptchaImgUrl { set; get; } = default!;

        /// <summary>
        /// Captcha Id
        /// </summary>
        public string CaptchaId { set; get; } = default!;

        /// <summary>
        /// Captcha's TextValue
        /// </summary>
        public string CaptchaTextValue { set; get; } = default!;

        /// <summary>
        /// Captcha's TokenValue
        /// </summary>
        public string CaptchaTokenValue { set; get; } = default!;
    }
}