using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SerialPage : Page
    {
        private DataReader reader;
        private DataWriter writer;
        private String[] devIds;
        private SerialDevice serial;
        private bool shouldBeReading = false;

        public SerialPage()
        {
            this.InitializeComponent();
            ListDevices();
        }

        async void ListDevices()
        {
            String ids = "";
            availableSelection.Items.Clear();
            String aqsFilter = SerialDevice.GetDeviceSelector();
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(aqsFilter);
            //Log(devices.Count);
            if (devices.Any())
            {
                DeviceInformation[] infos = devices.ToArray();
                foreach (DeviceInformation info in infos)
                {
                    //Log(info.Name);
                    //Log(info.Id);
                    try
                    {
                        SerialDevice sd = await SerialDevice.FromIdAsync(info.Id);
                        var x=sd.PortName;//wenn es der Port nicht geöffnet werden kann wird hier eine exception auftreten => Exception basierter filter für die Auswahl in der Combobox
                        var y = sd.BaudRate;
                        //Log(sd.PortName);
                        //Log(sd.BaudRate.ToString());
                        ids += info.Id + "|";
                        availableSelection.Items.Add(new TextBox() { IsReadOnly = true, IsHitTestVisible = false, Text = info.Name + " (" + sd.PortName + ")" });
                        sd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        //ex.PrintStackTrace();
                        //Log("EXC");
                    }
                    //&Log("");
                }
                devIds = ids.Split("|");
                availableSelection.SelectedIndex = 0;
            }
        }

        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            shouldBeReading = false;
            await Task.Delay(20);
            shouldBeReading = false;
            await Task.Delay(20);
            try
            {
                reader.Dispose();
                serial.Dispose();
            }
            catch (Exception ex)
            {
                ex.PrintStackTrace();
            }
        }

        private async void ConnectSComPortButton_Click(object sender, RoutedEventArgs e)
        {
            int idx = availableSelection.SelectedIndex;
            String id = devIds[idx];
            serial = await SerialDevice.FromIdAsync(id);
            if (serial != null)
            {
                serial.BaudRate = 9600;
                serial.StopBits = SerialStopBitCount.One;
                serial.DataBits = 8;
                serial.Parity = SerialParity.None;
                serial.Handshake = SerialHandshake.None;

                //reader = new DataReader(serial.InputStream);
                writer = new DataWriter(serial.OutputStream);
                reader = new DataReader(serial.InputStream);
                reader.InputStreamOptions = InputStreamOptions.Partial;
                shouldBeReading = true;
                Debug.WriteLine("IOFUKH");
            }
        }

        private async Task Send(String data = "Hello World!\r\n")
        {
            writer.WriteString(data);
            await writer.StoreAsync();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (InputBox.Text == "" || InputBox.Text == null)
            {
                await Send();
            }
            else
            {
                await Send(InputBox.Text);
            }
        }

        private async Task<String> Read()
        {
            uint len = reader.UnconsumedBufferLength;
            uint ldRes=await reader.LoadAsync(len);
            if (len != 0 || ldRes != 0)
            {
                Debug.WriteLine(len);
                Debug.WriteLine(ldRes);
            }
            byte[] bts = new byte[len];
            reader.ReadBytes(bts);
            if (len > 0)
            {
                Debug.WriteLine("#");
                foreach (byte b in bts)
                {
                    Debug.WriteLine(b);
                }
                Debug.WriteLine("#");
            }
            return "FUCK OFF";
        }

        private async void TriggerRecv_Click(object sender, RoutedEventArgs e)
        {
            while (true)
            {
                await Read();
                await Task.Delay(10);
            }
            //Debug.WriteLine(await Read());
        }
    }
}
