using GameStateSystem;
using NUnit.Framework;

namespace _Project.Tests.EditMode.GameStateTests
{
    public class GameStateTests
    {
        [Test]
        public void IsCurrentGameStateSetAfterInitialize()
        {
            var controller = new GameStateController(GameState.Blasting);

            Assert.AreEqual(GameState.Blasting, controller.currentGameState);
        }

        [Test]
        public void IsEventRaisedAfterCurrentStateChanged()
        {
            var state = GameState.Idle;
            var controller = new GameStateController(state);

            GameStateController.OnCurrentGameStateChanged += (currentState) => state = currentState;

            controller.currentGameState = GameState.Falling;

            Assert.AreEqual(GameState.Falling, state);
        }
    }
}