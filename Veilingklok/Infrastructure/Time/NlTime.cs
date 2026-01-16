using System;

namespace Veilingklok.Infrastructure.Time
{
    public static class NlTime
    {
        // Windows timezone id (werkt op Azure App Service Windows)
        private static readonly TimeZoneInfo Tz =
            TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

        public static DateTime Now() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Tz);

        public static DateTime Today() => Now().Date;
    }
}
