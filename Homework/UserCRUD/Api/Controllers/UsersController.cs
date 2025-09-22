using Microsoft.AspNetCore.Mvc;
using UserCRUD.Api.Extensions;
using UserCRUD.Api.Filters;
using UserCRUD.Application.Models;
using UserCRUD.Domain.Abstractions;

namespace UserCRUD.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController: Controller
{
    [HttpPost("create")]
    [ServiceFilter(typeof(ValidationFilterAttribute<CreateUserRequest>))]
    public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserRequest request, 
        [FromServices] IUserService userService)
    {
        var result = await userService.CreateUserAsync(request.Email, request.Password);
        return result.ToActionResult();
    }

    [ServiceFilter(typeof(ValidationFilterAttribute<UpdateUserRequest>))]
    [HttpPost("{id:guid}/update")]
    public async Task<IActionResult> UpdateUserAsync(Guid id, 
        [FromBody] UpdateUserRequest request,
        [FromServices] IUserService userService)
    {
        return (await userService.UpdateUserAsync(id, request.NewEmail, request.NewPassword)).ToActionResult();
    }
    
    [HttpDelete("{id:guid}/delete")]
    public async Task<IActionResult> DeleteUserAsync(Guid id,
        [FromServices] IUserService userService)
    {
        return (await userService.DeleteUserAsync(id)).ToActionResult();
    }

    [ServiceFilter(typeof(ValidationFilterAttribute<LoginUserRequest>))]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request,
        [FromServices] IUserService userService)
    {
        return (await userService.LoginUserAsync(request.Email, request.Password)).ToActionResult();
    }
}