using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationWebApi.Business;
using NotificationWebApi.Business.WhatApps;
using NotificationWebApi.Controllers;
using NotificationWebApi.Requests;
using NotificationWebApi.Requests.Validations;
using NotificationWebApi.Responses;

namespace NotificationWebApi.Tests.ControllersTests;

public class WhatAppControllerTests
{
    [Theory]
    [InlineData("whatAppNotification")]
    [InlineData("validator")]
    [InlineData("appointmentMessageService")]
    public void GivenNullDependencies_WhenConstructingController_ThenThrowsArgumentNullException(string nullParam)
    {
        // Arrange
        var whatAppNotification = nullParam == "whatAppNotification" ? null : new Mock<IWhatAppNotification>().Object;
        var validator = nullParam == "validator" ? null : new SendMessageRequestValidator();
        var appointmentMessageService = nullParam == "appointmentMessageService" ? null : new AppointmentMessageService();

        // Act
        var exception = Assert.Throws<ArgumentNullException>(() =>
            new WhatAppController(
                (IWhatAppNotification?)whatAppNotification!,
                (IValidator<SendMessageRequest>?)validator!,
                (IAppointmentMessageService?)appointmentMessageService!
            )
        );

        // Assert
        Assert.Equal(nullParam, exception.ParamName);
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendIsCalled_ThenReturnsOkWithTrueAndCallsSendWhatApp()
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();
        mockWhatAppNotification
            .Setup(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SendMessageResponse>(okResult.Value);
        Assert.True(response.IsMessageSent);
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendIsCalled_ThenReturnsOkWithFalseAndCallsSendWhatApp()
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();
        mockWhatAppNotification
            .Setup(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<SendMessageResponse>(okResult.Value);
        Assert.False(response.IsMessageSent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task GivenInvalidPatientName_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendWhatApp(string patientName)
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.PatientName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("phone")]
    public async Task GivenInvalidPhoneNumber_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendWhatApp(string phoneNumber)
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.PhoneNumber));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("xy")]
    public async Task GivenInvalidLanguageCode_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendWhatApp(string languageCode)
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.LanguageCode));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1990/01/01T00:00:00.000")]
    [InlineData("01/01/0001 00:00:00 +00:00")]
    public async Task GivenInvalidDateTime_WhenSendIsCalled_ThenReturnsBadRequestAndDoesNotCallSendWhatApp(DateTimeOffset dateTime)
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ValidationResult>(badRequestResult.Value);
        Assert.Contains(response.Errors, e => e.PropertyName == nameof(SendMessageRequest.DateTime));
    }

    [Fact]
    public async Task GivenValidRequest_WhenSendWhatAppThrowsException_ThenReturnsInternalServerError()
    {
        // Arrange
        var mockWhatAppNotification = new Mock<IWhatAppNotification>();
        var validator = new SendMessageRequestValidator();
        var mockMessageService = new Mock<IAppointmentMessageService>();
        mockWhatAppNotification
            .Setup(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Simulated failure"));

        var sut = new WhatAppController(mockWhatAppNotification.Object, validator, mockMessageService.Object);

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
        mockWhatAppNotification.Verify(x => x.SendWhatApp(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

        var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, internalServerErrorResult.StatusCode);
        Assert.Equal("Internal server error.", internalServerErrorResult.Value);
    }
}
