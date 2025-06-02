using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ 加载 .env 文件内容
DotNetEnv.Env.Load();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


// ✅ 构建 JWT 配置从 env 读取
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
var clientUrl = Environment.GetEnvironmentVariable("CLIENT_URL");

// ✅ Google OAuth 配置
var googleClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
var googleClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

// 使用这些变量配置服务
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
})
.AddGoogle(options =>
{
    options.ClientId = googleClientId!;
    options.ClientSecret = googleClientSecret!;
    options.CallbackPath = "/signin-google";
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins(clientUrl!)
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/google-login";
});

var app = builder.Build();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
