using System.Globalization;

namespace NotificationWebApi.Business;

public class AppointmentMessageService : IAppointmentMessageService
{
    public string GenerateMessage(string patientName, string languageCode, DateTimeOffset dateTime)
    {
        patientName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(patientName.Trim());

        var dateFormatted = dateTime.ToString("f", GetCulture(languageCode));

        return languageCode.ToLower() switch
        {
            "es" => $"Hola {patientName}, su cita médica está programada para el {dateFormatted}.",
            "en" => $"Hello {patientName}, your medical appointment is scheduled for {dateFormatted}.",
            _ => throw new ArgumentException("Unsupported language code. Only 'en' and 'es' are supported.")
        };
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