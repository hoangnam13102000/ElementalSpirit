using System;
using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ElementalSpirit.Services
{
    /// <summary>
    /// Quản lý âm thanh toàn game: nhạc nền (loop) và hiệu ứng skill (phát chồng nhau được).
    /// SFX dùng chung 1 thiết bị output + MixingSampleProvider để tránh xung đột
    /// khi bấm skill liên tục (mỗi lần bấm không tạo mới thiết bị âm thanh).
    /// </summary>
    public class AudioManager : IDisposable
    {
        private static AudioManager? _instance;
        public static AudioManager Instance => _instance ??= new AudioManager();

        private WaveOut? _musicOutput;
        private AudioFileReader? _musicReader;

        private readonly WaveOut _sfxOutput;
        private readonly MixingSampleProvider _sfxMixer;
        private const int SfxSampleRate = 44100;
        private const int SfxChannels = 2;

        public float MusicVolume { get; set; } = 0.5f;
        public float SfxVolume { get; set; } = 0.8f;

        private bool _musicEnabled = true;
        public bool MusicEnabled
        {
            get => _musicEnabled;
            set
            {
                _musicEnabled = value;
                if (_musicReader != null)
                    _musicReader.Volume = value ? MusicVolume : 0f;
            }
        }

        public bool SfxEnabled { get; set; } = true;

        private static string AudioBaseDir =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Audio");

        private AudioManager()
        {
            _sfxMixer = new MixingSampleProvider(
                WaveFormat.CreateIeeeFloatWaveFormat(SfxSampleRate, SfxChannels))
            {
                ReadFully = true // giữ mixer luôn "sống", không tự dừng khi hết input
            };

            _sfxOutput = new WaveOut();
            _sfxOutput.Init(_sfxMixer);
            _sfxOutput.Play(); // chạy sẵn, không cần Play lại mỗi lần phát SFX
        }

        public void PlayMusic(string fileName, bool loop = true)
        {
            StopMusic();

            string path = Path.Combine(AudioBaseDir, "Music", fileName);
            if (!File.Exists(path)) return;

            _musicReader = new AudioFileReader(path) { Volume = MusicEnabled ? MusicVolume : 0f };
            ISampleProvider source = loop
                ? new LoopStream(_musicReader).ToSampleProvider()
                : _musicReader;

            _musicOutput = new WaveOut();
            _musicOutput.Init(source);
            _musicOutput.Play();
        }

        public void StopMusic()
        {
            _musicOutput?.Stop();
            _musicOutput?.Dispose();
            _musicReader?.Dispose();
            _musicOutput = null;
            _musicReader = null;
        }

        public void PlaySfx(string fileName)
        {
            if (!SfxEnabled) return;
            if (string.IsNullOrEmpty(fileName)) return;

            string path = Path.Combine(AudioBaseDir, "SFX", fileName);

            if (!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"[AudioManager] KHÔNG TÌM THẤY FILE SFX: {path}");
                return;
            }

            try
            {
                var reader = new AudioFileReader(path) { Volume = SfxVolume };
                var sampleProvider = ConvertToMixerFormat(reader);
                var autoDisposeProvider = new AutoDisposeSampleProvider(sampleProvider, reader);

                _sfxMixer.AddMixerInput(autoDisposeProvider);

                // ✅ Đảm bảo output luôn đang phát — an toàn dù đang chạy sẵn (no-op),
                // nhưng "cứu" trường hợp playback bị dừng ngầm sau 1 thời gian.
                if (_sfxOutput.PlaybackState != PlaybackState.Playing)
                {
                    System.Diagnostics.Debug.WriteLine("[AudioManager] SFX output đã dừng, đang khởi động lại...");
                    _sfxOutput.Play();
                }

                System.Diagnostics.Debug.WriteLine($"[AudioManager] Đã thêm vào mixer: {fileName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioManager] Lỗi SFX: {ex.Message}");
            }
        }

        private ISampleProvider ConvertToMixerFormat(AudioFileReader reader)
        {
            ISampleProvider source = reader;

            // Đưa mọi file về cùng sample rate/channels với mixer, tránh lỗi "invalid parameter"
            if (source.WaveFormat.Channels == 1)
                source = new MonoToStereoSampleProvider(source);

            if (source.WaveFormat.SampleRate != SfxSampleRate)
                source = new WdlResamplingSampleProvider(source, SfxSampleRate);

            return source;
        }

        public void Dispose()
        {
            StopMusic();
            _sfxOutput.Stop();
            _sfxOutput.Dispose();
        }
    }


    public class AutoDisposeSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _source;
        private readonly IDisposable _resourceToDispose;
        private bool _isDisposed;

        public AutoDisposeSampleProvider(ISampleProvider source, IDisposable resourceToDispose)
        {
            _source = source;
            _resourceToDispose = resourceToDispose;
        }

        public WaveFormat WaveFormat => _source.WaveFormat;

        public int Read(Span<float> buffer)
        {
            if (_isDisposed) return 0;

            int read = _source.Read(buffer);
            if (read == 0)
            {
                _resourceToDispose.Dispose();
                _isDisposed = true;
            }
            return read;
        }
    }

    /// <summary>Wrap 1 audio stream để tự lặp lại (loop) vô hạn.</summary>
    public class LoopStream : WaveStream
    {
        private readonly WaveStream _source;
        public LoopStream(WaveStream source) => _source = source;

        public override WaveFormat WaveFormat => _source.WaveFormat;
        public override long Length => _source.Length;
        public override long Position
        {
            get => _source.Position;
            set => _source.Position = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int read = _source.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    if (_source.Position == 0) break;
                    _source.Position = 0;
                    continue;
                }
                totalRead += read;
            }
            return totalRead;
        }
    }
}