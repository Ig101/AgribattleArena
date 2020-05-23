using System.Linq;
using FluentValidation;

namespace ProjectArena.Application.Game.Commands.ChangeCharacterTalents
{
  public class ChangeCharacterTalentsCommandValidator : AbstractValidator<ChangeCharacterTalentsCommand>
    {
        public ChangeCharacterTalentsCommandValidator()
        {
            RuleFor(x => x.Changes).Must(x => x != null && x.Count() > 0);
        }
    }
}