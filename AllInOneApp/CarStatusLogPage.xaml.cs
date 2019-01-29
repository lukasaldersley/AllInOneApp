using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class CarStatusLogPage : Page
    {
        public CarStatusLogPage()
        {
            this.InitializeComponent();
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await Refresh();
        }

        private async Task<string> InputTextDialogAsync(string title)
        {
            TextBox inputTextBox1 = new TextBox
            {
                AcceptsReturn = false,
                PlaceholderText = "Strecke",
                Height = 32
            };
            TextBox inputTextBox2 = new TextBox
            {
                AcceptsReturn = false,
                PlaceholderText = "Liter",
                Height = 32
            };
            TextBox inputTextBox3 = new TextBox
            {
                AcceptsReturn = false,
                PlaceholderText = "Kosten",
                Height = 32
            };
            StackPanel cnt = new StackPanel();
            cnt.Children.Add(inputTextBox1);
            cnt.Children.Add(inputTextBox2);
            cnt.Children.Add(inputTextBox3);
            ContentDialog dialog = new ContentDialog
            {
                Content = cnt,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox1.Text+";"+inputTextBox2.Text+";"+inputTextBox3.Text+"\r\n".Replace('.',',');
            else
                return "";
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Refresh();
            base.OnNavigatedTo(e);
        }

        private async Task Refresh()
        {
            String[] X = (await StorageInterface.ReadFromRoamingFolder("PoloTanken.csv")).Trim().Replace("\r","|").Replace("\n","|").Split("|");
            Double km = 0, cost = 0, liter = 0;
            foreach(String A in X)
            {
                String[] S = A.Trim().Split(";");
                km += Double.Parse(S[0]);
                liter += Double.Parse(S[1]);
                cost += Double.Parse(S[2]);
            }
            TotalKmBox.Text = km.ToString();
            TotalEuroBox.Text = cost.ToString();
            TotalLiterBox.Text = liter.ToString();
            TotalAvgBox.Text = (100 * liter / km).ToString();
        }

        private async void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            await StorageInterface.WriteToRoamingFolder("PoloTanken.csv",await StorageInterface.ReadFromRoamingFolder("PoloTanken.csv") + await InputTextDialogAsync("Neue Batankung"));
            await Refresh();
        }
    }
}
