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
    public sealed partial class JobManagerPage : Page
    {
        private static bool checkedIn=false;
        private static long msSinceEpoch = 0;

        public JobManagerPage()
        {
            this.InitializeComponent();
        }

        public static async Task LoadAll()
        {
            String log = await StorageInterface.ReadFromRoamingFolder("BMW/log.log");
            String cfg = await StorageInterface.ReadFromRoamingFolder("BMW/cfg.cfg");
            String[] cfgs = cfg.Split("|");
            if (cfgs[0].Equals("IN"))
            {
                checkedIn = true;
            }
            msSinceEpoch = long.Parse(cfgs[1]);
        }

        //On reactivation/init/whatever
        //read file with state (checked in/out, last checkin time (if neccessary) and saldos
        //if needed adjust values and save again
        //update UI
        //funktion um urlaub zu buchen einbauen

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
