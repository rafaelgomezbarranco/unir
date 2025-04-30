using System.Globalization;

namespace NotificationWebApi.Business;

public class AppointmentMessageService : IAppointmentMessageService
{
    public string GenerateMessage(string patientName, string languageCode, DateTimeOffset dateTime)
    {
        patientName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(patientName.Trim());

        var dateFormatted = FormatDate(dateTime, languageCode);

        return languageCode.ToLower() switch
        {
            "es" => $"Hola {patientName}, \nSu cita médica está programada para el {dateFormatted}.",
            "en" => $"Hello {patientName}, \nYour medical appointment is scheduled for {dateFormatted}.",
            _ => throw new ArgumentException("Unsupported language code. Only 'en' and 'es' are supported.")
        };
    }

    private string FormatDate(DateTimeOffset dateTime, string languageCode)
    {
        var culture = GetCulture(languageCode);

        if (languageCode.ToLower() == "es")
        {
            // Example: "01 de mayo de 2025 a las 9:00"
            return dateTime.ToString("dd 'de' MMMM 'de' yyyy 'a las' HH:mm", culture);
        }

        // Example: "Thursday, May 1, 2025 at 09:00"
        return dateTime.ToString("f", culture);
    }

    private CultureInfo GetCulture(string languageCode)
    {
        return languageCode.ToLower() switch
        {
            "es" => new CultureInfo("es-ES"),
            "en" => new CultureInfo("en-US"),
            _ => CultureInfo.InvariantCulture
        };
    }
}