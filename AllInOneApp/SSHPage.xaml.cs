using Renci.SshNet;
using System;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SSHPage : Page
    {
        private int status = 3;//3=> hostname; 2=> user; 1=> password; 0=> commands

        private String hostname;
        private String user;
        private String password;
        SshClient client;

        public SSHPage()
        {
            this.InitializeComponent();

        }

        private void Input_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                    {
                        if (status == 3)
                        {
                            Println(Input.Text);
                            hostname = Input.Text;
                            Print("username: ");
                        }
                    }
                    break;
                default:
                    {
                        return;
                    }
            }
        }

        private void Println(String msg)
        {
            Output.Text += msg + "\r\n";
        }

        private void Print(String msg)
        {
            Output.Text += msg;
        }
    }
}
