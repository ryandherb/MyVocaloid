using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Vocaloid;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnSelectFileClicked(object sender, RoutedEventArgs e)
    {
        var storageProvider = this.StorageProvider;

        var jsonFileType = new FilePickerFileType("JSON Files")
        {
            Patterns = ["*.json"]
        };

        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select .JSON File...",
            AllowMultiple = false,
            FileTypeFilter = [jsonFileType]
        });

        if (files.Any())
        {
            string pathName = files[0].Path.AbsolutePath;
            FilePathText.Text = pathName;

            await Compile(pathName);

        }
    }


    public async Task Compile(string pathName)
    {
        string jsonRaw = File.ReadAllText(pathName);
        Console.WriteLine($"Path: {pathName}");
        Console.WriteLine($"JSON length: {jsonRaw.Length}");
        Console.WriteLine($"JSON: '{jsonRaw}'");
        var track = JsonSerializer.Deserialize<TrackData>(jsonRaw);


        if (track == null)
        {
            return;
        }

        Console.WriteLine($"Starting compilation of track: {track.TrackName}");
        Track media = new(track.TrackName, track.Tempo);

        foreach (NoteData n in track.Notes)
        {
            string phonemePath = "./phonemes/" + n.Note + ".wav";

            if (!File.Exists(phonemePath))
            {
                Console.WriteLine($"Phoneme file {phonemePath} not found");
            }

            double duration = n.Duration;
            int pitch = n.Pitch;

            media.AddNote(new(phonemePath, duration, pitch));
        }

        await media.Play();
    }
}