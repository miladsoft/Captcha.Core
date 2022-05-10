using System;
using Captcha.Core;
using Captcha.TestWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Captcha.TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICaptchaValidatorService _validatorService;
        private readonly CaptchaOptions _captchaOptions;

        public HomeController(
            ICaptchaValidatorService validatorService,
            IOptions<CaptchaOptions> options
            )
        {
            _validatorService = validatorService;
            _captchaOptions = options == null ? throw new ArgumentNullException(nameof(options)) : options.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        // You can use it as an ActionFilter.
        [HttpPost, ValidateAntiForgeryToken]
        [ValidateCaptcha(ErrorMessage = "Please enter the security code as a number.",
                            CaptchaGeneratorLanguage = Language.English,
                            CaptchaGeneratorDisplayMode = DisplayMode.NumberToWord)]
        public IActionResult Index([FromForm] AccountViewModel data)
        {
            if (ModelState.IsValid) // If `ValidateCaptcha` fails, it will set a `ModelState.AddModelError`.
            {
                //TODO: Save data
                return RedirectToAction(nameof(Thanks), new { name = data.Username });
            }
            return View();
        }

        // Or you can use the `ICaptchaValidatorService` directly.
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login2([FromForm] AccountViewModel data)
        {
            if (!_validatorService.HasRequestValidCaptchaEntry(Language.English, DisplayMode.NumberToWord))
            {
                this.ModelState.AddModelError(_captchaOptions.CaptchaComponent.CaptchaInputName, "Please enter the security code as a number.");
                return View(nameof(Index));
            }

            //TODO: Save data
            return RedirectToAction(nameof(Thanks), new { name = data.Username });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ValidateCaptcha(ErrorMessage = "Please enter the security code as a number.",
                            CaptchaGeneratorLanguage = Language.English,
                            CaptchaGeneratorDisplayMode = DisplayMode.NumberToWord)]
        public IActionResult Login3([FromForm] AccountViewModel data) // For Ajax Forms
        {
            if (ModelState.IsValid) // If `ValidateCaptcha` fails, it will set a `ModelState.AddModelError`.
            {
                //TODO: Save data
                return Ok();
            }
            return BadRequest(ModelState);
        }

        public IActionResult Thanks(string name)
        {
            return View(nameof(Thanks), name);
        }
    }
}