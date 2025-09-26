namespace NoteFeature_App.Helpers
{
    public static class AuthHelper
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Items["UserId"]?.ToString());
        }

        public static string? GetCurrentUserId(HttpContext context)
        {
            return context.Items["UserId"]?.ToString();
        }

        public static string? GetCurrentUserEmail(HttpContext context)
        {
            return context.Items["UserEmail"]?.ToString();
        }

        public static string? GetCurrentUserRole(HttpContext context)
        {
            return context.Items["UserRole"]?.ToString();
        }

        public static bool IsAdmin(HttpContext context)
        {
            return GetCurrentUserRole(context) == "Admin";
        }
    }
}
