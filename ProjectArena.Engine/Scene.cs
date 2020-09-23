using System;
using System.Collections.Generic;
using System.Linq;
using ProjectArena.Engine.Models;
using ProjectArena.Engine.State;
using ProjectArena.Infrastructure.Models.Battle;
using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine
{
    public class Scene
    {
        private const int TimeBeforeFirstTurn = 5;

        private const int TimeBeforeEndTurn = -2;

        private const int TurnTime = 31;

        public event Action<OutcomingMessage> OutcomingMessagesEvent;

        public event Action<string> SynchronizationErrorEvent;

        public Guid SceneId => State.GetSceneId();

        private readonly object _m = new object();

        private readonly Random _random = new Random();

        private ISceneStateProvider State { get; set; }

        private int Version { get; set; }

        private string CurrentCode { get; set; }

        // TODO Resend message on too much time
        private SynchronizationMessageDto WaitingSynchronizationMessage { get; set; }

        public Scene (
            ISceneStateProvider state)
        {
            State = state;
        }

        private bool CheckIncomingCode(string code)
        {
            return code == CurrentCode;
        }

        private void StartGame()
        {
            var state = this.State.RetrieveState();
            state.TurnInfo = new StartTurnInfoDto
            {
                Time = TimeBeforeFirstTurn
            };
        }

        private void StartTurn()
        {
            var state = this.State.RetrieveState();
            if (state.TurnInfo.TempActor != null)
            {
                var actor = state.FindActorWithParent(state.TurnInfo.TempActor);
            }

            ActorSynchronizationDto nextTurnActor = null;
            foreach (var actor in state.Actors)
            {
                if ((nextTurnActor == null || actor.InitiativePosition < nextTurnActor.InitiativePosition) &&
                    actor.Actions.Any(a => a.RemainedTime <= 0 && !a.Blocked && !a.IsAutomatic))
                {
                    nextTurnActor = actor;
                }
            }

            state.TurnInfo.TempActor = nextTurnActor.Reference;
            state.TurnInfo.Time = TurnTime;

            CurrentCode = Guid.NewGuid().ToString();
            WaitingSynchronizationMessage = new SynchronizationMessageDto
            {
                Id = Infrastructure.Enums.SynchronizationMessageType.TurnStarted,
                Version = Version,
                Code = CurrentCode,
                StartTurnInfo = state.TurnInfo
            };

            OutcomingMessagesEvent(new OutcomingMessage
            {
                Message = WaitingSynchronizationMessage,
                Users = state.Players.Where(p => p.BattlePlayerStatus != Infrastructure.Enums.PlayerStatus.Playing).Select(p => p.Id).ToList()
            });
        }

        private RewardInfoDto CalculateReward(PlayerState player)
        {
            return new RewardInfoDto
            {
                Experience = player.BattlePlayerStatus == Infrastructure.Enums.PlayerStatus.Defeated ? 1 : 4
            };
        }

        private void CheckVictoryConditions(SceneState state)
        {
            var activePlayers = state.Players.Where(p => p.BattlePlayerStatus == Infrastructure.Enums.PlayerStatus.Playing).ToList();

            foreach (var player in activePlayers)
            {
                var actors = state.GetAllPlayerActors(player.Id).Where(a => player.KeyActorIds.Contains(a.Reference.Id)).ToList();
                if (actors.Count == 0)
                {
                    player.BattlePlayerStatus = Infrastructure.Enums.PlayerStatus.Defeated;
                    OutcomingMessagesEvent(new OutcomingMessage
                    {
                        Message = new SynchronizationMessageDto
                        {
                            Id = Infrastructure.Enums.SynchronizationMessageType.Defeated,
                            Version = Version,
                            Code = CurrentCode,
                            Reward = CalculateReward(player)
                        },
                        Users = new[] { player.Id }
                    });
                }
            }

            var remainingPlayers = activePlayers.Where(p => p.BattlePlayerStatus == Infrastructure.Enums.PlayerStatus.Playing).ToList();

            if (remainingPlayers.Count == 1)
            {
                remainingPlayers[0].BattlePlayerStatus = Infrastructure.Enums.PlayerStatus.Victorious;
                OutcomingMessagesEvent(new OutcomingMessage
                {
                    Message = new SynchronizationMessageDto
                    {
                        Id = Infrastructure.Enums.SynchronizationMessageType.Victorious,
                        Version = Version,
                        Code = CurrentCode,
                        Reward = CalculateReward(remainingPlayers[0])
                    },
                    Users = new[] { remainingPlayers[0].Id }
                });
            }
        }

        private void NextAutomaticAction(SceneState state)
        {
            var action = state.FindNextAutomaticAction();
            if (action.action != null)
            {
                CurrentCode = Guid.NewGuid().ToString();
                WaitingSynchronizationMessage = new SynchronizationMessageDto
                {
                    Id = Infrastructure.Enums.SynchronizationMessageType.ActionDone,
                    Version = Version,
                    Code = CurrentCode,
                    Action = new ActionInfoDto
                    {
                        Actor = action.actor.Reference,
                        Id = action.action.Id,
                        Type = Infrastructure.Enums.ActionType.Targeted,
                        X = action.actor.Reference.X,
                        Y = action.actor.Reference.Y
                    }
                };
                OutcomingMessagesEvent(new OutcomingMessage
                {
                    Message = WaitingSynchronizationMessage,
                    Users = state.Players.Where(p => p.BattlePlayerStatus == Infrastructure.Enums.PlayerStatus.Playing).Select(p => p.Id).ToList()
                });
            }
            else
            {
                StartTurn();
            }
        }

        public void Update(double time)
        {
            lock (_m)
            {
                // TODO Updating
                var state = this.State.RetrieveState();

                state.TurnInfo.Time -= time;

                State.PushNewState(state);

                if (WaitingSynchronizationMessage == null && state.TurnInfo.Time < TimeBeforeEndTurn)
                {
                    NextAutomaticAction(state);
                }
            }
        }

        public void PushAction(IncomingAction action)
        {
            lock (_m)
            {
                if (Version >= action.Version)
                {
                    return;
                }

                if (!CheckIncomingCode(action.Code) ||
                    WaitingSynchronizationMessage != null)
                {
                    SynchronizationErrorEvent(action.UserId);
                    return;
                }

                var state = this.State.RetrieveState();

                if (state.TurnInfo.Time < TimeBeforeEndTurn ||
                    state.TurnInfo.TempActor.Id != action.Action.Actor.Id)
                {
                    SynchronizationErrorEvent(action.UserId);
                    return;
                }

                CurrentCode = action.NewCode;
                WaitingSynchronizationMessage = new SynchronizationMessageDto
                {
                    Id = Infrastructure.Enums.SynchronizationMessageType.ActionDone,
                    Version = Version,
                    Code = CurrentCode,
                    Action = action.Action
                };
                OutcomingMessagesEvent(new OutcomingMessage
                {
                    Message = WaitingSynchronizationMessage,
                    Users = state.Players.Where(p => p.Id.ToString() != action.UserId && p.BattlePlayerStatus == Infrastructure.Enums.PlayerStatus.Playing).Select(p => p.Id).ToList()
                });
            }
        }

        public void PushSynchronization(IncomingSynchronization synchronization)
        {
            lock (_m)
            {
                if (Version >= synchronization.Version)
                {
                    return;
                }

                if (!CheckIncomingCode(synchronization.Code) ||
                    WaitingSynchronizationMessage == null)
                {
                    SynchronizationErrorEvent(synchronization.UserId);
                    return;
                }

                var state = State.RetrieveState();
                state.MergeSynchronizer(synchronization.Synchronizer);
                Version = synchronization.Version;
                WaitingSynchronizationMessage = null;

                CheckVictoryConditions(state);

                State.PushNewState(state);

                if (state.TurnInfo.Time < TimeBeforeEndTurn)
                {
                    NextAutomaticAction(state);
                }
            }
        }

        public void Leave(string playerId)
        {
            lock (_m)
            {
                var state = this.State.RetrieveState();
            }
        }

        public static Scene StartGame(
            ISceneStateProvider state,
            Action<OutcomingMessage> outcomingMessageProcessor,
            Action<string> synchronizationErrorProcessor)
        {
            var scene = new Scene(state);
            scene.SynchronizationErrorEvent += synchronizationErrorProcessor;
            scene.OutcomingMessagesEvent += outcomingMessageProcessor;
            scene.StartGame();
            return scene;
        }
    }
}