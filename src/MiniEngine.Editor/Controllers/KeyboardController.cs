﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;

namespace MiniEngine.Editor.Controllers
{
    [Service]
    public sealed class KeyboardController
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

        public KeyboardController()
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

        public bool Pressed(Keys key)
            => this.KeyStates[key] == InputState.JustPressed;

        public bool Held(Keys key)
            => this.KeyStates[key] == InputState.Pressed;

        public bool Released(Keys key)
            => this.KeyStates[key] == InputState.JustReleased;

        internal float AsFloat(InputState state, Keys key)
            => this.KeyStates[key] == state ? 1.0f : 0.0f;

        internal float[] AsArray(InputState state, params Keys[] keys)
        {
            var values = new float[keys.Length];
            for (var i = 0; i < keys.Length; i++)
            {
                values[i] = this.AsFloat(state, keys[i]);
            }

            return values;
        }

        public bool ClickDigit(out int value)
        {
            foreach (var key in this.Digits)
            {
                if (this.Pressed(key.Key))
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
