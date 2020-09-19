using System;
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

        private const int TimeBeforeEndTurn = -3;

        public event Action<OutcomingMessage> OutcomingMessagesEvent;

        public event Action<string> SynchronizationErrorEvent;

        private readonly object _m = new object();

        private readonly Random _random = new Random();

        private ISceneStateProvider State { get; set; }

        private int Version { get; set; }

        private string CurrentCode { get; set; }

        private bool WaitingSynchronization { get; set; }

        public Scene (
            ISceneStateProvider state)
        {
           State = state;
        }

        private bool CheckIncomingCode(string code)
        {
            return true;
        }

        private string Encode(string code)
        {
            return code;
        }

        private void StartGame()
        {
            var state = this.State.RetrieveState();
            state.TurnInfo = new StartTurnInfoDto
            {
                Time = TimeBeforeFirstTurn
            };
            state.ProcessAllActors((_, a) =>
            {
                a.InitiativePosition = (float)((_random.NextDouble() - 0.5) / 100.0) + a.TurnCost;
            });
        }

        public void Update(double time)
        {
            lock (_m)
            {
                // TODO Updating
                var state = this.State.RetrieveState();
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
                    WaitingSynchronization)
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

                CurrentCode = Encode(action.NewCode);
                WaitingSynchronization = true;
                OutcomingMessagesEvent(new OutcomingMessage
                {
                    Message = new SynchronizationMessageDto
                    {
                        Id = Infrastructure.Enums.SynchronizationMessageType.ActionDone,
                        Version = Version,
                        Code = CurrentCode,
                        Action = action.Action
                    },
                    Users = state.Players.Where(p => p.Id != action.UserId).Select(p => p.Id)
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

                if (!CheckIncomingCode(synchronization.Code))
                {
                    SynchronizationErrorEvent(synchronization.UserId);
                    return;
                }

                var state = this.State.RetrieveState();
                state.MergeSynchronizer(synchronization.Synchronizer);
                Version = synchronization.Version;
                WaitingSynchronization = false;
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