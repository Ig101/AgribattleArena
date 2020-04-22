using FluentValidation;

namespace ProjectArena.Mediation.Users.Commands.VerifyEmail
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