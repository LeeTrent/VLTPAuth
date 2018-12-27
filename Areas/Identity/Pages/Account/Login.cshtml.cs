using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace VLTPAuth.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IEEXAuthService _eexAuthService;

        public LoginModel
        (
          SignInManager<IdentityUser> signInManager,
          ILogger<LoginModel> logger, 
          IEEXAuthService eexAuthService
        )
        {
            _signInManager = signInManager;
            _logger = logger;
            _eexAuthService = eexAuthService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [RegularExpression(@"^\d{9}$",ErrorMessage="9 digits only (no hyphens)")]
            public string SSN { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            _logger.LogInformation("[Login][OnPost] => BEGIN ...");

            returnUrl = returnUrl ?? Url.Content("~/");
            ViewData["EEXAuthFailure"] = null;

            _logger.LogInformation("[Login][OnPost] => ModelState.IsValid: " + ModelState.IsValid);
            
            if (ModelState.IsValid)
            {
              _logger.LogInformation("[Login][OnPost] => _eexAuthService.IsAuthorized: "
                  + _eexAuthService.IsAuthorized(Input.SSN, Input.Password));

              // Authenticate against EEX using SSN and PIN
              if ( _eexAuthService.IsAuthorized(Input.SSN, Input.Password) == false)
              {
                    ModelState.AddModelError(string.Empty, "Invalid SSN or PIN");
                    ViewData["EEXAuthFailure"] = "Authentication was not successful";
                     _logger.LogInformation("[Login][OnPost] => Returning to login page due to unsuccessful EEX authentication.");
                    return Page();                
              }
              _logger.LogInformation("[Login][OnPost] => EEX authentication succeeded - attempting Password Sign-in ...");

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.SSN, Input.Password, Input.RememberMe, lockoutOnFailure: true);

                _logger.LogInformation("[Login][OnPost] => Login succeeded: " + result.Succeeded);
                _logger.LogInformation("[Login][OnPost] => RequiresTwoFactor: " + result.RequiresTwoFactor);
                _logger.LogInformation("[Login][OnPost] => IsLockedOut: " + result.IsLockedOut);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    _logger.LogInformation("[Login][OnPost] => Password Sign-in failed - returning to login page.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
