using Avalonia.Controls;

namespace Vocaloid;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Generate()
    {
        Track music = new("Test", 120);
    }
}