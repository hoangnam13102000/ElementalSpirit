using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ElementalSpirit.GameEngine
{
    public class GameTimer
    {
        private readonly System.Windows.Forms.Timer _uiTimer;
        private readonly Stopwatch _stopwatch;
        private long _lastTicks;

        public event Action<float>? OnTick;

        public bool IsRunning { get; private set; }

        public GameTimer(int targetFps = 60)
        {
            _stopwatch = new Stopwatch();
            _uiTimer = new System.Windows.Forms.Timer
            {
                Interval = Math.Max(1, 1000 / targetFps)
            };
            _uiTimer.Tick += OnUiTimerTick;
        }

        public void Start()
        {
            if (IsRunning) return;
            _stopwatch.Restart();
            _lastTicks = _stopwatch.ElapsedTicks;
            _uiTimer.Start();
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning) return;
            _uiTimer.Stop();
            _stopwatch.Stop();
            IsRunning = false;
        }

        public void Pause()
        {
            if (!IsRunning) return;
            _uiTimer.Stop();
            IsRunning = false;
        }

        public void Resume()
        {
            if (IsRunning) return;
            _lastTicks = _stopwatch.ElapsedTicks;
            if (!_stopwatch.IsRunning)
                _stopwatch.Start();
            _uiTimer.Start();
            IsRunning = true;
        }

        private void OnUiTimerTick(object? sender, EventArgs e)
        {
            long currentTicks = _stopwatch.ElapsedTicks;
            float deltaTime = (currentTicks - _lastTicks) / (float)Stopwatch.Frequency;
            _lastTicks = currentTicks;
            if (deltaTime > 0.1f) deltaTime = 0.1f;
            OnTick?.Invoke(deltaTime);
        }

        public void Dispose()
        {
            Stop();
            _uiTimer.Dispose();
        }
    }
}