using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ProjectArena.Domain.BattleService.Helpers;
using ProjectArena.Domain.Game;
using ProjectArena.Domain.Game.Entities;
using ProjectArena.Domain.Registry;
using ProjectArena.Domain.Registry.Entities;
using ProjectArena.Domain.Registry.Helpers;
using ProjectArena.Infrastructure.Models.ErrorHandling;
using ProjectArena.Infrastructure.Models.Game;

namespace ProjectArena.Application.Game.Commands.ChangeCharacterTalents
{
    public class ChangeCharacterTalentsCommand : IRequest
    {
        public string UserId { get; set; }

        public string CharacterId { get; set; }

        public IEnumerable<TalentNodeChangeDeclarationDto> Changes { get; set; }

        internal class Handler : IRequestHandler<ChangeCharacterTalentsCommand>
        {
            private const int AddChangeCost = 1;
            private const int RemoveChangeCost = 1;
            private readonly GameContext _gameContext;
            private readonly RegistryContext _registryContext;

            private readonly int _centerX = 24;
            private readonly int _centerY = 12;
            private readonly int _talentsMaxCount = 30;

            public Handler(
                GameContext gameContext,
                RegistryContext registryContext)
            {
                _registryContext = registryContext;
                _gameContext = gameContext;
            }

            public bool FindPath(TalentNode node, IEnumerable<TalentNode> characterTalents, IList<TalentNode> procesedNodes)
            {
                procesedNodes.Add(node);
                var (x, y) = TalentPositionHelper.GetCoordinatesFromPosition(node.Position);
                if ((Math.Abs(x - _centerX) == 1 && y == _centerY) || (Math.Abs(y - _centerY) == 1 && x == _centerX))
                {
                    return true;
                }

                var leftTalent = characterTalents.FirstOrDefault(k =>
                {
                    var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(k.Position);
                    return coordinates.x == x + 1 && coordinates.y == y;
                });
                if (leftTalent != null && !procesedNodes.Contains(leftTalent) && FindPath(leftTalent, characterTalents, procesedNodes))
                {
                    return true;
                }

                var rightTalent = characterTalents.FirstOrDefault(k =>
                {
                    var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(k.Position);
                    return coordinates.x == x - 1 && coordinates.y == y;
                });
                if (rightTalent != null && !procesedNodes.Contains(rightTalent) && FindPath(rightTalent, characterTalents, procesedNodes))
                {
                    return true;
                }

                var topTalent = characterTalents.FirstOrDefault(k =>
                {
                    var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(k.Position);
                    return coordinates.x == x && coordinates.y == y - 1;
                });
                if (topTalent != null && !procesedNodes.Contains(topTalent) && FindPath(topTalent, characterTalents, procesedNodes))
                {
                    return true;
                }

                var bottomTalent = characterTalents.FirstOrDefault(k =>
                {
                    var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(k.Position);
                    return coordinates.x == x && coordinates.y == y + 1;
                });
                if (bottomTalent != null && !procesedNodes.Contains(bottomTalent) && FindPath(bottomTalent, characterTalents, procesedNodes))
                {
                    return true;
                }

                return false;
            }

