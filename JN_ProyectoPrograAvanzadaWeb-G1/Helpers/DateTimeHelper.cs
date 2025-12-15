using System;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Helpers
{
    public static class DateTimeHelper
    {
        
        public static DateTime ToCostaRicaTime(this DateTime utcDateTime)
        {
            try
            {
                
                var costaRicaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, costaRicaTimeZone);
            }
            catch
            {
                
                return utcDateTime.AddHours(-6);
            }
        }

        
        public static DateTime? ToCostaRicaTime(this DateTime? utcDateTime)
        {
            if (!utcDateTime.HasValue)
                return null;

            return utcDateTime.Value.ToCostaRicaTime();
        }
    }
}

