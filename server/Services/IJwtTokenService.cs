namespace server.Services
{
  public interface IJwtTokenService
  {
    string GenerateToken(ApplicationUser user);
  }
}