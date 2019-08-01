using System;
using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SimpleOrientationSensor _simpleorientation;
        UIElement[] items;

        public MainPage()
        {
            this.InitializeComponent();
            items = new UIElement[Stack.Children.Count];
            Stack.Children.CopyTo(items, 0);
            // Put hits in the Constructor
            _simpleorientation = SimpleOrientationSensor.GetDefault();
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            }
            Setup();
        }
        // Event function
        private async void OrientationChanged(object sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                //Setup();
            });
        }

        void Setup()
        {
            int spacing = 120;
            Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            double scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            Debug.WriteLine(scaleFactor);
            Size displayResolution = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            Debug.WriteLine(displayResolution);
            Size size = new Size(bounds.Width, bounds.Height);
            Debug.WriteLine(size.Width + "x" + size.Height);
            Debug.WriteLine((int)(size.Width / spacing) + "x" + (int)(size.Height / spacing));
            Debug.WriteLine(ApplicationView.GetForCurrentView().Orientation);

            int nrY = items.Length / (size.Width / spacing).ToIntCeil();
            int nrX = (int)(size.Width / spacing);
            int nextItem = 0;
            Debug.WriteLine(items.Length);
            Stack.Children.Clear();
            for (int i = 0; i <= nrY; i++)
            {
                StackPanel sp = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };
                for (int j = 0; j < nrX; j++)
                {
                    if (nextItem < items.Length)
                    {
                        sp.Children.Add(items[nextItem++]);
                    }
                }
                Stack.Children.Add(sp);
            }
        }

        private void NavigateToGarfieldPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GarfieldPage));
        }

        private void NavigateToToDoListPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ToDoListPage));
        }

        private void NavigateToSettingsPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void NavigateToPasswordManagerPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PasswordManagerPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var X = "Hallo Welt Lorem Ipsum dolor lkdfsjkhljkköjhgfugkukfshkjdfhakhgfiagäikjsfdhjsfglaihgföfPFÜÜÜA'wöefwuoauhwerigserhgl0".DivideToLength(5);
            Debug.WriteLine(X);
            //Frame.Navigate(typeof(FolderInspectionPage), StorageInterface.ROAMING_FOLDER);
        }

        private void NavigateToTextEditor_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TextEditorPage), null);
        }

        private void NavigateToJobManagerPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(JobManagerPage));
        }

        private void NavigateToSensorPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SensorPage));
        }

        private void NavigateToDriveSystem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DriveSystemPage));
        }

        private void NavigateToSerialPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SerialPage));
        }

        private void NavigateToMicrosoftBandSetupPage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToFuellingPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CarStatusLogPage));
        }

        private void NavigateToDiaShowPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DiaShowPage));
        }
    }
}
