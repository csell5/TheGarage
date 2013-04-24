using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;
using Dapper;

namespace TheGarageMvc.PushNotifications
{
    public class PushClientRepository
    {
        readonly string _connString = string.Empty;

        public PushClientRepository()
        {
             _connString = WebConfigurationManager.ConnectionStrings["dbContext"].ConnectionString;
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