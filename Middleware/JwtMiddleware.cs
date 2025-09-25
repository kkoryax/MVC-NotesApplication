using NoteFeature_App.Helpers;

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
                    // เก็บข้อมูล user ใน context
                    context.Items["UserId"] = JwtHelper.GetUserIdFromToken(token);
                    context.Items["UserEmail"] = JwtHelper.GetUserEmailFromToken(token);
                    context.Items["UserRole"] = JwtHelper.GetUserRoleFromToken(token);
                }
            }

            await _next(context);
        }
    }
}
