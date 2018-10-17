using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Input
{
    public sealed class KeyboardInput
    {
        private readonly Dictionary<Keys, int> Digits = new Dictionary<Keys, int>
        {
            {Microsoft.Xna.Framework.Input.Keys.D1, 1},
            {Microsoft.Xna.Framework.Input.Keys.D2, 2},
            {Microsoft.Xna.Framework.Input.Keys.D3, 3},
            {Microsoft.Xna.Framework.Input.Keys.D4, 4},
            {Microsoft.Xna.Framework.Input.Keys.D5, 5},
            {Microsoft.Xna.Framework.Input.Keys.D6, 6},
            {Microsoft.Xna.Framework.Input.Keys.D7, 7},
            {Microsoft.Xna.Framework.Input.Keys.D8, 8},
            {Microsoft.Xna.Framework.Input.Keys.D9, 9},
            {Microsoft.Xna.Framework.Input.Keys.D0, 0},
        };

        private readonly Keys[] Keys;
        private readonly Dictionary<Keys, InputState> KeyStates;

        public KeyboardInput()
        {
            this.Keys = Enum.GetValues(typeof(Keys)).OfType<Keys>().ToArray();
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

        public bool JustReleased(Keys key)
        {
            var state = this.KeyStates[key];
            return state == InputState.JustReleased;
        }

        public bool AnyKeyPressedExcept(Keys key)
        {
            if (this.KeyStates[key] == InputState.JustPressed ||
                this.KeyStates[key] == InputState.Pressed)
            {
                return false;
            }

            return this.KeyStates.Any(kv => kv.Value != InputState.Released);
        }

        public bool ClickDigit(out int value)
        {
            foreach (var key in this.Digits)
            {
                if (Click(key.Key))
                {
                    value = key.Value;
                    return true;
                }
            }

            value = 0;
            return false;
        }
    }
}
