using System;
using System.Diagnostics;
using System.Net.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CameraControlPage : Page
    {
        private static String targetHost = "lukasaldersley.de";
        private bool tryReconn;
        private bool isOn = false;
        bool statup = true;

        public CameraControlPage()
        {
            this.InitializeComponent();
        }



        public static String Send(String command)
        {
            try
            {
                Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
                s.Connect(targetHost, 44444);
                s.ReceiveTimeout = 10000;
                Debug.WriteLine(NetworkInterface.TX_RX_TCP("HELLO", s));
                String str = NetworkInterface.TX_RX_TCP(command, s);
                Debug.WriteLine(str);
                Debug.WriteLine(NetworkInterface.TX_RX_TCP("EXIT", s));
                s.Dispose();
                return str.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return "VERBINDUNGSAUFBAU NICHT MÖGLICH! BESTEHT EINE INTERNETVERBINDUNG?";
            }
        }


        private void ToggleState_Click(object sender, RoutedEventArgs e)
        {
            if (statup)
            {
                String resp = Send("HELLO");
                //resp = resp;
                if (resp.Equals("HELLO"))
                {
                    Logln("INFO: Verschlüsselte Verbindung mit \"" + targetHost + ":44444\" (\"" + Send("YOUR IP") + ":44444\") hergestellt.");
                    String stats = Send("STATUS");
                    //stats = stats;
                    isOn = stats.Contains("LAEUFT: JA");
                    if (isOn)
                    {
                        ToggleState.Content = "Kamera deaktivieren";
                    }
                    else
                    {
                        ToggleState.Content = "Kamera aktivieren";
                    }
                }
                else
                {
                    Logln("WARNUNG: Server reagiert nicht. (Eigene Internetverbindung prüfen)");
                    Logln("\tFehler: " + resp);
                    ToggleState.Content = "Erneut versuchen";
                    tryReconn = true;
                }
                statup = false;
            }
            else
            {
                if (tryReconn)
                {
                    String resp = Send("HELLO");
                    if (resp.Equals("HELLO"))
                    {
                        Logln("INFO: Verschlüsselte Verbindung mit \"" + targetHost + ":44444\" (\"" + Send("YOUR IP") + ":44444\") hergestellt.");
                    }
                    else
                    {
                        Logln("WARNUNG: Server reagiert nicht. (Eigene Internetverbindung prüfen)");
                        Logln("\tFehler: " + resp);
                        ToggleState.Content = "Erneut versuchen";
                        tryReconn = true;
                    }
                }
                else
                {
                    String X = "";
                    if (isOn)
                    {
                        Logln("INFO: Versuche Kamera zu deaktivieren...");
                        X = Send("RECOFF");
                        Logln("\t\"RECOFF\" -> SERVER");
                        Logln("\tSERVER -> \"" + X + "\"");
                        if (X.Equals("RECOFF: OK"))
                        {
                            Logln("ERFOLG!");
                            isOn = false;
                            ToggleState.Content = "Kamera aktivieren";
                        }
                        else
                        {
                            Logln("NICHT ERFOLGREICH!");
                        }
                    }
                    else
                    {
                        Logln("INFO: Versuche Kamera zu aktivieren...");
                        X = Send("RECON");
                        Logln("\t\"RECON\" -> SERVER");
                        Logln("\tSERVER -> \"" + X + "\"");
                        if (X.Equals("RECON: OK"))
                        {
                            Logln("ERFOLG!");
                            isOn = true;
                            ToggleState.Content = "Kamera deaktivieren";
                        }
                        else
                        {
                            Logln("NICHT ERFOLGREICH!");
                        }
                    }
                }
            }
        }

        private void CheckStatus_Click(object sender, RoutedEventArgs e)
        {
            Logln("INFO: Frage Server-Status ab...");
            Logln("\tSTATUS -> SERVER");
            Logln("\tSERVER ->");
            Logln(Send("STATUS"));
        }

        private void Logln(String data)
        {
            OutputArea.Text += data + "\r\n";
        }

        private void Log(String data)
        {
            OutputArea.Text += data;
        }
    }
}