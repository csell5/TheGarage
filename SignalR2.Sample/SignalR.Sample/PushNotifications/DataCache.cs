using System;
using System.Collections.Generic;

namespace theGarage.PushNotifications
{
    public sealed class DataCache
    {
        private DataCache()
        {
            var pushRepo = new PushClientRepository();
            _pushClients = pushRepo.GetAll();
        }

        private static volatile DataCache instance;
        private static object syncRoot = new Object();

        private static List<PushClient> _pushClients;
        public List<PushClient> PushNotificationClients
        {
            get { return _pushClients; }
            set { _pushClients = value; }
        }

        public static DataCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataCache();
                    }
                }

                return instance;
            }
        }
    }
}