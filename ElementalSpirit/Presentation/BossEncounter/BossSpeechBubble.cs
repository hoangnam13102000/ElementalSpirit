namespace ElementalSpirit.Presentation.BossEncounter
{
    public sealed class BossSpeechBubble
    {
        public string Speaker { get; private set; } = "";
        public string Text { get; private set; } = "";
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Width { get; set; } = 300f;
        public float Height { get; set; } = 86f;
        public bool IsVisible { get; private set; }
        public bool IsPlayerBubble { get; private set; }
        public float RemainingTime { get; private set; }

        public void Show(string speaker, string text, float displaySeconds = 4.5f, bool isPlayerBubble = false)
        {
            Speaker = speaker ?? "";
            Text = text ?? "";
            IsPlayerBubble = isPlayerBubble;
            IsVisible = true;
            RemainingTime = displaySeconds;
        }

        public void Hide()
        {
            IsVisible = false;
            RemainingTime = 0f;
        }

        public void Update(float deltaTime)
        {
            if (!IsVisible) return;

            RemainingTime -= deltaTime;
            if (RemainingTime <= 0f)
                Hide();
        }

        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void SetPositionFromAnchor(float anchorX, float anchorY)
        {
            X = IsPlayerBubble ? anchorX + 32f : anchorX - 150f;
            Y = IsPlayerBubble ? anchorY - 110f : anchorY - 120f;
        }
    }
}
