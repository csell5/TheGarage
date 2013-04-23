
using System.Collections.Generic;
using System.Web.Http;
using theGarage.PushNotifications;

namespace theGarage.API
{
    public class PushNotificationClient
    {
        public string uri { get; set; }
    }

    public class PushNotificationsController : ApiController
    {
        // GET api/values
        public IEnumerable<PushClient> Get()
        {
            return DataCache.Instance.PushNotificationClients;
        }

        // POST api/values
        public void Post(PushNotificationClient client)
        {
            if (!DataCache.Instance.PushNotificationClients.Contains(new PushClient(client.uri)))
            {
                DataCache.Instance.PushNotificationClients.Add(new PushClient(client.uri));

                var repo = new PushClientRepository();
                repo.Add(client.uri);
            }
        }

    }
}