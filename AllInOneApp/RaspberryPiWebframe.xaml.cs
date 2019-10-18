using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RaspberryPiWebframe : Page
    {
        public RaspberryPiWebframe()
        {
            this.InitializeComponent();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Making API Call...");
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("https://kephiso.webuntis.com/");
                HttpResponseMessage response = client.GetAsync("WebUntis/?school=OTH-Regensburg#/basic/timetable").Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                Debug.WriteLine("Result: !#!" + result+"!#!");
            }
            Debug.WriteLine("Testing with the WebView");
            webViewForHackingEverythingTogether.Navigate(new Uri("https://kephiso.webuntis.com/WebUntis/?school=OTH-Regensburg#/basic/timetable"));
            String tmp= await webViewForHackingEverythingTogether.InvokeScriptAsync("eval", new string[] { "document.documentElement.outerHTML;" });
            await Task.Delay(100);
            Debug.WriteLine("--"+tmp+"--");
        }
    }
}
