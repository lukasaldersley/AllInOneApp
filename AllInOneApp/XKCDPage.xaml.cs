using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class XKCDPage : Page
    {
        String data = "";
        private string nextUrl;
        private string prevUrl;
        private string desc;
        private string title;
        private string imgUrlLarge;
        private string imgUrl;
        private string currentID;

        public XKCDPage()
        {
            this.InitializeComponent();
        }

        private void DayBeforeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DayAfterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            await GetRandom();
            idPicker.TextChanged -= IdPicker_TextChanged;
            idPicker.Text = currentID;
            descriptionBox.Text = desc;
            titleBox.Text = title;
            ComicImage.Source = new BitmapImage(new Uri("http://" + imgUrlLarge));
            idPicker.TextChanged += IdPicker_TextChanged;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Download();
        }

        private async void Download()
        {
            String Token = await StorageInterface.ReadFromLocalFolder("Storage.XKCD.token");
            if (Token == null || Token.Equals(""))
            {
                await UserInteraction.ShowDialogAsync("INFORMATION", "You will now be prompted to chose a Folder in which to save the Comic. This App will create a new Folder within the Folder you selected, called \"XKCD\", which will be used to store the images (in order not to confuse them with your files). The App will remember the location you have picked and will use this location until you change it in the Settings.");
                Token = await StorageInterface.PickExternalStorageFolder();
                await StorageInterface.WriteToLocalFolder("Storage.XKCD.token", Token);
            }
            try
            {
                await StorageInterface.WriteBytesToKnownFolder("XKCD/XKCD_" + currentID+"_"+title + ".png", await NetworkInterface.DownloadXKCD(imgUrlLarge), Token);
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                UserInteraction.ShowToast("ERROR! Could not save file.", "XKCD");
            }
            UserInteraction.ShowToast("Comic has successfully been saved", "XKCD");
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

        private async Task GetRandom()
        {
            await GetData("RANDOM");
        }

        private async Task GetData()
        {
            await GetData("START");
        }

        private async Task GetData(String id)
        {
            if (id == "START")
            {
                id = "https://xkcd.com";
            }
            else if (id == "RANDOM")
            {
                id = "https://c.xkcd.com/random/comic";
            }
            else if (id == "#")
            {
                return;
            }
            else
            {
                id = "https://xkcd.com/" + id;
            }
            try
            {
                String data = (await NetworkInterface.Download(id));
                //Debug.WriteLine("##" + data + "##");
                int SI = data.IndexOf("<div id=\"ctitle\">");
                int EI = data.IndexOf("Image URL (for hotlinking/embedding)");
                data = data.Substring(SI, EI - SI);
                int nuSI = data.IndexOf("rel=\"next\" href=\"") + 17;
                int nuEI = data.IndexOf("\" accesskey=\"n");
                nextUrl = data.Substring(nuSI, nuEI - nuSI);
                nextUrl = nextUrl.Replace("/", "");
                int puSI = data.IndexOf("rel=\"prev\" href=\"") + 17;
                int puEI = data.IndexOf("\" accesskey=\"p");
                prevUrl = data.Substring(puSI, puEI - puSI);
                prevUrl = prevUrl.Replace("/", "");
                int deSI = data.IndexOf("\" title=\"") + 9;
                int deEI = data.IndexOf("\" alt=\"");
                desc = data.Substring(deSI, deEI - deSI);
                title = data.Substring(17, data.IndexOf("</div>") - 17);
                int iulSI = data.IndexOf("\" srcset=\"//") + 12;
                int iulEI = data.IndexOf("\"/>");//remove last stuff
                imgUrlLarge = data.Substring(iulSI, iulEI - iulSI);
                imgUrlLarge = imgUrlLarge.Substring(0, imgUrlLarge.IndexOf(" "));
                int iuSI = data.IndexOf("<img src=\"//") + 12;
                int iuEI = data.IndexOf("\" title=\"");
                imgUrl = data.Substring(iuSI, iuEI - iuSI);
                int cidSI = data.IndexOf("to this comic: ") + 30;
                int cidEI = data.LastIndexOf("<br");
                currentID = data.Substring(cidSI, cidEI - cidSI);
                currentID = currentID.Replace("/", "");
            }
            catch(Exception e)
            {
                e.PrintStackTrace();
                Debug.WriteLine(id);
                Debug.WriteLine("!!" + data + "!!");
                Debug.WriteLine("");
                Debug.WriteLine(prevUrl);
                Debug.WriteLine(nextUrl);
                Debug.WriteLine(desc);
                Debug.WriteLine(title);
                Debug.WriteLine(imgUrl);
                Debug.WriteLine(imgUrlLarge);
            }
        }

        private void IdPicker_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
