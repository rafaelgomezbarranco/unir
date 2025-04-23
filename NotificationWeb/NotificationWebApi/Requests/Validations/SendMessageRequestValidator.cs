using FluentValidation;

namespace NotificationWebApi.Requests.Validations;

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        var allowedLanguages = new[] { "es", "en" };

        RuleFor(user => user.PatientName).NotEmpty().NotNull().WithMessage("Patient name is required.")
            .MaximumLength(20).WithMessage("Patient name cannot exceed 20 characters.");
        RuleFor(user => user.PhoneNumber).NotEmpty().NotNull().WithMessage("Phone number is required.")
            .Matches(@"^\+?[1-9]\d{1,14}$") // E.164 format (international standard)
            .WithMessage("Phone number is invalid.");
        RuleFor(user => user.LanguageCode).NotEmpty().NotNull().WithMessage("Language code is required.")
            .MaximumLength(20).WithMessage("Language code cannot exceed 2 characters.")
            .Must(code => allowedLanguages.Contains(code)).WithMessage("Language code must be 'es' or 'en'.");
        RuleFor(user => user.DateTime)
            .Must(date => date != default).WithMessage("Date and time are required.")
            .Must(date => date > DateTimeOffset.Now).WithMessage("Date and time must be in the future.");
    }
}