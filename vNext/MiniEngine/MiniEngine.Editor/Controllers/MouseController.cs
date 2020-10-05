using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;

namespace MiniEngine.Editor.Controllers
{
    [Service]
    public sealed class MouseController
    {
        private readonly MouseButtons[] Buttons;
        private readonly Dictionary<MouseButtons, InputState> ButtonStates;

        private float scrollWheelValue;

        private ScrollDirection scrollDirection;

        public MouseController()
        {
            this.Buttons = Enum.GetValues(typeof(MouseButtons)).OfType<MouseButtons>().ToArray();
            this.ButtonStates = new Dictionary<MouseButtons, InputState>();

            foreach (var button in this.Buttons)
            {
                this.ButtonStates.Add(button, InputState.Released);
            }

            this.Position = Point.Zero;
        }

        public void Update()
        {
            var current = Mouse.GetState();

            this.Movement = current.Position - this.Position;
            this.Position = current.Position;


            if (current.ScrollWheelValue > this.scrollWheelValue)
            {
                this.scrollDirection = ScrollDirection.Up;
            }
            else if (current.ScrollWheelValue < this.scrollWheelValue)
            {
                this.scrollDirection = ScrollDirection.Down;
            }
            else
            {
                this.scrollDirection = ScrollDirection.None;
            }

            this.scrollWheelValue = current.ScrollWheelValue;

            foreach (var button in this.Buttons)
            {
                var oldState = this.ButtonStates[button];

                var isDown = IsButtonDown(button, current);

                switch (oldState)
                {
                    case InputState.JustPressed:
                        if (isDown)
                        {
                            this.ButtonStates[button] = InputState.Pressed;
                        }
                        else
                        {
                            this.ButtonStates[button] = InputState.JustReleased;
                        }
                        break;
                    case InputState.Pressed:
                        if (!isDown)
                        {
                            this.ButtonStates[button] = InputState.JustReleased;
                        }
                        break;
                    case InputState.JustReleased:
                        if (isDown)
                        {
                            this.ButtonStates[button] = InputState.JustPressed;
                        }
                        else
                        {
                            this.ButtonStates[button] = InputState.Released;
                        }
                        break;
                    case InputState.Released:
                        if (isDown)
                        {
                            this.ButtonStates[button] = InputState.JustPressed;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Point Movement { get; private set; }

        public Point Position { get; private set; }

        public bool ScrolledUp => this.scrollDirection == ScrollDirection.Up;
        public bool ScrolledDown => this.scrollDirection == ScrollDirection.Down;

        public bool Pressed(MouseButtons button)
            => this.ButtonStates[button] == InputState.JustPressed;

        public bool Held(MouseButtons button)
            => this.ButtonStates[button] == InputState.Pressed;

        public bool Released(MouseButtons button)
            => this.ButtonStates[button] == InputState.JustReleased;

        private static bool IsButtonDown(MouseButtons button, MouseState state)
            => button switch
            {
                MouseButtons.Left => state.LeftButton == ButtonState.Pressed,
                MouseButtons.Middle => state.MiddleButton == ButtonState.Pressed,
                MouseButtons.Right => state.RightButton == ButtonState.Pressed,
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null),
            };
    }
}
