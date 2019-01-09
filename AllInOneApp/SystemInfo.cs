using Windows.ApplicationModel;
using Windows.Graphics.Display;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.Xaml;
namespace AllInOneApp
{
    public static class SystemInfo
    {
        public static string SystemFamily { get; }
        public static string SystemVersion { get; }
        public static string SystemArchitecture { get; }
        public static string ApplicationName { get; }
        public static string ApplicationVersion { get; }
        public static string DeviceManufacturer { get; }
        public static string DeviceModel { get; }
        public static string FriendlyDeviceName { get; }
        public static string SystemSku { get; }
        public static string OperatingSystem { get; }
        public static string SystemHardwareVersion { get; }
        public static string SystemFirmwareVersion { get; }
        public static DisplayOrientations DeviceOrientation { get; }
        public static double DisplayResolutionWidth { get; }
        public static double DisplayResolutionHeight { get; }

        static SystemInfo()
        {
            // get the system family name
            AnalyticsVersionInfo ai = AnalyticsInfo.VersionInfo;
            SystemFamily = ai.DeviceFamily;

            // get the system version number
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            SystemVersion = $"{v1}.{v2}.{v3}.{v4}";

            // get the package architecure
            Package package = Package.Current;
            SystemArchitecture = package.Id.Architecture.ToString();

            // get the user friendly app name
            ApplicationName = package.DisplayName;

            // get the app version
            PackageVersion pv = package.Id.Version;
            ApplicationVersion = $"{pv.Major}.{pv.Minor}.{pv.Build}.{pv.Revision}";

            // get the device manufacturer and model name
            EasClientDeviceInformation eas = new EasClientDeviceInformation();
            FriendlyDeviceName = eas.FriendlyName;
            //eas.Id;
            OperatingSystem = eas.OperatingSystem;
            SystemFirmwareVersion = eas.SystemFirmwareVersion;
            SystemHardwareVersion = eas.SystemHardwareVersion;
            DeviceManufacturer = eas.SystemManufacturer;
            DeviceModel = eas.SystemProductName;
            SystemSku = eas.SystemSku;


            DeviceOrientation = DisplayInformation.GetForCurrentView().CurrentOrientation;
            DisplayResolutionWidth = Window.Current.Bounds.Width;
            DisplayResolutionHeight = Window.Current.Bounds.Height;
        }

    }
}