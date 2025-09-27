using System.IdentityModel.Tokens.Jwt;

namespace Concertation.Banking.API.Features.Auth.Models;

public class TokenResponse
{
    public string Access_Token { get; set; } = string.Empty;
    public string Refresh_Token { get; set; } = string.Empty;
    public int Expires_In { get; set; }
    public string UserId => ExtractUserIdFromToken(Access_Token);

    private string ExtractUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Subject;
    }
}
