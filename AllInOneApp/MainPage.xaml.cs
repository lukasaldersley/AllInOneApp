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
        //private SimpleOrientationSensor _simpleorientation;
        private UIElement[] items;
        private DispatcherTimer ResizeDelayTimer = new DispatcherTimer();
        private int ResizeDelayTimerTicks = 0;
        private int ResizeDelayTimerMaximum = 5;//5*50ms = 250ms

        public MainPage()
        {
            this.InitializeComponent();
            items = new UIElement[Stack.Children.Count];
            Stack.Children.CopyTo(items, 0);
            ResizeDelayTimer.Tick += ResizeDelayTimer_Tick;
            ResizeDelayTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            // Das ganze SimpleOrientationSensor zeug war ein experiment, aber ich glaube, ich brauche das nicht mehr. Ich lass es im comment falls ich nochmal damit experimentieren will.
            //Es ist aber vollkommen unabhängig vom rest, kann also bedenkenlos entfernt werden.
            /*_simpleorientation = SimpleOrientationSensor.GetDefault();
            if (_simpleorientation != null)
            {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            }*/
            Window.Current.SizeChanged += Current_SizeChanged;
            Setup();
        }

        private void ResizeDelayTimer_Tick(object sender, object e)
        {
            ResizeDelayTimerTicks++;
            if (ResizeDelayTimerTicks >= ResizeDelayTimerMaximum)
            {
                ResizeDelayTimer.Stop();
                ResizeDelayTimerTicks = 0;
                Debug.WriteLine("Actually resizing");
                Setup(true);
            }
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            Debug.WriteLine("Window.Current.SizeChanged Fired");
            ResizeDelayTimerTicks = 0;
            if (!ResizeDelayTimer.IsEnabled)
            {
                ResizeDelayTimer.Start();
            }
        }

        /*// Event function
        private async void OrientationChanged(object sender, SimpleOrientationSensorOrientationChangedEventArgs e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Debug.WriteLine("SimpleOrientationSensor Fired");
                //Setup();
            });
        }*/

        void Setup(bool isResize)
        {
            for(int i = 0; i < Stack.Children.Count; i++)
            {
                ((StackPanel)(Stack.Children[i])).Children.Clear();
            }
            Setup();
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
            

            //int nrY = items.Length / (size.Height / spacing).ToIntCeil();
            int nrX = (int)(size.Width / spacing);
            int nrY = (items.Length / (double)nrX).ToIntCeil();
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
                        sp.Children.Add(items[nextItem++]);//elements are already children of something
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

        private void NavigateToXKCDPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(XKCDPage));
        }

        private void NavigateToWebframeTestPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RaspberryPiWebframe));
        }

        private async void TestServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Debug.WriteLine(await ClintServerMethods.SendDataAndRecieveAnswer("localhost", 1337, "TEST REQUEST"));
                await ServerInterface.UploadFile(await StorageInterface.GetStorageFileFromToken(await StorageInterface.PickExternalStorageFile_OpenFile()), "/CODE/IMG.png");// "C:\\Users\\alder\\Desktop\\IMG.png", "localhost");
                                                                                                                                                                             //ServerInterface.ReadFile("invalidTestFile");//if reading a non-existent file I get 0x15 (NAK) in flags[1]
            }
            catch(Exception ex)
            {
                ex.PrintStackTrace();
            }
        }
    }
}
