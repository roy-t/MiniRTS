using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MiniEngine.Input
{
    public sealed class KeyboardInput
    {
        private readonly Keys[] Keys;
        private readonly Dictionary<Keys, InputState> KeyStates;

        public KeyboardInput()
        {
            this.Keys = Enum.GetValues(typeof (Keys)).OfType<Keys>().ToArray();
            this.KeyStates = new Dictionary<Keys, InputState>();

            foreach (var key in this.Keys)
            {
                this.KeyStates.Add(key, InputState.Released);
            }
        }

        public void Update()
        {
            var current = Keyboard.GetState();

            foreach (var key in this.Keys)
            {
                var oldState = this.KeyStates[key];
                var isDown = current.IsKeyDown(key);

                switch (oldState)
                {
                    case InputState.JustPressed:
                        if (isDown)
                        {
                            this.KeyStates[key] = InputState.Pressed;
                        }
                        else
                        {
                            this.KeyStates[key] = InputState.JustReleased;
                        }
                        break;
                    case InputState.Pressed:
                        if (!isDown)
                        {
                            this.KeyStates[key] = InputState.JustReleased;
                        }
                        break;
                    case InputState.JustReleased:
                        if (isDown)
                        {
                            this.KeyStates[key] = InputState.JustPressed;
                        }
                        else
                        {
                            this.KeyStates[key] = InputState.Released;
                        }
                        break;
                    case InputState.Released:
                        if (isDown)
                        {
                            this.KeyStates[key] = InputState.JustPressed;
                        }                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }             
            }
        }

        public bool Click(Keys key)
        {
            return this.KeyStates[key] == InputState.JustPressed;
        }

        public bool Hold(Keys key)
        {
            var state = this.KeyStates[key];
            return state == InputState.JustPressed || state == InputState.Pressed;
        }
    }
}
