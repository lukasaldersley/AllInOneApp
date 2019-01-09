using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class TextEditorPage : Page
    {
        private StorageFile SourceFile;
        private Boolean hasFile;
        private Boolean isSaved;

        public TextEditorPage()
        {
            this.InitializeComponent();
            //Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AcceleratorKeyActivated;
        }

        /*private void AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down"))
            {
                if(args.VirtualKey==Windows.System.VirtualKey.)
            }
        }*/

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            /*Debug.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
            Debug.WriteLine(e.GetType());*/
            Debug.WriteLine("OnNavigatedTo in TextEditorPage");
            if (e.Parameter == null)
            {
                Debug.WriteLine("CREATING NEW");
                await NewFile(false);
            }
            else
            {
                if (e.Parameter is IActivatedEventArgs args && args.Kind == ActivationKind.File)
                {
                    Debug.WriteLine("Opening Argument-File");
                    SourceFile = (StorageFile)((args as FileActivatedEventArgs).Files[0]);
                }
                else
                {
                    Debug.WriteLine("OPENING GIVEN");
                    SourceFile = e.Parameter as StorageFile;
                }
                hasFile = true;
                await LoadFile();
                isSaved = true;
            }
            base.OnNavigatedTo(e);
            Workspace.TabFocusNavigation = Windows.UI.Xaml.Input.KeyboardNavigationMode.Local;
            Debug.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (!isSaved)
            {
                if (hasFile)
                {
                    await Save();
                }
                else
                {
                    await SaveAs();
                }
            }
        }

        private async Task LoadFile()
        {
            try
            {
                Workspace.Text = await StorageInterface.ReadFromStorageFile(SourceFile);
                NameBox.Text = SourceFile.Name;
                StatusBox.Text = "OK";
                StatusBox.Background = new SolidColorBrush(Colors.Black);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                Debug.WriteLine("SOME ERROR READING");
                StatusBox.Text = "ERR";
                StatusBox.Background = new SolidColorBrush(Colors.DarkRed);
            }
            hasFile = true;
            isSaved = true;
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            SourceFile = await StorageInterface.GetStorageFileFromToken(await StorageInterface.PickExternalStorageFile_OpenFile());
            await LoadFile();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (hasFile)
            {
                await Save();
            }
            else
            {
                await SaveAs();
            }
        }

        private async Task Save()
        {
            await StorageInterface.WriteToStorageFile(SourceFile, Workspace.Text);
            isSaved = true;
            StatusBox.Text = "OK";
            StatusBox.Background = new SolidColorBrush(Colors.Black);
        }

        private async void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            await SaveAs();
        }

        private async Task SaveAs()
        {
            SourceFile = await StorageInterface.GetStorageFileFromToken(await StorageInterface.PickExternalStorageFile_NewFile("Neue Textdatei"));
            await StorageInterface.WriteToStorageFile(SourceFile, Workspace.Text);
            hasFile = true;
            isSaved = true;
            NameBox.Text = SourceFile.Name;
            StatusBox.Text = "OK";
            StatusBox.Background = new SolidColorBrush(Colors.Black);
        }

        private async void NewButton_Click(object sender, RoutedEventArgs e)
        {
            await NewFile();
        }

        private async Task NewFile(Boolean needsNew = true)
        {
            if (needsNew)
            {
                if (!isSaved)
                {
                    if (hasFile)
                    {
                        await Save();
                    }
                    else
                    {
                        await SaveAs();
                    }
                }
            }

            Workspace.Text = "";

            NameBox.Text = "*Neue Textdatei";

            hasFile = false;
            isSaved = true;
        }

        private void Workspace_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            isSaved = false;
            StatusBox.Text = "!";
            StatusBox.Background = new SolidColorBrush(Colors.Red);
        }
    }
}
