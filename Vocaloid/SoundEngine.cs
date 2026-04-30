using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.Lame;

namespace Vocaloid
{
    enum Phonemes
    {
        // This would contain paths to each phoneme
        // Use this enum to load the Note class with sound.
    }
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

        public void AddNote(Note n)
        {
            using var reader = new AudioFileReader(n.FilePath);
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
            var outputDevice = new WaveOutEvent();

            SaveToMp3(playlist, $"{this.TrackName}.mp3");

            var tcs = new TaskCompletionSource<bool>();

            outputDevice.PlaybackStopped += (s, e) => tcs.SetResult(true);

            outputDevice.Init(playlist);
            outputDevice.Play();

            await tcs.Task;

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
    }
}