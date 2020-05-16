using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectArena.Domain.BattleService;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.Identity;
using ProjectArena.Domain.Registry;
using ProjectArena.Infrastructure.Enums;
using ProjectArena.Infrastructure.Models.Game;
using ProjectArena.Infrastructure.Models.User;

namespace ProjectArena.Application.Users.Queries.GetActiveUser
{
  public class GetActiveUserQuery : IRequest<ActiveUserDto>
    {
        public ClaimsPrincipal User { get; set; }

        private class Handler : IRequestHandler<GetActiveUserQuery, ActiveUserDto>
        {
            private readonly IdentityUserManager _userManager;
            private readonly IBattleService _battleService;
            private readonly GameContext _gameContext;
            private readonly RegistryContext _registryContext;

            public Handler(
                IdentityUserManager userManager,
                IBattleService battleService,
                GameContext gameContext,
                RegistryContext registryContext)
            {
                _userManager = userManager;
                _battleService = battleService;
                _gameContext = gameContext;
                _registryContext = registryContext;
            }

            public async Task<ActiveUserDto> Handle(GetActiveUserQuery request, CancellationToken cancellationToken)
            {
                var user = await _userManager.GetUserAsync(request.User);
                var statusInBattle = _battleService.IsUserInBattle(user.Id);
                var roster = await _gameContext.Rosters.GetOneAsync(x => x.UserId == user.Id);
                IEnumerable<Character> characters;
                if (roster == null)
                {
                    roster = new Roster()
                    {
                        UserId = user.Id,
                        Experience = 0,
                        Seed = user.Id.GetHashCode(),
                        TavernCapacity = 6,
                        BoughtPatrons = new int[0]
                    };
                    await _gameContext.Rosters.InsertOneAtomicallyAsync(roster);
                    characters = new Character[0];
                }
                else
                {
                    characters = await _gameContext.Characters.GetAsync(x => x.RosterUserId == roster.UserId && !x.Deleted);
                }

                var tavern = new List<CharacterForSaleDto>();
                for (int i = 1; i <= roster.TavernCapacity; i++)
                {
                    if (!roster.BoughtPatrons.Any(x => x == i))
                    {
                        tavern.Add(new CharacterForSaleDto()
                        {
                            Id = i
                        });
                    }
                }

                var talents = await _registryContext.TalentMap.GetAsync(x => true);

                return new ActiveUserDto()
                {
                    Name = user.ViewName,
                    UniqueId = user.UserName,
                    Id = user.Id,
                    Email = user.Email,
                    State = statusInBattle ? UserState.Battle : UserState.Lobby,
                    Experience = roster.Experience,
                    Tavern = tavern,
                    Roster = characters.Select(x => new CharacterDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        NativeId = "adventurer"
                    }),
                    TalentsMap = talents.Select(x => new TalentNodeDto()
                    {
                        X = x.Position / 1000,
                        Y = x.Position % 1000,
                        Name = x.Name,
                        Description = x.UniqueDescription,
                        Id = x.Id,
                        Speed = x.SpeedModifier,
                        Strength = x.StrengthModifier,
                        Willpower = x.WillpowerModifier,
                        Constitution = x.ConstitutionModifier,
                        Class = x.Class,
                        ClassPoints = x.ClassPoints,
                        Exceptions = x.Exceptions
                    })
                };
            }
        }
    }
}