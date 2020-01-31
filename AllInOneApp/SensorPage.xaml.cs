using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace AllInOneApp
{
    public sealed partial class SensorPage : Page
    {
        private Accelerometer acc;
        //private ActivitySensor act;
        private Altimeter alt;
        private Barometer baro;
        private Compass comp;
        private Gyrometer gyro;
        //private HingeAngleSensor has;
        private Inclinometer inc;
        //private LightSensor ls;
        private Magnetometer mm;
        private OrientationSensor os;
        //private Pedometer pm;
        //private ProximitySensor ps;
        private SimpleOrientationSensor sos;
        private string Token;
        private bool logging;
        private bool rapid;

        public SensorPage()
        {
            this.InitializeComponent();
        }

        public async void Log()
        {
            await Task.Delay(1000);
            int i = 0;
            while (logging)
            {
                i++;
                cnt.Text = i.ToString();
                if (i % 29 == 0)
                {
                    OUT.Text = "";
                }
                OUT.Text += i + ": ";
                PrintSensors(rapid);
                if (!rapid)
                {
                    await Task.Delay(1000);
                }
                else
                {
                    await Task.Delay(50);
                }
            }
        }

        public void PrintSensors(bool fast=false)
        {

            //BarometerReading read = baro.GetCurrentReading();
            var accR = acc.GetCurrentReading();
            //Debug.WriteLine(Math.Sqrt(Math.Pow(accR.AccelerationX; 2) + Math.Pow(accR.AccelerationY; 2) + Math.Pow(accR.AccelerationZ; 2)) + " (" + accR.AccelerationX + "/" + accR.AccelerationX + "/" + accR.AccelerationX + ")");
            //var actR = act.GetCurrentReadingAsync().GetResults();
            //Debug.WriteLine(actR.Activity + " (" + actR.Confidence + ")");
            //Debug.WriteLine(alt.GetCurrentReading().AltitudeChangeInMeters);
            //Debug.WriteLine(baro.GetCurrentReading().StationPressureInHectopascals);
            var compR = comp.GetCurrentReading();
            //Debug.WriteLine(compR.HeadingMagneticNorth + " (+/-" + compR.HeadingAccuracy + ")");
            var gyroR = gyro.GetCurrentReading();
            //Debug.WriteLine("(" + gyroR.AngularVelocityX + "/" + gyroR.AngularVelocityY + "/" + gyroR.AngularVelocityZ + ")");
            var incR = inc.GetCurrentReading();
            //Debug.WriteLine("(" + incR.PitchDegrees + "/" + incR.RollDegrees + "/" + incR.YawDegrees + ")");
            //var psR=ps.GetCurrentReading();
            //Debug.WriteLine(psR.IsDetected + ": " + psR.DistanceInMillimeters);
            //Debug.WriteLine(sos.GetCurrentOrientation());



            String csv=(Math.Sqrt(Math.Pow(accR.AccelerationX, 2) + Math.Pow(accR.AccelerationY, 2) + Math.Pow(accR.AccelerationZ, 2)) + ";" + accR.AccelerationX + ";" + accR.AccelerationY + ";" + accR.AccelerationZ + ";"
                + alt.GetCurrentReading().AltitudeChangeInMeters + ";" + baro.GetCurrentReading().StationPressureInHectopascals + ";" + compR.HeadingMagneticNorth + ";" + compR.HeadingTrueNorth + ";" + compR.HeadingAccuracy
                + ";" + gyroR.AngularVelocityX + ";" + gyroR.AngularVelocityY + ";" + gyroR.AngularVelocityZ + ";" + incR.PitchDegrees + ";" + incR.RollDegrees + ";" + incR.YawDegrees + ";" + sos.GetCurrentOrientation()+";"+accR.Timestamp.ToUnixTimeMilliseconds()+";"+fast);
            Debug.WriteLine(csv);
            StorageInterface.AppendToKnownStorageFile(Token, csv+"\r\n").GetAwaiter();
            OUT.Text += csv + "\r\n";
        }

        private async void Sample_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Token = await  StorageInterface.PickExternalStorageFile_NewFile("log.txt");
            await StorageInterface.WriteToKnownStorageFile(Token, "acc;accX;accY;accZ;alt;baro;compMN;compTN;compAC;gyroX;gyroY;gyroZ;pitch;roll;yaw;sos;ts(acc);isFast\r\n");
            UserInteraction.ShowToast("Sensors", "INIT DONE!");
        }


        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.LandscapeFlipped | DisplayOrientations.Portrait | DisplayOrientations.PortraitFlipped;
            acc = Accelerometer.GetDefault();
            //act = ActivitySensor.GetDefaultAsync().GetResults();
            alt = Altimeter.GetDefault();
            baro = Barometer.GetDefault();
            comp = Compass.GetDefault();
            gyro = Gyrometer.GetDefault();
            //has = HingeAngleSensor.GetDefaultAsync().GetResults();
            inc = Inclinometer.GetDefault();
            mm = Magnetometer.GetDefault();
            os = OrientationSensor.GetDefault();
            //pm = Pedometer.GetDefaultAsync().GetResults();
            //ps = ProximitySensor.FromId(ProximitySensor.GetDeviceSelector());
            sos = SimpleOrientationSensor.GetDefault();
        }

        private void StartStop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            logging =! logging;
            Log();
            UserInteraction.ShowToast("Sensors", "Start/Stop Request handled");
        }

        private void Pacer_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            rapid = !rapid;
            UserInteraction.ShowToast("Sensors", "Toggled Mode");
        }
    }
}
