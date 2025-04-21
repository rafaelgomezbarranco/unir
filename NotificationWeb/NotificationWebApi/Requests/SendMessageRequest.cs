using NotificationWebApi.Requests.Validations;
using System.ComponentModel.DataAnnotations;

namespace NotificationWebApi.Requests;

public class SendMessageRequest
{
    [Required(ErrorMessage = "The patient name is required.")]
    public string PatientName { get; set; }

    [Required(ErrorMessage = "The phone number is required.")]
    [Phone(ErrorMessage = "The phone number is not valid.")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "The language code is required.")]
    [ValidLanguageCode(ErrorMessage = "The language code is invalid.")]
    public string LanguageCode { get; set; }

    [Required(ErrorMessage = "The date and time are required.")]
    [FutureDate(ErrorMessage = "The date and time must be in the future")]
    public DateTimeOffset DateTime { get; set; }
}