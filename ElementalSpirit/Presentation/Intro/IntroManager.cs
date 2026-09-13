using System;
using System.Collections.Generic;
using ElementalSpirit.Localization;

namespace ElementalSpirit.Presentation.Intro
{
    public class IntroManager
    {
        private readonly List<DialogueScene> _scenes;
        private int _currentSceneIndex;
        private int _currentLineIndex;

        public event Action<DialogueScene>? OnSceneChanged;
        public event Action<DialogueLine>? OnLineChanged;
        public event Action? OnIntroCompleted;

        public DialogueScene CurrentScene => _scenes[_currentSceneIndex];
        public DialogueLine CurrentLine => CurrentScene.Lines[_currentLineIndex];
        public bool IsFinished => _currentSceneIndex >= _scenes.Count;

        public IntroManager(ILocalizationService localization)
        {
            if (localization == null) throw new ArgumentNullException(nameof(localization));
            _scenes = IntroData.CreateAllScenes(localization);
            _currentSceneIndex = 0;
            _currentLineIndex = 0;
        }

        public void Start()
        {
            if (_scenes.Count == 0)
            {
                OnIntroCompleted?.Invoke();
                return;
            }
            OnSceneChanged?.Invoke(CurrentScene);
            OnLineChanged?.Invoke(CurrentLine);
        }

        public void NextLine()
        {
            if (IsFinished) return;
            _currentLineIndex++;
            if (_currentLineIndex >= CurrentScene.Lines.Count)
            {
                _currentSceneIndex++;
                _currentLineIndex = 0;
                if (IsFinished) { OnIntroCompleted?.Invoke(); return; }
                OnSceneChanged?.Invoke(CurrentScene);
            }
            OnLineChanged?.Invoke(CurrentLine);
        }

        public void SkipScene()
        {
            if (IsFinished) return;
            _currentSceneIndex++;
            _currentLineIndex = 0;
            if (IsFinished) { OnIntroCompleted?.Invoke(); return; }
            OnSceneChanged?.Invoke(CurrentScene);
            OnLineChanged?.Invoke(CurrentLine);
        }

        public void SkipAll()
        {
            _currentSceneIndex = _scenes.Count;
            OnIntroCompleted?.Invoke();
        }
    }
}