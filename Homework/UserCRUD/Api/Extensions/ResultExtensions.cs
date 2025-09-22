using Microsoft.AspNetCore.Mvc;
using UserCRUD.Domain.Utils;

namespace UserCRUD.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Data);
            }

            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
                ErrorType.Forbidden => new ForbiddenObjectResult(result.ErrorMessage),
                ErrorType.Validation => new BadRequestObjectResult(result.ErrorMessage),
                ErrorType.Conflict => new ConflictObjectResult(result.ErrorMessage),
                ErrorType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                _ => new ObjectResult(result.ErrorMessage) 
                { 
                    StatusCode = StatusCodes.Status500InternalServerError 
                }
            };
        }

        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }

            return result.ErrorType switch
            {
                ErrorType.NotFound => new NotFoundObjectResult(result.ErrorMessage),
                ErrorType.Unauthorized => new UnauthorizedObjectResult(result.ErrorMessage),
                ErrorType.Forbidden => new ForbiddenObjectResult(result.ErrorMessage),
                ErrorType.Validation => new BadRequestObjectResult(result.ErrorMessage),
                ErrorType.Conflict => new ConflictObjectResult(result.ErrorMessage),
                ErrorType.BadRequest => new BadRequestObjectResult(result.ErrorMessage),
                _ => new ObjectResult(result.ErrorMessage) 
                { 
                    StatusCode = StatusCodes.Status500InternalServerError 
                }
            };
        }
    }
    public class ForbiddenObjectResult : ObjectResult
    {
        public ForbiddenObjectResult(object? value) : base(value)
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }
    }
}