using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationWebApi.Business.SMSs;
using NotificationWebApi.Controllers;
using NotificationWebApi.Requests;
using NotificationWebApi.Requests.Validations;
using NotificationWebApi.Responses;

namespace NotificationWebApi.Tests.ControllersTests;

public class SMSControllerTests
{

    [Fact]
    public void GivenNullISMSNotification_WhenConstructingController_ThenThrowsArgumentNullException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new SMSController(null!, new SendMessageRequestValidator())
        );

        Assert.Equal("smsNotification", exception.ParamName);
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendIsCalled_ThenReturnsOkWithTrueAndCallsSendSMS()
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();

        mockSmsNotification
            .Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = "+34654321789",
            LanguageCode = "es",
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SendMessageResponse>(okResult.Value);
        Assert.True(response.IsMessageSent);
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendIsCalled_ThenReturnsOkWithFalseAndCallsSendSMS()
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();
        mockSmsNotification
            .Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = "+34654321789",
            LanguageCode = "es",
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SendMessageResponse>(okResult.Value);
        Assert.False(response.IsMessageSent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GivenInvalidPatientName_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendSMS(string patientName)
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = patientName,
            PhoneNumber = "+34654321789",
            LanguageCode = "es",
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.PatientName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("phone")]
    public async Task GivenInvalidPhoneNumber_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendSMS(string phoneNumber)
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = phoneNumber,
            LanguageCode = "es",
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.PhoneNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xy")]
    public async Task GivenInvalidLanguageCode_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendSMS(string languageCode)
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = "+34654321789",
            LanguageCode = languageCode,
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.LanguageCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1990/01/01T00:00:00.000")]
    [InlineData("01/01/0001 00:00:00 +00:00")]
    public async Task GivenInvalidDateTime_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendSMS(DateTimeOffset dateTime)
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = "+34654321789",
            LanguageCode = "es",
            DateTime = dateTime
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.DateTime));
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendSMSThrowsException_ThenReturnsInternalServerError()
    {
        // Arrange
        var mockSmsNotification = new Mock<ISMSNotification>();
        var validator = new SendMessageRequestValidator();
        mockSmsNotification
            .Setup(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Simulated failure"));

        var sut = new SMSController(mockSmsNotification.Object, validator);

        var request = new SendMessageRequest()
        {
            PatientName = "test",
            PhoneNumber = "+34654321789",
            LanguageCode = "es",
            DateTime = DateTimeOffset.UtcNow.AddDays(1)
        };

        // Act
        var result = await sut.Send(request);

        // Assert
        mockSmsNotification.Verify(x => x.SendSMS(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        Assert.Equal("Internal server error.", internalServerErrorResult.Value);
    }
}
