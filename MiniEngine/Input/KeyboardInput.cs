using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace MiniEngine.Input
{
    public class KeyboardInput
    {
        private readonly Keys[] Keys;
        private readonly Dictionary<Keys, KeyState> KeyStates;

        public KeyboardInput()
        {
            this.Keys = Enum.GetValues(typeof (Keys)).OfType<Keys>().ToArray();
            this.KeyStates = new Dictionary<Keys, KeyState>();

            foreach (var key in this.Keys)
            {
                this.KeyStates.Add(key, KeyState.Released);
            }
        }

        public void Update()
        {
            var current = Keyboard.GetState();

            foreach (var key in this.Keys)
            {
                var oldState = this.KeyStates[key];

                switch (oldState)
                {
                    case KeyState.JustPressed:
                        if (current.IsKeyDown(key))
                        {
                            this.KeyStates[key] = KeyState.Pressed;
                        }
                        else if (current.IsKeyUp(key))
                        {
                            this.KeyStates[key] = KeyState.JustReleased;
                        }
                        break;
                    case KeyState.Pressed:
                        if (current.IsKeyUp(key))
                        {
                            this.KeyStates[key] = KeyState.JustReleased;
                        }
                        break;
                    case KeyState.JustReleased:
                        if (current.IsKeyDown(key))
                        {
                            this.KeyStates[key] = KeyState.JustPressed;
                        }
                        else if (current.IsKeyUp(key))
                        {
                            this.KeyStates[key] = KeyState.Released;
                        }
                        break;
                    case KeyState.Released:
                        if (current.IsKeyDown(key))
                        {
                            this.KeyStates[key] = KeyState.JustPressed;
                        }                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }             
            }
        }

        public bool Click(Keys key)
        {
            return this.KeyStates[key] == KeyState.JustPressed;
        }

        public bool Hold(Keys key)
        {
            var state = this.KeyStates[key];
            return state == KeyState.JustPressed || state == KeyState.Pressed;
        }


        private enum KeyState
        {                        
            JustPressed,
            Pressed,
            JustReleased,
            Released
        }
    }
}
