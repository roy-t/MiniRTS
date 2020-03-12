using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MiniEngine.Input
{
    public sealed class MouseInput
    {
        private readonly MouseButtons[] Buttons;
        private readonly Dictionary<MouseButtons, InputState> ButtonStates;

        private float scrollWheelValue;

        private Point lastPosition;
        private ScrollDirection scrollDirection;

        public MouseInput()
        {
            this.Buttons = Enum.GetValues(typeof(MouseButtons)).OfType<MouseButtons>().ToArray();
            this.ButtonStates = new Dictionary<MouseButtons, InputState>();

            foreach (var button in this.Buttons)
            {
                this.ButtonStates.Add(button, InputState.Released);
            }

            this.lastPosition = Point.Zero;
        }

        public void Update()
        {
            var current = Mouse.GetState();

            this.Movement = current.Position - this.lastPosition;
            this.lastPosition = current.Position;

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

        public bool Click(MouseButtons button) => this.ButtonStates[button] == InputState.JustPressed;

        public bool ScrolledUp => this.scrollDirection == ScrollDirection.Up;
        public bool ScrolledDown => this.scrollDirection == ScrollDirection.Down;

        public bool Hold(MouseButtons button)
        {
            var state = this.ButtonStates[button];
            return state == InputState.JustPressed || state == InputState.Pressed;
        }

        private static bool IsButtonDown(MouseButtons button, MouseState state)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return state.LeftButton == ButtonState.Pressed;
                case MouseButtons.Middle:
                    return state.MiddleButton == ButtonState.Pressed;
                case MouseButtons.Right:
                    return state.RightButton == ButtonState.Pressed;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
    }
}
