using Captcha.Core;
using Captcha.TestWebApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Captcha.TestWebApp.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    public class NgxController : Controller
    {
        private readonly ICaptchaApiProvider _apiProvider;

        public NgxController(ICaptchaApiProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        [HttpPost("[action]")]
        [ValidateCaptcha(ErrorMessage = "Please enter the security code as a number.",
                            CaptchaGeneratorLanguage = Language.English,
                            CaptchaGeneratorDisplayMode = DisplayMode.NumberToWord)]
        public IActionResult Login([FromBody] AccountViewModel data)
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