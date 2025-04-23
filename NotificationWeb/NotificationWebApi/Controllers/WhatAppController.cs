using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationWebApi.Business.WhatApps;
using NotificationWebApi.Requests;
using NotificationWebApi.Responses;

namespace NotificationWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WhatAppController : ControllerBase
{
    private readonly IWhatAppNotification _whatAppNotification;
    private readonly IValidator<SendMessageRequest> _validator;

    public WhatAppController(IWhatAppNotification whatAppNotification, IValidator<SendMessageRequest> validator)
    {
        _whatAppNotification = whatAppNotification ?? throw new ArgumentNullException(nameof(whatAppNotification));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    /// <summary>
    /// Send an WhatApp notification
    /// </summary>
    /// <returns>true if the message is sent, or false otherwise</returns>
    /// <response code="200">With true or false</response>
    /// <response code="400">For validation errors</response>
    /// <response code="500">For internal server errors</response>
    [HttpPost("send")]
    [ProducesResponseType(200, Type = typeof(SendMessageResponse))]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return BadRequest(result);
        }

        try
        {
            var response = await _whatAppNotification.SendWhatApp(string.Empty, string.Empty);

            var sendMessageResponse = new SendMessageResponse { IsMessageSent = response };

            return Ok(sendMessageResponse);
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error.");
        }
    }
}