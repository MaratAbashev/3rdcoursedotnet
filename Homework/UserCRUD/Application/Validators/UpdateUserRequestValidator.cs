using System.Collections.Concurrent;
using FluentValidation;
using UserCRUD.Application.Models;
using UserCRUD.Domain.Models;

namespace UserCRUD.Application.Validators;

public class UpdateUserRequestValidator: AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(ConcurrentDictionary<Guid, CustomUser> userStorage)
    {
        RuleFor(r => r.NewEmail)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email is required")
            .Must(email =>
            {
                return userStorage.Values.All(u => u.Email != email);
            })
            .WithMessage("Email is already used");
        RuleFor(r => r.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,20}$")
            .WithMessage("Password must be 5 to 20 characters long, contain at least 1 latin capital letter, case letter, one number and one special character.");

    }
}