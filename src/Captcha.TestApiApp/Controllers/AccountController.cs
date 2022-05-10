using Captcha.Core;
using Captcha.TestApiApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace Captcha.TestApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ICaptchaApiProvider _apiProvider;

        public AccountController(ICaptchaApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        [HttpPost("[action]")]
        [ValidateCaptcha(ErrorMessage = "Please enter the security code as a number.",
                    CaptchaGeneratorLanguage = Language.English,
                    CaptchaGeneratorDisplayMode = DisplayMode.SumOfTwoNumbers)]
        public IActionResult Login([FromForm] AccountViewModel data) //NOTE: It's a [FromForm] data or `application/x-www-form-urlencoded` data.
        {
            if (ModelState.IsValid) // If `ValidateCaptcha` fails, it will set a `ModelState.AddModelError`.
            {
                //TODO: Save data
                return Ok(new { name = data.Username });
            }
            return BadRequest(ModelState);
        }

        [HttpGet("[action]")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true, Duration = 0)]
        public ActionResult<CaptchaApiResponse> CreateCaptchaParams()
        {
            // Note: For security reasons, a JavaScript client shouldn't be able to provide these attributes directly.
            // Otherwise an attacker will be able to change them and make them easier!
            return _apiProvider.CreateCaptcha(new CaptchaTagHelperHtmlAttributes
            {
                BackColor = "#f7f3f3",
                FontName = "Tahoma",
                FontSize = 18,
                ForeColor = "#111111",
                Language = Language.English,
                DisplayMode = DisplayMode.SumOfTwoNumbers,
                Max = 90,
                Min = 1
            });
        }
    }
}