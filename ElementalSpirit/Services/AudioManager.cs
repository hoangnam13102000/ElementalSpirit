using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly object _musicOperationsLock = new();
        private Task _musicOperations = Task.CompletedTask;

        private readonly WaveOut _sfxOutput;
        private readonly MixingSampleProvider _sfxMixer;
        private readonly object _cachedSfxLock = new();
        private readonly Dictionary<string, float[]> _cachedSfxSamples =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> _pendingCachedSfxPlays =
            new(StringComparer.OrdinalIgnoreCase);
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
                QueueMusicOperation(() =>
                {
                    if (_musicReader != null)
                        _musicReader.Volume = value ? MusicVolume : 0f;
                }, "update music volume");
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

            _ = Task.Run(() => PreloadCachedSfx("coin.flac"));
            _ = Task.Run(() => PreloadCachedSfx("waterfall.mp3"));
        }

        public void PlayMusic(string fileName, bool loop = true)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Music file name cannot be empty.", nameof(fileName));

            QueueMusicOperation(() =>
            {
                StopMusicCore();

                string path = Path.Combine(AudioBaseDir, "Music", fileName);
                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"[AudioManager] Music file not found: {path}");
                    return;
                }

                _musicReader = new AudioFileReader(path) { Volume = MusicEnabled ? MusicVolume : 0f };
                ISampleProvider source = loop
                    ? new LoopStream(_musicReader).ToSampleProvider()
                    : _musicReader;

                _musicOutput = new WaveOut();
                _musicOutput.Init(source);
                _musicOutput.Play();
            }, $"play music '{fileName}'");
        }

        public void StopMusic()
        {
            QueueMusicOperation(StopMusicCore, "stop music");
        }

        private void StopMusicCore()
        {
            _musicOutput?.Stop();
            _musicOutput?.Dispose();
            _musicReader?.Dispose();
            _musicOutput = null;
            _musicReader = null;
        }

        private void QueueMusicOperation(Action operation, string description)
        {
            lock (_musicOperationsLock)
            {
                Task previousOperation = _musicOperations;
                _musicOperations = Task.Run(async () =>
                {
                    await previousOperation.ConfigureAwait(false);
                    try
                    {
                        operation();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[AudioManager] Could not {description}: {ex}");
                    }
                });
            }
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
                if (IsCachedSfx(fileName))
                {
                    PlayCachedSfx(fileName);
                    return;
                }

                WaveStream reader = Path.GetExtension(path).Equals(".flac", StringComparison.OrdinalIgnoreCase)
                    ? new MediaFoundationReader(path)
                    : new AudioFileReader(path);
                var volumeProvider = new VolumeSampleProvider(reader.ToSampleProvider()) { Volume = SfxVolume };
                var sampleProvider = ConvertToMixerFormat(volumeProvider);
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

        private static bool IsCachedSfx(string fileName) =>
            string.Equals(fileName, "coin.flac", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(fileName, "waterfall.mp3", StringComparison.OrdinalIgnoreCase);

        private void PreloadCachedSfx(string fileName)
        {
            string path = Path.Combine(AudioBaseDir, "SFX", fileName);
            if (!File.Exists(path))
            {
                System.Diagnostics.Debug.WriteLine($"[AudioManager] KHÔNG TÌM THẤY FILE SFX: {path}");
                return;
            }

            try
            {
                using WaveStream reader = Path.GetExtension(path).Equals(".flac", StringComparison.OrdinalIgnoreCase)
                    ? new MediaFoundationReader(path)
                    : new AudioFileReader(path);
                var source = ConvertToMixerFormat(reader.ToSampleProvider());
                var chunk = new float[8192];
                var samples = new List<float>();
                int read;

                while ((read = source.Read(chunk)) > 0)
                {
                    for (int i = 0; i < read; i++)
                        samples.Add(chunk[i]);
                }

                lock (_cachedSfxLock)
                {
                    var cachedSamples = samples.ToArray();
                    _cachedSfxSamples[fileName] = cachedSamples;
                    _pendingCachedSfxPlays.TryGetValue(fileName, out int pendingPlays);
                    while (pendingPlays > 0)
                    {
                        AddCachedSfxToMixer(cachedSamples);
                        pendingPlays--;
                    }
                    _pendingCachedSfxPlays.Remove(fileName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AudioManager] Lỗi preload {fileName}: {ex}");
            }
        }

        private void PlayCachedSfx(string fileName)
        {
            lock (_cachedSfxLock)
            {
                if (!_cachedSfxSamples.TryGetValue(fileName, out var samples))
                {
                    _pendingCachedSfxPlays.TryGetValue(fileName, out int pendingPlays);
                    _pendingCachedSfxPlays[fileName] = pendingPlays + 1;
                    return;
                }

                AddCachedSfxToMixer(samples);
            }
        }

        private void AddCachedSfxToMixer(float[] samples)
        {
            _sfxMixer.AddMixerInput(new CachedSfxSampleProvider(samples, SfxVolume));

            if (_sfxOutput.PlaybackState != PlaybackState.Playing)
                _sfxOutput.Play();
        }

        private ISampleProvider ConvertToMixerFormat(ISampleProvider source)
        {
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
            Task musicOperations;
            lock (_musicOperationsLock)
                musicOperations = _musicOperations;
            musicOperations.GetAwaiter().GetResult();
            _sfxOutput.Stop();
            _sfxOutput.Dispose();
        }
    }

    internal sealed class CachedSfxSampleProvider : ISampleProvider
    {
        private readonly float[] _samples;
        private readonly float _volume;
        private int _position;

        public CachedSfxSampleProvider(float[] samples, float volume)
        {
            _samples = samples;
            _volume = volume;
        }

        public WaveFormat WaveFormat =>
            WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public int Read(Span<float> buffer)
        {
            int count = Math.Min(buffer.Length, _samples.Length - _position);
            for (int i = 0; i < count; i++)
                buffer[i] = _samples[_position + i] * _volume;

            _position += count;
            return count;
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