using System;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Controllers;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.Utilities
{
    public sealed class DebugController
    {
        private readonly KeyboardInput Keyboard;
        private readonly LightSystemsController LightSystemsController;
        private readonly CameraController CameraController;
        private readonly StateMachine<State, KeyboardInput> StateMachine;

        public DebugController(KeyboardInput keyboard, PerspectiveCamera camera,
                               CameraControllerFactory cameraControllerFactory,
                               LightSystemsControllerFactory lightSystemsControllerFactory)
        {
            this.Keyboard = keyboard;
            this.LightSystemsController = lightSystemsControllerFactory.Build(camera);
            this.CameraController = cameraControllerFactory.Build(camera);

            this.StateMachine = new StateMachine<State, KeyboardInput>(
                State.Movement,
                CreateStateChangeTrigger(State.NoControl, State.Movement, k => k.Click(Keys.M)),
                CreateStateChangeTrigger(State.Movement, State.NoControl, k => k.JustReleased(Keys.Escape)),
                CreateStateChangeTrigger(State.NoControl, State.Light, k => k.Click(Keys.L)),
                CreateStateChangeTrigger(State.Light, State.NoControl, k => k.JustReleased(Keys.Escape)));
        }


        public bool Update(Seconds elapsed)
        {
            this.StateMachine.Update(this.Keyboard);

            switch (this.StateMachine.State)
            {
                case State.NoControl:
                    return false;
                case State.Light:
                    this.LightSystemsController.Update(elapsed);
                    return true;
                case State.Movement:
                    this.CameraController.Update(elapsed);
                    return true;
            }

            return false;
        }

        public string DescribeState()
        {
            return Enum.GetName(typeof(State), this.StateMachine.State);
        }

        private static StateChangeTrigger<State, KeyboardInput> CreateStateChangeTrigger(
            State before,
            State after,
            Func<KeyboardInput, bool> conditionFunc)
        {
            return new StateChangeTrigger<State, KeyboardInput>(before, after, conditionFunc);
        }

        private enum State
        {
            NoControl,
            Light,
            Movement
        }
    }
}
