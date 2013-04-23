using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using Dapper;
using Microsoft.WindowsAzure;

namespace theGarage.PushNotifications
{
    public class PushClientRepository
    {
        string _connString = string.Empty;

        public PushClientRepository()
        {
            _connString = CloudConfigurationManager.GetSetting("dbContext");
        }

        public List<PushClient> GetAll()
        {
            List<PushClient> clientUris = null;

            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                clientUris = cn.Query<PushClient>("SELECT * FROM PushClients").ToList();

                cn.Close();
            }

            return clientUris;
        }

        public void Add(string clientUri)
        {
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                var result = cn.Execute(string.Format("INSERT INTO PushClients (ClientUrl) VALUES ('{0}')", clientUri));

                cn.Close();
            }
        }

        public void DeleteClient(string clientUri)
        {
            using (var cn = new SqlConnection(_connString))
            {
                cn.Open();

                var result = cn.Execute(string.Format("DELETE FROM PushClients WHERE ClientUrl = '{0}'", clientUri));

                cn.Close();
            }
        }
    }
}