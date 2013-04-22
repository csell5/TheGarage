using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace GarageTestConsole
{
    public static class HttpHelper
    {
        public static string GetStatus(string url)
        {
            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;

            if (httpWebRequest == null) return null;

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            var responseStream = httpResponse.GetResponseStream();

            if (responseStream == null) return null;

            using (var streamReader = new StreamReader(responseStream))
            {
                var responseText = streamReader.ReadToEnd();
                Debug.WriteLine("Response Text: " + responseText);

                return responseText;
            }
        }


        public static void SendJsonCommand(string url, string jsonToSend)
        {
            // Create a request for the URL. 		
            var httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            //var request = WebRequest.Create("http://10.0.0.39/status") as HttpWebRequest;

            if (httpWebRequest == null) return;

            httpWebRequest.Method = WebRequestMethods.Http.Post;
            httpWebRequest.ServicePoint.Expect100Continue = false;
            httpWebRequest.Timeout = 20000;

            httpWebRequest.ContentType = "application/json";

            //var json = "{\"component\":\"light\",\"componentNumber\":1,\"command\":\"off\"}";
            var encoding = new ASCIIEncoding();

            var jsonData = encoding.GetBytes(jsonToSend);

            httpWebRequest.ContentLength = jsonData.Length;

            using (var stream = httpWebRequest.GetRequestStream())
            {
                stream.Write(jsonData, 0, jsonData.Length);
            }

            var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();

            var responseStream = httpResponse.GetResponseStream();

            if (responseStream == null) return;

            using (var streamReader = new StreamReader(responseStream))
            {
                var responseText = streamReader.ReadToEnd();
                Debug.WriteLine("Response Text: " + responseText);
            }
        }
    }
}
