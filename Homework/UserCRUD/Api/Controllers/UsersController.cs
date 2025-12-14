using Microsoft.AspNetCore.Mvc;
using NotificationService;
using UserCRUD.Api.Extensions;
using UserCRUD.Api.Filters;
using UserCRUD.Application.Models;
using UserCRUD.Domain.Abstractions;

namespace UserCRUD.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(ILogger<UsersController> logger,
    UserNotificationService.UserNotificationServiceClient client): Controller
{
    [HttpPost("create")]
    [ServiceFilter(typeof(ValidationFilterAttribute<CreateUserRequest>))]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request, 
        [FromServices] IUserService userService)
    {
        var result = await userService.CreateUserAsync(request.Email, request.Password);
        if (!result.IsSuccess) return result.ToActionResult();
        var message = (await client.NotifyUserEventAsync(new UserEventRequest
        {
            EventType = EventType.Created,
            UserId = result.Data.Id.ToString()
        })).Message;
        logger.LogInformation(message);
        return result.ToActionResult();
    }

    [ServiceFilter(typeof(ValidationFilterAttribute<UpdateUserRequest>))]
    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> UpdateUserAsync(Guid id, 
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserService userService)
    {
        var result = await userService.UpdateUserAsync(id, request.NewEmail, request.NewPassword);
        if (!result.IsSuccess) return result.ToActionResult();
        var message = (await client.NotifyUserEventAsync(new UserEventRequest
        {
            EventType = EventType.Updated,
            UserId = result.Data.Id.ToString()
        })).Message;
        logger.LogInformation(message);
        return result.ToActionResult();
    }
    
    [HttpDelete("{id:guid}/delete")]
    public async Task<IActionResult> DeleteUserAsync(Guid id,
        [FromServices] IUserService userService)
    {
        var result = await userService.DeleteUserAsync(id);
        if (!result.IsSuccess) return result.ToActionResult();
        var message = (await client.NotifyUserEventAsync(new UserEventRequest
        {
            EventType = EventType.Deleted,
            UserId = id.ToString()
        })).Message;
        logger.LogInformation(message);
        return result.ToActionResult();
    }

    [ServiceFilter(typeof(ValidationFilterAttribute<LoginUserRequest>))]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request,
        [FromServices] IUserService userService)
    {
        var result = await userService.LoginUserAsync(request.Email, request.Password);
        if (!result.IsSuccess) return result.ToActionResult();
        var message = (await client.NotifyUserEventAsync(new UserEventRequest
        {
            EventType = EventType.Created,
            UserId = result.Data.Id.ToString()
        })).Message;
        logger.LogInformation(message);
        return result.ToActionResult();
    }
}