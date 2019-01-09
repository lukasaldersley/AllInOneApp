using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void InspectRoaming_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FolderInspectionPage), StorageInterface.ROAMING_FOLDER);
        }

        private void InspectLocal_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FolderInspectionPage), StorageInterface.LOCAL_FOLDER);
        }

        private async void ChangeGarfieldFolder_Click(object sender, RoutedEventArgs e)
        {
            await UserInteraction.ShowDialogAsync("INFORMATION", "You will now be prompted to chose a new Folder in which to save the Garfield Comics. This App will create a new Folder within the Folder you selected, called \"Garfield\", which will be used to store the images (in order not to confuse them with your files). The App will remember the location you have picked and will use this location until you change it here again.");
            string Token = await StorageInterface.PickExternalStorageFolder();
            await StorageInterface.WriteToLocalFolder("Storage.Garfield.token", Token);
        }
    }
}