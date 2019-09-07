using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using System.Net;

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
        private UIElement[] elements;

        public XKCDPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += AccereratorKeyActivated;
            this.PointerWheelChanged += MouseWheelChanged;
            this.Loaded += XKCDPage_Loaded;
            //GetData().GetAwaiter().GetResult();
            //Navigate();
        }

        private async void XKCDPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(basePanel.Children.Count);
            elements = new UIElement[basePanel.Children.Count];
            basePanel.Children.CopyTo(elements, 0);
            basePanel.Children.Clear();
            basePanel.Children.Add(elements[0]);
            Navigate(await GetData());
            Debug.WriteLine("LOADED!!!");
        }

        private void MouseWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            int mwd = e.GetCurrentPoint(null).Properties.MouseWheelDelta;
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
                    RandomDay();
                }
                if (args.VirtualKey == VirtualKey.Down)
                {
                    UserInteraction.Vibrate(1000);
                    Download();
                }
            }
        }

        private void DayBeforeButton_Click(object sender, RoutedEventArgs e)
        {
            PreviousDay();
        }

        private void DayAfterButton_Click(object sender, RoutedEventArgs e)
        {
            NextDay();
        }

        private async void NextDay()
        {
            Navigate(await GetData(nextUrl));
        }

        private async void PreviousDay()
        {
            Navigate(await GetData(prevUrl));
        }

        private async void RandomDay()
        {
            Navigate(await GetRandom());
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            RandomDay();
        }

        private void Navigate(bool canDoNative)
        {
            basePanel.Children.Clear();
            if (canDoNative)
            {
                basePanel.Children.Add(elements[0]);
                webPanel.Visibility = Visibility.Collapsed;
                nativePanel.Visibility = Visibility.Visible;
                idPicker.TextChanged -= IdPicker_TextChanged;
                idPicker.Text = currentID;
                descriptionBox.Text = WebUtility.HtmlDecode(desc);//1984
                titleBox.Text = WebUtility.HtmlDecode(title);
                ComicImage.Source = new BitmapImage(new Uri("http://" + imgUrlLarge));
                idPicker.TextChanged += IdPicker_TextChanged;
            }
            else
            {
                basePanel.Children.Add(elements[1]);
                webPanel.Visibility = Visibility.Visible;
                nativePanel.Visibility = Visibility.Collapsed;
                WebImage.Source = new Uri("https://xkcd.com/" + currentID);
                idPicker.TextChanged -= IdPicker_TextChanged;
                idPicker.Text = currentID;
                idPicker.TextChanged += IdPicker_TextChanged;
            }
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
                await StorageInterface.WriteBytesToKnownFolder("XKCD/XKCD_" + currentID + "_" + title + ".png", await NetworkInterface.DownloadXKCD(imgUrlLarge), Token);
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

        private async Task<bool> GetRandom()
        {
            return await GetData("RANDOM");
        }

        private async Task<bool> GetData()
        {
            return await GetData("START");
        }

        private async Task<bool> GetData(String id)
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
                return true;
            }
            else
            {
                id = "https://xkcd.com/" + id;
            }
            int SI = 0, EI = 0, nuSI = 0, nuEI = 0, puSI = 0, puEI = 0, deSI = 0, deEI = 0, iulSI = 0, iulEI = 0, iuSI = 0, iuEI = 0, cidSI = 0, cidEI = 0, tiSI = 0, tiEI = 0;
            String OData = "";
            String data = "";
            try
            {
                data = (await NetworkInterface.Download(id));
                OData = data;
                //Debug.WriteLine("##" + data + "##");
                SI = data.IndexOf("<div id=\"ctitle\">");
                EI = data.IndexOf("Image URL (for hotlinking/embedding)");
                data = data.Substring(SI, EI - SI);

                cidSI = data.IndexOf("to this comic: ") + 32;
                cidEI = data.LastIndexOf("<br");
                currentID = data.Substring(cidSI, cidEI - cidSI);
                currentID = currentID.Replace("/", "");

                nuSI = data.IndexOf("rel=\"next\" href=\"") + 17;
                nuEI = data.IndexOf("\" accesskey=\"n");
                nextUrl = data.Substring(nuSI, nuEI - nuSI);
                nextUrl = nextUrl.Replace("/", "");
                puSI = data.IndexOf("rel=\"prev\" href=\"") + 17;
                puEI = data.IndexOf("\" accesskey=\"p");
                prevUrl = data.Substring(puSI, puEI - puSI);
                prevUrl = prevUrl.Replace("/", "");
                deSI = data.IndexOf("\" title=\"") + 9;
                deEI = data.IndexOf("\" alt=\"");
                desc = data.Substring(deSI, deEI - deSI);
                tiSI = 17;
                tiEI = data.IndexOf("</div>");
                title = data.Substring(tiSI, tiEI - tiSI);
                iuSI = data.IndexOf("<img src=\"//") + 12;
                iuEI = data.IndexOf("\" title=\"");
                imgUrl = data.Substring(iuSI, iuEI - iuSI);
                iulSI = data.IndexOf("\" srcset=\"//") + 12;
                iulEI = data.IndexOf("\"/>");//remove last stuff
                if (iulSI != 11 && iulEI != -1)
                {
                    imgUrlLarge = data.Substring(iulSI, iulEI - iulSI);
                    imgUrlLarge = imgUrlLarge.Substring(0, imgUrlLarge.IndexOf(" "));
                }
                else
                {
                    imgUrlLarge = imgUrl;
                }
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
                Debug.WriteLine("ID: \t" + id);
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("[O_DATA_START]" + OData + "[O_DATA_END]");
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("[DATA_START]" + data + "[DATA_END]");
                Debug.WriteLine("");
                Debug.WriteLine("");
                Debug.WriteLine("Prev: \t" + prevUrl);
                Debug.WriteLine("\t\t" + puSI);
                Debug.WriteLine("\t\t" + puEI);
                Debug.WriteLine("");
                Debug.WriteLine("Next: \t" + nextUrl);
                Debug.WriteLine("\t\t" + nuSI);
                Debug.WriteLine("\t\t" + nuEI);
                Debug.WriteLine("");
                Debug.WriteLine("this: \t" + currentID);
                Debug.WriteLine("\t\t" + cidSI);
                Debug.WriteLine("\t\t" + cidEI);
                Debug.WriteLine("");
                Debug.WriteLine("desc: \t" + desc);
                Debug.WriteLine("\t\t" + deSI);
                Debug.WriteLine("\t\t" + deEI);
                Debug.WriteLine("");
                Debug.WriteLine("title: \t" + title);
                Debug.WriteLine("\t\t" + tiSI);
                Debug.WriteLine("\t\t" + tiEI);
                Debug.WriteLine("");
                Debug.WriteLine("img: \t" + imgUrl);
                Debug.WriteLine("\t\t" + iuSI);
                Debug.WriteLine("\t\t" + iuEI);
                Debug.WriteLine("");
                Debug.WriteLine("imgL: \t" + imgUrlLarge);
                Debug.WriteLine("\t\t" + iulSI);
                Debug.WriteLine("\t\t" + iulEI);
                return false;
            }
            return true;
        }

        private async void IdPicker_TextChanged(object sender, TextChangedEventArgs e)
        {
            Navigate(await GetData(idPicker.Text));
        }
    }
}
