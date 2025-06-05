using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;

namespace ResourceApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            if (!User.HasScope("message_api"))
            {
                return Forbid(authenticationSchemes: OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string>
                    {
                        [OpenIddictValidationAspNetCoreConstants.Properties.Scope] = "message_api",
                        [OpenIddictValidationAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InsufficientScope,
                        [OpenIddictValidationAspNetCoreConstants.Properties.ErrorDescription] =
                            "The 'message_api' scope is required to perform this action."
                    }));
            }

            // Hent brugerens subject claim (fx bruger-id)
            var userId = User.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Access token does not contain the required subject claim.");
            }

            return Ok($"User '{userId}' has been successfully authenticated. \nHello from the Resource API! ");
        }
    }
}
