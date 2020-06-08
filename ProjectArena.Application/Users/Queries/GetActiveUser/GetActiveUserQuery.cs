using System;
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
using ProjectArena.Domain.Registry.Helpers;
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
                    _gameContext.Rosters.InsertOne(roster);
                    var userCharacter = new Character()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Deleted = false,
                        IsKeyCharacter = true,
                        RosterUserId = roster.UserId,
                        Name = user.ViewName,
                        ChosenTalents = new int[0]
                    };
                    _gameContext.Characters.InsertOne(userCharacter);
                    characters = new Character[] { userCharacter };
                    await _gameContext.ApplyChangesAsync();
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

                // TODO NativeId calculation by talents
                return new ActiveUserDto()
                {
                    Name = user.ViewName,
                    UniqueId = user.UserName,
                    Id = user.Id,
                    Email = user.Email,
                    State = statusInBattle ? UserState.Battle : UserState.Lobby,
                    Experience = roster.Experience,
                    Tavern = tavern,
                    Roster = characters.Select(x =>
                    {
                        return new CharacterDto()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsKeyCharacter = x.IsKeyCharacter,
                            Talents = x.ChosenTalents.Select(k =>
                            {
                                var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(k);
                                return new PointDto()
                                {
                                    X = coordinates.x,
                                    Y = coordinates.y
                                };
                            })
                        };
                    }),
                    TalentsMap = talents.Select(x =>
                    {
                        var position = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                        return new TalentNodeDto()
                        {
                            X = position.x,
                            Y = position.y,
                            Name = x.Name,
                            Description = x.UniqueDescription,
                            Id = x.Id,
                            Speed = x.SpeedModifier,
                            Strength = x.StrengthModifier,
                            Willpower = x.WillpowerModifier,
                            Constitution = x.ConstitutionModifier,
                            Class = x.Class,
                            ClassPoints = x.ClassPoints,
                            Prerequisites = x.Prerequisites,
                            Exceptions = x.Exceptions,
                            SkillsAmount = x.SkillsAmount
                        };
                    })
                };
            }
        }
    }
}