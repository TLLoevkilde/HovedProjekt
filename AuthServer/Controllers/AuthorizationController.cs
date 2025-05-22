using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Controllers
{
    public class AuthorizationController(IOpenIddictApplicationManager applicationManager, SignInManager<IdentityUser> signInManager) : Controller
    {
        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            // Håndter client_credentials flow
            if (request.IsClientCredentialsGrantType())
            {    
                var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Krævet claim: Subject (sub)
                identity.AddClaim(Claims.Subject, request.ClientId!,Destinations.AccessToken);

                // Opret principal
                var principal = new ClaimsPrincipal(identity);
                principal.SetResources("resource_server");
                principal.SetScopes(request.GetScopes());

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Håndter authorization_code flow
            else if (request.IsAuthorizationCodeGrantType())
            {
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal
                    ?? throw new InvalidOperationException("Brugerens principal kunne ikke hentes.");
                principal.SetResources("resource_server");

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new NotImplementedException("The specified grant is not implemented.");
        }



        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest()
                          ?? throw new InvalidOperationException("OpenID Connect request mangler.");

            if (!User.Identity.IsAuthenticated)
            {
                return Challenge(
                    authenticationSchemes: IdentityConstants.ApplicationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.Path + Request.QueryString
                    });
            }

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name, Claims.Role);

            var subject = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? throw new InvalidOperationException("UserId claim mangler.");

            identity.SetClaim(Claims.Subject, subject);
            identity.SetClaim(Claims.Name, User.Identity?.Name ?? "Unknown");

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());

            principal.SetDestinations(claim =>
            {
                return claim.Type switch
                {
                    Claims.Subject or Claims.Name => [Destinations.AccessToken, Destinations.IdentityToken],
                    _ => [Destinations.AccessToken]
                };
            });

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }


        [HttpGet("~/connect/logout")]
        public async Task<IActionResult> Logout()
        {
            // Hvis ikke logget ind, giv dem alligevel en redirect hjem
            if (User?.Identity is { IsAuthenticated: false })
            {
                return SignOut(
                    new AuthenticationProperties { RedirectUri = "/" },
                    IdentityConstants.ApplicationScheme);
            }

            // Log ud af ASP.NET Identity (cookie)
            await signInManager.SignOutAsync();

            // Hent OIDC‐logout‐requesten
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request is null)
            {
                return BadRequest("Invalid logout request.");
            }

            // SignOut fire Identity‐cookie og OIDC (fjern session), derefter redirect til klient
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = request.PostLogoutRedirectUri ?? "/"
                },
                IdentityConstants.ApplicationScheme);
        }

    }
}
