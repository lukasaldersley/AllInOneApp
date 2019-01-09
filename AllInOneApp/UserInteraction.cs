using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.Phone.Devices.Notification;

namespace AllInOneApp
{
    class UserInteraction
    {
        public static void Vibrate(double duration)
        {
            //VibrationDevice.GetDefault().Vibrate(TimeSpan.FromMilliseconds(duration));
        }

        public static void ShowToast(String content="Why is there no content?", String title = "AllInOneApp")
        {

            // Now we can construct the final toast content
            ToastContent toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText(){
                                Text = title
                            },

                            new AdaptiveText(){
                                Text = content
                            }
                        }

                    }
                }
            };

            // And create the toast notification
            var toast = new ToastNotification(toastContent.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static async Task ShowDialogAsync(String title="Generic Message",String content="Generic Content",String buttonText = "OK")
        {
            ContentDialog ErrorDialog = new ContentDialog()
            {
                Title = title,
                Content = content,
                PrimaryButtonText = buttonText
            };
            //ErrorDialog.PrimaryButtonClick += MethodToExecOnClick;

            await ErrorDialog.ShowAsync();
        }
    }
}