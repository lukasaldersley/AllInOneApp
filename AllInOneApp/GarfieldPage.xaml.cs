using System;
using System.Diagnostics;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class GarfieldPage : Page
    {
        DateTimeOffset date;
        DateTime now = DateTime.Now;
        DateTime startOfTime = new DateTime(1978, 6, 19);
        public GarfieldPage()
        {
            this.InitializeComponent();
            date = DateTime.Now;
            datePicker.Date = date;
            ConfigurePage();
            Navigate();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AccereratorKeyActivated;
            datePicker.DateChanged += DatePicker_DateChanged;
            this.PointerWheelChanged += MouseWheelChanged;
        }

        private void MouseWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int mwd=e.GetCurrentPoint(null).Properties.MouseWheelDelta;
            Debug.WriteLine(mwd);
            if (mwd == -120 || mwd == 15240)
            {
                Debug.WriteLine("->");
                NextDay();
            }
            else if (mwd == 120 || mwd == -15240)
            {
                Debug.WriteLine("<-");
                PreviousDay();
            }
            else
            {
                Debug.WriteLine("WTF");
            }
        }

        private void AccereratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                if (args.VirtualKey == VirtualKey.Left)
                {
                    UserInteraction.Vibrate(1000);
                    PreviousDay();
                }
                if (args.VirtualKey == VirtualKey.Right)
                {
                    UserInteraction.Vibrate(1000);
                    NextDay();
                }
                if (args.VirtualKey == VirtualKey.Up)
                {
                    UserInteraction.Vibrate(1000);
                    date = RandomDate();
                    Navigate();
                    datePicker.Date = date;
                }
                if (args.VirtualKey == VirtualKey.Down)
                {
                    UserInteraction.Vibrate(100);
                    Download();
                }
            }
        }

        private void NextDay()
        {
            date = date.AddDays(1);
            now = DateTime.Now;
            if (date.CompareTo(now) > 0)
            {
                date = now;
            }
            datePicker.Date = date;
        }

        private void PreviousDay()
        {
            date = date.AddDays(-1);
            if (date.CompareTo(startOfTime) < 0)
            {
                date = startOfTime;
            }
            datePicker.Date = date;
        }

        private async void Download()
        {
            String Token = await StorageInterface.ReadFromLocalFolder("Storage.Garfield.token");
            if (Token == null || Token.Equals(""))
            {
                await UserInteraction.ShowDialogAsync("INFORMATION", "You will now be prompted to chose a Folder in which to save the Comic. This App will create a new Folder within the Folder you selected, called \"Garfield\", which will be used to store the images (in order not to confuse them with your files). The App will remember the location you have picked and will use this location until you change it in the Settings.");
                Token = await StorageInterface.PickExternalStorageFolder();
                await StorageInterface.WriteToLocalFolder("Storage.Garfield.token", Token);
            }
            try
            {
                await StorageInterface.WriteBytesToKnownFolder("Garfield/Garfield_" + date.ToString("yyyy_MM_dd") + ".gif", await NetworkInterface.DownloadGarfield(date), Token);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                UserInteraction.ShowToast("ERROR! Could not save file.","Garfield");
            }
            UserInteraction.ShowToast("Comic has successfully been saved", "Garfield");
        }

        private void ConfigurePage()
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped;         //NON DEFAULT VALUE
                                                                                                                                       //RequestedTheme = DATA.GetAppTheme();
                                                                                                                                       //RequestedTheme = ElementTheme.Light;
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }


        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            date = datePicker.Date;
            if (date.CompareTo(startOfTime) < 0)
            {
                date = startOfTime;
                datePicker.Date = date;
            }
            if (date.CompareTo(now) > 0)
            {
                date = now;
                datePicker.Date = date;
            }
            Navigate();
        }

        private void Navigate()
        {
            //displayComic.Navigate(new Uri("https://d1ejxu6vysztl5.cloudfront.net/comics/garfield/" + date.ToString("yyyy") + "/" + date.ToString("yyyy-MM-dd") + ".gif"));
            ComicImage.Source = new BitmapImage(new Uri("https://d1ejxu6vysztl5.cloudfront.net/comics/garfield/" + date.ToString("yyyy") + "/" + date.ToString("yyyy-MM-dd") + ".gif"));
            //scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight / 2);
            //scrollViewer.ScrollToHorizontalOffset(scrollViewer.ScrollableWidth / 2);
        }

        private void DayBeforeButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousDay();
        }

        private void DayAfterButton_Click(object sender, RoutedEventArgs e)
        {
            NextDay();
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            date = RandomDate();
            Navigate();
            datePicker.Date = date;
        }

        private DateTime RandomDate()
        {
            now = DateTime.Now;
            UInt64 days = (UInt64)(now.Subtract(startOfTime).TotalDays);
            Debug.WriteLine(days);
            int newDays = (int)(new Random().NextDouble() * (days + 1));
            return startOfTime.AddDays(newDays);
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
    }
}