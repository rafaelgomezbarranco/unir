namespace NotificationWebApi.Business;

public interface IAppointmentMessageService
{
    string GenerateMessage(string patientName, string languageCode, DateTimeOffset dateTime);
}