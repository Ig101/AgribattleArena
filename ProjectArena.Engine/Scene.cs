using System;
using ProjectArena.Engine.Models;
using ProjectArena.Engine.State;
using ProjectArena.Infrastructure.Models.Battle;
using ProjectArena.Infrastructure.Models.Battle.Incoming;

namespace ProjectArena.Engine
{
    public class Scene
    {
        private const int TimeBeforeFirstTurn = 5;

        public event Action<SynchronizationMessageDto> OutcomingMessagesEvent;

        public event Action<string> SynchronizationErrorEvent;

        private readonly object _m = new object();

        private readonly Random _random = new Random();

        private ISceneStateProvider State { get; set; }

        private int Version { get; set; }

        private string CurrentCode { get; set; }

        public Scene (
            ISceneStateProvider state)
        {
           State = state;
        }

        private bool CheckIncomingCode(string code)
        {
            return true;
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
                // TODO Check for version and code
                var state = this.State.RetrieveState();
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
            Action<SynchronizationMessageDto> outcomingMessageProcessor,
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