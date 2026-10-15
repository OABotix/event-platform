namespace RaceDay.Api.Helpers
{
    public static class SessionHelper
    {
        public static int? GetUserId(HttpContext context) => context.Session.GetInt32("UserId");
        public static string? GetRole(HttpContext context) => context.Session.GetString("Role");
        public static bool IsLoggedIn(HttpContext context) => GetUserId(context).HasValue;
        public static bool IsOrganiser(HttpContext context) => GetRole(context) == "Organiser";
        public static bool IsParticipant(HttpContext context) => GetRole(context) == "Participant";
    }
}