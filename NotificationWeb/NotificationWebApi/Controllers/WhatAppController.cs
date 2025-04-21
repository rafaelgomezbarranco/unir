using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationWebApi.Business.WhatApps;
using NotificationWebApi.Requests;

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
    [HttpPost("send")]
    [ProducesResponseType(200, Type = typeof(bool))]
    public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
    {
        var response = await _whatAppNotification.SendWhatApp(string.Empty, string.Empty);

        return Ok(true);
    }
}