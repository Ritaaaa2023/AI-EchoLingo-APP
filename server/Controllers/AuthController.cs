using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using server.Services;
using System.Security.Claims;

namespace server.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _jwtTokenService = jwtTokenService;
      _configuration = configuration;
    }

    // ✅ 注册
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
      var existingUser = await _userManager.FindByEmailAsync(dto.Email);
      if (existingUser != null)
        return BadRequest("User already exists.");

      var user = new ApplicationUser
      {
        UserName = dto.Email,
        Email = dto.Email,
        DisplayName = dto.DisplayName ?? dto.Email,
        EmailConfirmed = true
      };

      var result = await _userManager.CreateAsync(user, dto.Password);
      if (!result.Succeeded)
        return BadRequest(result.Errors);

      return Ok("User registered successfully.");
    }

    // ✅ 登录
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
      var user = await _userManager.FindByEmailAsync(dto.Email);
      if (user == null)
        return Unauthorized("Invalid email or password.");

      var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
      if (!result.Succeeded)
        return Unauthorized("Invalid email or password.");

      var token = _jwtTokenService.GenerateToken(user);
      return Ok(new { token });
    }

    // ✅ Google 登录跳转
    [HttpGet("google-login")]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
      var properties = _signInManager.ConfigureExternalAuthenticationProperties(
          GoogleDefaults.AuthenticationScheme,
          Url.Action(nameof(GoogleCallback), new { returnUrl }));

      return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // ✅ Google 回调处理
    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
    {
      var info = await _signInManager.GetExternalLoginInfoAsync();
      if (info == null)
        return BadRequest("Failed to retrieve external login info.");

      var email = info.Principal.FindFirstValue(ClaimTypes.Email);
      var name = info.Principal.FindFirstValue(ClaimTypes.Name);

      if (string.IsNullOrEmpty(email))
        return BadRequest("Email not found from Google.");

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

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
          return BadRequest("Failed to create user.");

        await _userManager.AddLoginAsync(user, info);
      }

      var token = _jwtTokenService.GenerateToken(user);

      return Redirect($"{_configuration["ClientUrl"]}/login-success?token={token}");
    }
  }
}
