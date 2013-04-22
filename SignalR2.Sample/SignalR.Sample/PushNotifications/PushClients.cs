
using System.ComponentModel.DataAnnotations;

namespace theGarage.PushNotifications
{
    public class PushClient
    {
        [Key]
        public string ClientUrl { get; set; }

        public PushClient() { }

        public PushClient(string url)
        {
            this.ClientUrl = url;
        }

        public override bool Equals(object obj)
        {
            return this.ClientUrl == ((PushClient)obj).ClientUrl;
        }

    }
}