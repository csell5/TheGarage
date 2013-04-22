using Microsoft.WindowsAzure;
using NotificationsExtensions;
using NotificationsExtensions.TileContent;
using NotificationsExtensions.ToastContent;
using System;

namespace theGarage.PushNotifications
{
    public class NotificationBroker
    {
        Action<NotificationSendResult> successCallback;
        Action<NotificationSendResult> errorCallback;

        private static void successResult(NotificationSendResult result)
        {
            var statusCode = result.StatusCode;
        }

        private static void errorResult(NotificationSendResult result)
        {
            var statusCode = result.StatusCode;
            var repo = new PushClientRepository();
            repo.DeleteClient(result.ChannelUri.ToString());

            foreach (var item in DataCache.Instance.PushNotificationClients)
            {
                if (item.ClientUrl == result.ChannelUri.ToString())
                    DataCache.Instance.PushNotificationClients.Remove(item);
            }
        }

        public void Send(string device, string status)
        {
            string _sid = CloudConfigurationManager.GetSetting("sid");
            string _secret = CloudConfigurationManager.GetSetting("secret");

            if (DataCache.Instance.PushNotificationClients.Count > 0)
            {
                WnsAccessTokenProvider tokenProvider = new WnsAccessTokenProvider(_sid, _secret);

                foreach (var clientUri in DataCache.Instance.PushNotificationClients)
                {
                    successCallback = successResult;
                    errorCallback = errorResult;

                    var title = "theGarage";
                    var subText = String.Format("thaGarage {0}: {1}", device, status);

                    var tileNotification = TileContentFactory.CreateTileWideText01();

                    tileNotification.RequireSquareContent = false;

                    tileNotification.TextHeading.Text = title;
                    tileNotification.TextBody1.Text = subText;
                    tileNotification.TextBody2.Text = DateTime.Now.ToLongTimeString();

                    tileNotification.SendAsynchronously(new Uri(clientUri.ClientUrl), tokenProvider, successCallback, errorCallback);

                    var toastNotification = ToastContentFactory.CreateToastText02();
                    toastNotification.TextHeading.Text = title;
                    toastNotification.TextBodyWrap.Text = subText;

                    toastNotification.SendAsynchronously(new Uri(clientUri.ClientUrl), tokenProvider, successCallback, errorCallback);
                }
            }

        }
    }
}