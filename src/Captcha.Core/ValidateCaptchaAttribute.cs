using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Captcha.Core
{
    /// <summary>
    /// Validate Captcha Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ValidateCaptchaAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The language of captcha generator. It's default value is Persian.
        /// </summary>
        public Language CaptchaGeneratorLanguage { set; get; } = Language.Persian;

        /// <summary>
        /// The display mode of captcha generator. It's default value is NumberToWord.
        /// </summary>
        public DisplayMode CaptchaGeneratorDisplayMode { set; get; }

        /// <summary>
        /// Validation error message. It's default value is `لطفا کد امنیتی را به رقم وارد نمائید`.
        /// </summary>
        public string ErrorMessage { set; get; } = "لطفا کد امنیتی را به رقم وارد نمائید";

        /// <summary>
        /// Captcha validator.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var httpContext = context.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("httpContext is null.");
            }

            var validatorService = httpContext.RequestServices.GetRequiredService<ICaptchaValidatorService>();
            if (validatorService.HasRequestValidCaptchaEntry(CaptchaGeneratorLanguage, CaptchaGeneratorDisplayMode))
            {
                base.OnActionExecuting(context);
                return;
            }

            if (context.Controller is not ControllerBase controllerBase)
            {
                throw new InvalidOperationException("controllerBase is null.");
            }

            var options = httpContext.RequestServices.GetRequiredService<IOptions<CaptchaOptions>>();
            controllerBase.ModelState.AddModelError(options.Value.CaptchaComponent.CaptchaInputName, ErrorMessage);
            base.OnActionExecuting(context);
        }
    }
}