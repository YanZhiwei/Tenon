using FluentValidation;
using Tenon.FluentValidation.Extensions;

namespace CleanArchitecture.Identity.Application.Dtos.Validators;

public sealed class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.Password).Required();
    }
}