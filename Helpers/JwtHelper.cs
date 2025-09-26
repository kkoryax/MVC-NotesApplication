using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NoteFeature_App.Helpers
{
    public static class JwtHelper
    {
        private static readonly string _secretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";

        public static bool ValidateToken(string token, out ClaimsPrincipal? principal)
        {
            principal = null;
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = "NoteFeatureApp",
                    ValidateAudience = true,
                    ValidAudience = "NoteFeatureAppUsers",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string? GetUserIdFromToken(string token)
        {
            if (ValidateToken(token, out var principal))
            {
                return principal?.FindFirst("UserId")?.Value;
            }
            return null;
        }

        public static string? GetUserRoleFromToken(string token)
        {
            if (ValidateToken(token, out var principal))
            {
                return principal?.FindFirst(ClaimTypes.Role)?.Value;
            }
            return null;
        }

        public static string? GetUserEmailFromToken(string token)
        {
            if (ValidateToken(token, out var principal))
            {
                return principal?.FindFirst(ClaimTypes.Name)?.Value;
            }
            return null;
        }
    }
}
