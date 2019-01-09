using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class DriveSystemPage : Page
    {
        DispatcherTimer Timer = new DispatcherTimer();
        public DriveSystemPage()
        {
            this.InitializeComponent();
            UpdateTime();
            /*focusGrabber.Focus(FocusState.Programmatic);
            DateBox.Focus(FocusState.Unfocused);
            TimeBox.Focus(FocusState.Unfocused);
            TitleBox.Focus(FocusState.Unfocused);*/
            //this.ActiveControl = null;
            //focusGrabber.Visibility = Visibility.Collapsed;
            Timer.Tick += Timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            UpdateTime();
        }

        void UpdateTime()
        {
            TimeBox.Text = DateTime.Now.ToString("HH:mm:ss");
            DateBox.Text = DateTime.Now.ToString("ddddd, dd.MM.yyyy");
        }

        private void LocalMusicSystemButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AllInOneAppBaseSystemButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void OBDSystemButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BluetoothMediaSystemButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioSystemButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void NavigationSystemButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-drive-to://"));
        }
    }
}