            public async Task<Unit> Handle(ChangeCharacterTalentsCommand request, CancellationToken cancellationToken)
            {
                var roster = await _gameContext.Rosters.GetOneAsync(x => x.UserId == request.UserId);
                if (roster == null)
                {
                    throw new HttpException()
                    {
                        Error = "User is not found",
                        StatusCode = 404
                    };
                }

                var character = await _gameContext.Characters.GetOneAsync(x => x.Id == request.CharacterId);
                if (character == null)
                {
                    throw new HttpException()
                    {
                        Error = "Character is not found",
                        StatusCode = 404
                    };
                }

                if (character.RosterUserId != request.UserId)
                {
                    throw new HttpException()
                    {
                        Error = "Character is not belong to user",
                        StatusCode = 400
                    };
                }

                var changedTalentPositions = request.Changes.Select(x => TalentPositionHelper.GetPositionFromCoordinates(x.X, x.Y)).ToList();
                var allTalentPositions = character.ChosenTalents.Union(changedTalentPositions).Distinct().ToList();
                var allTalents = await _registryContext.TalentMap.GetAsync(x => allTalentPositions.Contains(x.Position));
                var characterTalents = allTalents.Where(x => character.ChosenTalents.Contains(x.Position)).ToList();

                var strength = BattleHelper.DefaultStrength;
                var willpower = BattleHelper.DefaultWillpower;
                var constitution = BattleHelper.DefaultConstitution;
                var speed = BattleHelper.DefaultSpeed;
                var skillsCount = 1;
                var cost = 0;
                var talentsCount = 0;
                characterTalents.ForEach(x =>
                {
                    strength += x.StrengthModifier;
                    willpower += x.WillpowerModifier;
                    constitution += x.ConstitutionModifier;
                    speed += x.SpeedModifier;
                    skillsCount += x.SkillsAmount;
                    talentsCount++;
                });

                var newTalents = new List<TalentNodeChangeDeclarationDto>();

                foreach (var change in request.Changes)
                {
                    var changePosition = TalentPositionHelper.GetPositionFromCoordinates(change.X, change.Y);

                    // Accessibility check
                    TalentNodeChangeDeclarationDto talent;
                    if ((talent = newTalents.FirstOrDefault(x => x.X == change.X && x.Y == change.Y)) != null)
                    {
                        if (change.State == talent.State)
                        {
                            throw new HttpException()
                            {
                                Error = "Cannot do the same action with talent twice",
                                StatusCode = 400
                            };
                        }

                        cost--;
                        newTalents.Remove(talent);
                    }
                    else
                    {
                        if (characterTalents.Any(x => x.Position == changePosition) == change.State)
                        {
                            throw new HttpException()
                            {
                                Error = "Cannot do action with talent",
                                StatusCode = 400
                            };
                        }

                        cost++;
                        newTalents.Add(change);
                    }

                    var changeTalent = allTalents.First(x => x.Position == changePosition);

                    // ConditionalsCheck
                    if (change.State)
                    {
                        if (!((Math.Abs(change.X - _centerX) == 1 && change.Y == _centerY) || (Math.Abs(change.Y - _centerY) == 1 && change.X == _centerX)) &&
                        !characterTalents.Any(x =>
                        {
                            var talentCoordinates = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                            return (Math.Abs(change.X - talentCoordinates.x) == 1 && change.Y == talentCoordinates.y) ||
                                (Math.Abs(change.Y - talentCoordinates.y) == 1 && change.X == talentCoordinates.x);
                        }))
                        {
                            throw new HttpException()
                            {
                                Error = "Cannot pick remote talent",
                                StatusCode = 400
                            };
                        }

                        if (changeTalent.Exceptions != null && characterTalents.Any(x => changeTalent.Exceptions.Contains(x.Id)))
                        {
                            throw new HttpException()
                            {
                                Error = "Cannot add talent with unprocessible exceptions",
                                StatusCode = 400
                            };
                        }

                        if (changeTalent.Prerequisites != null && changeTalent.Prerequisites.Any(x => !characterTalents.Any(t => t.Id == x)))
                        {
                            throw new HttpException()
                            {
                                Error = "Cannot add talent with unresolved prerequisites",
                                StatusCode = 400
                            };
                        }

                        talentsCount++;
                        strength += changeTalent.StrengthModifier;
                        willpower += changeTalent.WillpowerModifier;
                        constitution += changeTalent.ConstitutionModifier;
                        speed += changeTalent.SpeedModifier;
                        skillsCount += changeTalent.SkillsAmount;

                        characterTalents.Add(changeTalent);
                    }
                    else
                    {
                        talentsCount--;
                        strength -= changeTalent.StrengthModifier;
                        willpower -= changeTalent.WillpowerModifier;
                        constitution -= changeTalent.ConstitutionModifier;
                        speed -= changeTalent.SpeedModifier;
                        skillsCount -= changeTalent.SkillsAmount;

                        characterTalents.Remove(characterTalents.First(x => x.Position == changePosition));

                        var leftTalent = characterTalents.FirstOrDefault(x =>
                        {
                            var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                            return coordinates.x == change.X + 1 && coordinates.y == change.Y;
                        });
                        var rightTalent = characterTalents.FirstOrDefault(x =>
                        {
                            var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                            return coordinates.x == change.X - 1 && coordinates.y == change.Y;
                        });
                        var topTalent = characterTalents.FirstOrDefault(x =>
                        {
                            var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                            return coordinates.x == change.X && coordinates.y == change.Y - 1;
                        });
                        var bottomTalent = characterTalents.FirstOrDefault(x =>
                        {
                            var coordinates = TalentPositionHelper.GetCoordinatesFromPosition(x.Position);
                            return coordinates.x == change.X && coordinates.y == change.Y + 1;
                        });
                        if ((leftTalent != null && !FindPath(leftTalent, characterTalents, new List<TalentNode>())) ||
                            (rightTalent != null && !FindPath(rightTalent, characterTalents, new List<TalentNode>())) ||
                            (topTalent != null && !FindPath(topTalent, characterTalents, new List<TalentNode>())) ||
                            (bottomTalent != null && !FindPath(bottomTalent, characterTalents, new List<TalentNode>())))
                        {
                            throw new HttpException()
                            {
                                Error = "Remains talents without bonds",
                                StatusCode = 400
                            };
                        }
                    }

                    if (talentsCount > _talentsMaxCount)
                    {
                        throw new HttpException()
                        {
                            Error = "Extra talents",
                            StatusCode = 400
                        };
                    }

                    if (cost > roster.Experience)
                    {
                        throw new HttpException()
                        {
                            Error = "Not enough money",
                            StatusCode = 400
                        };
                    }

                    if (strength < 5 || willpower < 5 || constitution < 5 || speed < 5)
                    {
                        throw new HttpException()
                        {
                            Error = "Stats cannot be less than five",
                            StatusCode = 400
                        };
                    }

                    if (skillsCount > 5)
                    {
                        throw new HttpException()
                        {
                            Error = "Skills amount cannot be more than 5",
                            StatusCode = 400
                        };
                    }
                }

                foreach (var change in newTalents)
                {
                    if (change.State)
                    {
                        _gameContext.Characters.Update(
                            x => x.Id == character.Id,
                            Builders<Character>.Update.Push(k => k.ChosenTalents, TalentPositionHelper.GetPositionFromCoordinates(change.X, change.Y)));
                    }
                    else
                    {
                        _gameContext.Characters.Update(
                            x => x.Id == character.Id,
                            Builders<Character>.Update.Pull(k => k.ChosenTalents, TalentPositionHelper.GetPositionFromCoordinates(change.X, change.Y)));
                    }
                }

                _gameContext.Rosters.Update(
                    x => x.UserId == roster.UserId,
                    Builders<Roster>.Update.Set(k => k.Experience, roster.Experience - cost));

                await _gameContext.ApplyChangesAsync();

                return Unit.Value;
            }
        }
    }
}