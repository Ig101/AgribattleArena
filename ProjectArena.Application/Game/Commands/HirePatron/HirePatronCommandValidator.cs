using FluentValidation;

namespace ProjectArena.Application.Game.Commands.HirePatron
{
    public class HirePatronCommandValidator : AbstractValidator<HirePatronCommand>
  {
    public HirePatronCommandValidator()
    {
        RuleFor(x => x.PatronId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Name).MaximumLength(15);
        RuleFor(x => x.Name).MinimumLength(1);
    }
  }
}