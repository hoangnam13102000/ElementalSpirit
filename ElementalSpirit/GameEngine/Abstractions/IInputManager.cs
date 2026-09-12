using System.Windows.Forms;

namespace ElementalSpirit.GameEngine.Abstractions
{
    public interface IInputManager
    {
        void KeyDown(Keys key);
        void KeyUp(Keys key);
        bool IsKeyDown(Keys key);
        (float dirX, float dirY) GetMovementDirection();
        bool IsJumpPressed();
        void Clear();
    }
}
