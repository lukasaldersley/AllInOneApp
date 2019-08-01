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
using Windows.UI.Xaml.Input;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class DiaShowPage : Page
    {
        private bool halted = true;
        int i = 0;
        int j = 0;
        StorageFile[] arr;
        int del = 30;//del*100 = ms to wait

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
                    await Prev();
                }
                if (args.VirtualKey == VirtualKey.Right)
                {
                    await Nxt();
                }
                if (args.VirtualKey == VirtualKey.P)
                {
                    halted = !halted;
                    PauseButton.Content = halted ? "START" : "STOP";
                    PauseButton.Background = new SolidColorBrush(Colors.Black);
                }
            }
        }

        async Task Prev()
        {
            i--;
            i--;
            await Nav();
            j = 0;
        }

        async Task Nxt()
        {
            await Nav();
            j = 0;
        }

        private async void Show()
        {
            while (true)
            {
                if (!halted)
                {
                    await Nav();
                    for (j = 0; j < del; j++)
                        await Task.Delay(100);
                }
                await Task.Delay(10);
            }
        }

        async Task Nav()
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
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            i = 0;
            bool x = halted;
            halted = true;
            arr = await (await StorageInterface.GetStorageFolderFromToken(await StorageInterface.PickExternalStorageFolder())).GetStorageFileArray();
            Show();
            PauseButton.IsEnabled = true;
            halted = x;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            halted = !halted;
            PauseButton.Content = halted ? "START" : "STOP";
            PauseButton.Background = new SolidColorBrush(Colors.Black);
        }

        private async void ScrollViewer_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            var doubleTapPoint = e.GetPosition(scrollViewer);

            if (scrollViewer.ZoomFactor != 1)
            {
                scrollViewer.ZoomToFactor(1);
            }
            else if (scrollViewer.ZoomFactor == 1)
            {
                scrollViewer.ZoomToFactor(2);

                var dispatcher = Window.Current.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    scrollViewer.ScrollToHorizontalOffset(doubleTapPoint.X);
                    scrollViewer.ScrollToVerticalOffset(doubleTapPoint.Y);
                });
            }
        }

        private async void DayBeforeButton_Click(object sender, RoutedEventArgs e)
        {
            await Prev();
        }

        private async void DayAfterButton_Click(object sender, RoutedEventArgs e)
        {
            await Nxt();
        }
    }
}
