using NoteFeature_App.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace NoteFeature_App.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Cookies["jwt_token"];

            if (!string.IsNullOrEmpty(token))
            {
                if (JwtHelper.ValidateToken(token, out var principal))
                {
                    var userId = JwtHelper.GetUserIdFromToken(token);
                    var userEmail = JwtHelper.GetUserEmailFromToken(token);
                    var userRole = JwtHelper.GetUserRoleFromToken(token);

                    context.Items["UserId"] = userId;
                    context.Items["UserEmail"] = userEmail;
                    context.Items["UserRole"] = userRole;

                    var claims = new List<Claim>();

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userId));
                    }

                    if (!string.IsNullOrWhiteSpace(userEmail))
                    {
                        claims.Add(new Claim(ClaimTypes.Email, userEmail));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Email, userEmail));
                    }

                    if (!string.IsNullOrWhiteSpace(userRole))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var identity = new ClaimsIdentity(claims, "Jwt", ClaimTypes.Email, ClaimTypes.Role);
                    context.User = new ClaimsPrincipal(identity);
                }
            }

            await _next(context);
        }
    }
}
