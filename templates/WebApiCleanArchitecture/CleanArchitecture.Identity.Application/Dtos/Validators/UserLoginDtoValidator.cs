using FluentValidation;
using Tenon.FluentValidation.Extensions;

namespace CleanArchitecture.Identity.Application.Dtos.Validators;

public sealed class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
{
    public UserLoginDtoValidator()
    {
        RuleFor(x => x.Account).Required().Length(6, 16);
        RuleFor(x => x.Password).Required().Length(6, 16);
    }
}