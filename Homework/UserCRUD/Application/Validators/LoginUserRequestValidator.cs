using FluentValidation;
using UserCRUD.Application.Models;

namespace UserCRUD.Application.Validators;

public class LoginUserRequestValidator: AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}