namespace RaceDay.Api.Helpers
{
    public static class SessionHelper
    {
        public static int? GetUserId(HttpContext context)
        {
            return context.Session.GetInt32("UserId");
        }

        public static string? GetRole(HttpContext context)
        {
            return context.Session.GetString("Role");
        }

        public static bool IsLoggedIn(HttpContext context)
        {
            return GetUserId(context).HasValue;
        }

        public static bool IsOrganiser(HttpContext context)
        {
            return GetRole(context) == "Organiser";
        }

        public static bool IsParticipant(HttpContext context)
        {
            return GetRole(context) == "Participant";
        }
    }
}