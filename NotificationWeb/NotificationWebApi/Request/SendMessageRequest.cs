namespace NotificationWebApi.Request
{
    public class SendMessageRequest
    {
        public required string PatientName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string LanguageCode { get; set; }
        public required DateTimeOffset DateTime { get; set; }
    }
}
