using FluentValidation;

namespace ProjectArena.Application.Users.Commands.VerifyEmail
{
  public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
  {
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
    }
  }
}