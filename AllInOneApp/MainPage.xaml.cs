using System.Diagnostics;
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
        public MainPage()
        {
            this.InitializeComponent();
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
    }
}
