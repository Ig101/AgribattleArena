using System;
using FluentValidation;

namespace ProjectArena.Application.Users.Commands.SendPasswordChangeVerification
{
  public class SendPasswordChangeVerificationCommandValidator : AbstractValidator<SendPasswordChangeVerificationCommand>
  {
    public SendPasswordChangeVerificationCommandValidator()
    {
      RuleFor(x => x.Email).NotEmpty();
    }
  }
}