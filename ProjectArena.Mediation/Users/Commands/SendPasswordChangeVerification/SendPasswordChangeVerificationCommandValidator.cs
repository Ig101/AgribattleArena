using System;
using FluentValidation;

namespace ProjectArena.Mediation.Users.Commands.SendPasswordChangeVerification
{
  public class SendPasswordChangeVerificationCommandValidator : AbstractValidator<SendPasswordChangeVerificationCommand>
  {
    public SendPasswordChangeVerificationCommandValidator()
    {
      RuleFor(x => x.Email).NotEmpty();
    }
  }
}