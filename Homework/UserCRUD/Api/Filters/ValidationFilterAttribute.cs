using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;

namespace UserCRUD.Api.Filters;

public class ValidationFilterAttribute<T> : Attribute, IAsyncActionFilter where T : class
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var argToValidate = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

        if (argToValidate is null)
        {
            context.Result = new BadRequestObjectResult(new
            {
                Message = $"Request body of type {typeof(T).Name} is required"
            });
            return;
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(argToValidate);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                context.Result = new UnprocessableEntityObjectResult(new
                {
                    Title = "Validation failed",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Errors = errors
                });
                return;
            }
        }

        await next();
    }
}