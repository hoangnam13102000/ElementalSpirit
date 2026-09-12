using System.Collections.Generic;
using System.Windows.Forms;
using ElementalSpirit.GameEngine.Abstractions;

namespace ElementalSpirit.GameEngine
{
    public class InputManager : IInputManager
    {
        private readonly HashSet<Keys> _pressedKeys = new();

        public void KeyDown(Keys key)
        {
            _pressedKeys.Add(key);
        }

        public void KeyUp(Keys key)
        {
            _pressedKeys.Remove(key);
        }

        public bool IsKeyDown(Keys key) => _pressedKeys.Contains(key);

        public (float dirX, float dirY) GetMovementDirection()
        {
            float x = 0f;

            if (IsKeyDown(Keys.A) || IsKeyDown(Keys.Left)) x -= 1f;
            if (IsKeyDown(Keys.D) || IsKeyDown(Keys.Right)) x += 1f;

            return (x, 0f);
        }

        public bool IsJumpPressed()
        {
            return IsKeyDown(Keys.W) || IsKeyDown(Keys.Up);
        }

        public void Clear()
        {
            _pressedKeys.Clear();
        }
    }
}