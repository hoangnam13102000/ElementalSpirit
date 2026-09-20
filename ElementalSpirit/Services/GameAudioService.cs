using System;
using System.IO;
using ElementalSpirit.Services.Abstractions;
using NAudio.Wave;

namespace ElementalSpirit.Services
{
    public sealed class GameAudioService : IAudioService
    {
        private readonly string _mediaDirectory;
        private IWavePlayer? _outputDevice;
        private AudioFileReader? _audioFile;

        public GameAudioService()
        {
            _mediaDirectory = Path.Combine(AppContext.BaseDirectory, "Resources", "Media");
        }

        public void PlayStageTheme(int stageNumber)
        {
            string fileName = stageNumber >= 4
                ? "Boss Battle.wav"
                : "Fantasy Night mp3.mp3";

            string absolutePath = Path.Combine(_mediaDirectory, fileName);
            if (!File.Exists(absolutePath))
            {
                Stop();
                return;
            }

            if (_outputDevice != null && _audioFile != null &&
                string.Equals(_audioFile.FileName, absolutePath, StringComparison.OrdinalIgnoreCase) &&
                _outputDevice.PlaybackState == PlaybackState.Playing)
            {
                return;
            }

            Stop();
            _audioFile = new AudioFileReader(absolutePath);
            _outputDevice = new WaveOutEvent
            {
                Volume = 0.5f
            };
            _outputDevice.PlaybackStopped += (_, _) =>
            {
                if (_audioFile != null)
                {
                    _audioFile.Position = 0;
                    if (_outputDevice != null && _outputDevice.PlaybackState != PlaybackState.Playing)
                    {
                        _outputDevice.Play();
                    }
                }
            };
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }

        public void Stop()
        {
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }
        }

        public void Dispose() => Stop();
    }
}
