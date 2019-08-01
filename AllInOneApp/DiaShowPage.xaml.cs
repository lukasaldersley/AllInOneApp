using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.UI.Xaml.Media;
using Windows.UI;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class DiaShowPage : Page
    {
        private bool halted=true;
        int i = 0;
        int j = 0;
        StorageFile[] arr;

        public DiaShowPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AccereratorKeyActivated;
        }

        private async void AccereratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                if (args.VirtualKey == VirtualKey.Left)
                {
                    i--;
                    i--;
                    if (i < 0)
                    {
                        i = 0;
                    }
                    ComicImage.Source = await StorageInterface.GetImageSourceFromStorageFile(arr[i++]);
                    if (i == arr.Length)
                    {
                        i = 0;
                        halted = true;
                        PauseButton.Content = "ENDE. Neu starten?";
                        PauseButton.Background = new SolidColorBrush(Colors.Red);
                    }
                    j = 0;
                }
                if (args.VirtualKey == VirtualKey.Right)
                {
                    if (i < 0)
                    {
                        i = 0;
                    }
                    ComicImage.Source = await StorageInterface.GetImageSourceFromStorageFile(arr[i++]);
                    if (i == arr.Length)
                    {
                        i = 0;
                        halted = true;
                        PauseButton.Content = "ENDE. Neu starten?";
                        PauseButton.Background = new SolidColorBrush(Colors.Red);
                    }
                    j = 0;
                }
                if (args.VirtualKey == VirtualKey.P)
                {
                    halted = !halted;
                    PauseButton.Content = halted ? "START" : "STOP";
                    PauseButton.Background = new SolidColorBrush(Colors.Black);
                }
            }
        }
        private async void Show()
        {
            while (true)
            {
                if (!halted)
                {
                    if (i < 0)
                    {
                        i = 0;
                    }
                    ComicImage.Source = await StorageInterface.GetImageSourceFromStorageFile(arr[i++]);
                    if (i == arr.Length)
                    {
                        i = 0;
                        halted = true;
                        PauseButton.Content = "ENDE. Neu starten?";
                        PauseButton.Background = new SolidColorBrush(Colors.Red);
                    }
                    for (j=0;j<40;j++)
                    await Task.Delay(100);
                }
                await Task.Delay(10);
            }
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            i = 0;
            arr = await (await StorageInterface.GetStorageFolderFromToken(await StorageInterface.PickExternalStorageFolder())).GetStorageFileArray();
            Show();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            halted = !halted;
            PauseButton.Content = halted ? "START" : "STOP";
            PauseButton.Background = new SolidColorBrush(Colors.Black);
        }
    }
}
