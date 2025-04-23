namespace NotificationWebApi.Requests;

public class SendMessageRequest
{
    public string PatientName { get; set; }
    public string PhoneNumber { get; set; }
    public string LanguageCode { get; set; }
    public DateTimeOffset DateTime { get; set; }
}