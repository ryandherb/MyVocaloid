using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Vocaloid
{
    public class Note(string filePath, double duration, int pitch)
    {
        public string FilePath { get; } = filePath;
        public double Duration { get; } = duration;
        public int Pitch { get; } = pitch;
        public async Task PlayNote()
        {
            await using var audioFile = new AudioFileReader(FilePath);
            using var outputDevice = new WaveOutEvent();

            var tcs = new TaskCompletionSource<bool>();

            outputDevice.PlaybackStopped += (s, e) => tcs.SetResult(true);

            outputDevice.Init(audioFile);
            outputDevice.Play();

            await tcs.Task;
        }
    }

    public class Track(string trackName, int tempo)
    {
        public string TrackName { get; set; } = trackName;
        public int Tempo { get; set; } = tempo;
        private readonly List<SmbPitchShiftingSampleProvider> Notes = [];
        private readonly List<AudioFileReader> Readers = [];

        public void AddNote(Note n)
        {
            var reader = new AudioFileReader(n.FilePath);
            Readers.Add(reader);  // Keep it alive
            var sample = reader.ToSampleProvider();

            var trimmed = new OffsetSampleProvider(sample)
            {
                Take = TimeSpan.FromSeconds(n.Duration)
            };

            var pitchShifted = new SmbPitchShiftingSampleProvider(trimmed)
            {
                PitchFactor = (float)Math.Pow(2.0, n.Pitch / 12.0)
            };

            Notes.Add(pitchShifted);
        }
        public async Task<bool> Play()
        {
            if (Notes.Count == 0)
            {
                return false;
            }

            var playlist = new ConcatenatingSampleProvider(Notes);

            // Only save MP3 on Windows; use WAV on other platforms
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                string outputFile = $"{this.TrackName}.mp3";
                SaveToMp3(playlist, outputFile);
                Console.WriteLine($"Saved track to {outputFile}");
            }
            else
            {
                // Save as WAV on Linux/macOS
                string outputFile = $"{this.TrackName}.wav";
                SaveToWav(playlist, outputFile);
                Console.WriteLine($"Saved track to {outputFile}");
            }

            // Only attempt playback on Windows
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                var outputDevice = new WaveOutEvent();
                var tcs = new TaskCompletionSource<bool>();

                outputDevice.PlaybackStopped += (s, e) => tcs.SetResult(true);

                outputDevice.Init(playlist);
                outputDevice.Play();

                await tcs.Task;
            }
            else
            {
                Console.WriteLine("Audio playback not supported on this platform. MP3 saved instead.");
                await Task.CompletedTask;
            }

            return true;
        }

        public void SaveToMp3(ISampleProvider provider, string outputFilePath)
        {
            using var writer = new LameMP3FileWriter(outputFilePath, provider.WaveFormat, 128);
            var waveProvider = provider.ToWaveProvider();
            byte[] buffer = new byte[waveProvider.WaveFormat.AverageBytesPerSecond];
            int bytesRead;
            while ((bytesRead = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.Write(buffer, 0, bytesRead);
            }
        }

        public void SaveToWav(ISampleProvider provider, string outputFilePath)
        {
            using var writer = new NAudio.Wave.WaveFileWriter(outputFilePath, provider.WaveFormat);
            var waveProvider = provider.ToWaveProvider();
            byte[] buffer = new byte[waveProvider.WaveFormat.AverageBytesPerSecond];
            int bytesRead;
            while ((bytesRead = waveProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                writer.Write(buffer, 0, bytesRead);
            }
        }

        public void Dispose()
        {
            foreach (var reader in Readers)
            {
                reader?.Dispose();
            }
            Readers.Clear();
        }
    }
}