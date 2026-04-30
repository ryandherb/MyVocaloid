using System.IO;
using System.Linq;
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
            FilePathText.Text = files[0].Path.LocalPath;
        }
    }


    public void Compile(string pathName)
    {
        
    }
}