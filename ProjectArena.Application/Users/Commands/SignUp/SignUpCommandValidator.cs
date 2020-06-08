using FluentValidation;

namespace ProjectArena.Application.Users.Commands.SignUp
{
  public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
  {
    public SignUpCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(20);
        RuleFor(x => x.Name).MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty();
    }
  }
}