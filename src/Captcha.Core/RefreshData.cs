using System;

namespace Captcha.Core
{
    /// <summary>
    /// Refresh Data
    /// </summary>
    [Serializable]
    public class RefreshData : CaptchaTagHelperHtmlAttributes
    {
        /// <summary>
        /// Current Date
        /// </summary>
        public long RndDate { get; set; }
    }
}