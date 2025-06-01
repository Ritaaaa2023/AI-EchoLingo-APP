using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;
  private readonly IConfiguration _configuration;

  public AuthController(
      UserManager<ApplicationUser> userManager,
      SignInManager<ApplicationUser> signInManager,
      IConfiguration configuration)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _configuration = configuration;
  }

  // Step 1: redirect to Google login
  [HttpGet("google-login")]
  public IActionResult GoogleLogin(string returnUrl = "/")
  {
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(
        GoogleDefaults.AuthenticationScheme,
        Url.Action(nameof(GoogleCallback), new { returnUrl }));

    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
  }

  // Step 2: callback from Google
  [HttpGet("google-callback")]
  public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
  {
    var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
    if (externalLoginInfo == null)
    {
      return BadRequest("Failed to retrieve external login info.");
    }

    var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
    var name = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(email))
    {
      return BadRequest("Email not found from Google.");
    }

    // Check if user exists
    var user = await _userManager.FindByEmailAsync(email);
    if (user == null)
    {
      user = new ApplicationUser
      {
        UserName = email,
        Email = email,
        DisplayName = name ?? email,
        EmailConfirmed = true
      };

      var result = await _userManager.CreateAsync(user);
      if (!result.Succeeded)
      {
        return BadRequest("Failed to create new user.");
      }

      await _userManager.AddLoginAsync(user, externalLoginInfo);
    }

    // Sign in the user (optional if only issuing token)
    await _signInManager.SignInAsync(user, isPersistent: false);

    // Issue JWT token
    var token = GenerateJwtToken(user);

    // Redirect back to frontend with token
    return Redirect($"{_configuration["ClientUrl"]}/login-success?token={token}");
  }

  private string GenerateJwtToken(ApplicationUser user)
  {
    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "default-key"));

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddDays(7),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
