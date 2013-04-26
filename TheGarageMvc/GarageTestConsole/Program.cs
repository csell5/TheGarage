using System.Web.Security;
using GarageTestConsole.Devices;
using GarageTestConsole.Security;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace GarageTestConsole
{
    public enum RequestType
    {
        SignOn = 0,
        Info = 1,
        Get = 2,
        Set = 3
    }

    public class Response
    {
        public int Version { get; set; }
        public string Payload { get; set; }
    }

    /// <summary>
    /// Server to Client Communication (Requesting Data)
    /// </summary>
    //public class Request
    //{
    //    public int Version { get; set; }
    //    public RequestType MessageType { get; set; }
    //    public string Payload { get; set; }
    //}

    public class SignOnRequest
    {
        public string LocationId { get; set; }
        public string Identity { get; set; }
    }

    public class Device
    {
        public Device(string name, string deviceType, string deviceUrl)
        {
            Name = name;
            DeviceType = deviceType;
            DeviceUrl = deviceUrl;
        }

        public string Name { get; private set; }
        public string DeviceType { get; private set; }
        public string DeviceUrl { get; private set; }
    }

    public class Application
    {
        public static HubConnection Connection { get; private set; }
        public static IHubProxy Proxy { get; private set; }
        public static Garage LastGarageStatus { get; set; }

        public static readonly Dictionary<string, Device> DeviceList = new Dictionary<string, Device>();

        static void ActivateDoor(int id, string command)
        {
            var commandToSend = new GarageComponentCommand { component = "door", componentNumber = id, command = command };
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        static void ActivateLight(int id, string command)
        {
            var commandToSend = new GarageComponentCommand { component = "light", componentNumber = id, command = command };
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        static void ActivateSoftLock(string command)
        {
            var commandToSend = new GarageComponentCommand { component = "softlock", componentNumber = 0, command = command };
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        static void RequestStatus(string id)
        {
            Proxy.Invoke("OnLockChange", LastGarageStatus.Name, LastGarageStatus.Locked, LastGarageStatus.HardwareLock, LastGarageStatus.SoftLock);

            for (var i = 0; i < LastGarageStatus.Door.Length; i++)
                Proxy.Invoke("OnDoorChange", i, LastGarageStatus.Door[i].Status);

            for (var i = 0; i < LastGarageStatus.Light.Length; i++)
                Proxy.Invoke("OnLightChange", i, LastGarageStatus.Light[i].Status);
        }

        
        static void ReadConfiguration()
        {
            if (!File.Exists("Device.config")) return;

            var configDoc = XDocument.Load("Device.config");

            foreach (var device in configDoc.Descendants("Device"))
            {
                DeviceList.Add(device.Attribute("name").Value,
                               new Device(device.Attribute("name").Value, device.Attribute("deviceType").Value,
                                          device.Attribute("deviceUrl").Value));
            }
        }
        

        private static void Main()
        {
            //var status = HttpHelper.GetStatus("http://10.0.0.39/status");

            ReadConfiguration();

            var parentHub = ConfigurationManager.AppSettings["ParentHub"];
            var locationId = ConfigurationManager.AppSettings["LocationId"];
            var key = Convert.FromBase64String(ConfigurationManager.AppSettings["SharedSecret"]);
            var initializationVector = Convert.FromBase64String(ConfigurationManager.AppSettings["SharedIV"]);
            var pw = ConfigurationManager.AppSettings["Password"];

            // Connect to the service
            //Connection = new HubConnection(parentHub);

            Connection = new HubConnection(parentHub);
            //{
            //    CookieContainer = new CookieContainer()
            //};

            //var cookie = GetAuthCookie(parentHub, "marknic@marknic.net", "pass@word1");

            //Connection.CookieContainer.Add(cookie);


            Proxy = Connection.CreateHubProxy("DeviceCommunicationHub");

            Proxy.On<int, string>("ActivateDoor", ActivateDoor);
            Proxy.On<int, string>("ActivateLight", ActivateLight);
            Proxy.On<string>("ActivateSoftLock", ActivateSoftLock);
            Proxy.On<string>("RequestStatus", RequestStatus);

            var startTask = Connection.Start();
                
            startTask.Wait();

            var identityPayload = new DeviceHubIdentity(new NetworkCredential { UserName = locationId, Password = pw }, Connection.ConnectionId)
                .EncryptAndEncode(key, initializationVector);

            var signOnRequest = new SignOnRequest { LocationId = locationId, Identity = identityPayload };

            Proxy.Invoke<Response>("SignOn", signOnRequest).ContinueWith(task =>
            {
                var response = task.Result;
                Console.WriteLine("Response: {0}", response.Payload);
            });

            BackgroundThread.Start();
            //proxy.Invoke("JoinGroup", "ConsoleApp").ContinueWith(task => Console.WriteLine("JoinGroup completed"));

            Console.ReadLine();


        }

        private static Cookie GetAuthCookie(string baseUrl, string user, string pass)
        {
            var http = WebRequest.Create(baseUrl + "Account/Login") as HttpWebRequest;
            http.AllowAutoRedirect = false;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            http.CookieContainer = new CookieContainer();
            var postData = "UserName=" + user + "&Password=" + pass + "&RememberMe=false&returnUrl=http://localhost/";
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(postData);
            http.ContentLength = dataBytes.Length;
            using (var postStream = http.GetRequestStream())
            {
                postStream.Write(dataBytes, 0, dataBytes.Length);
            }
            var httpResponse = http.GetResponse() as HttpWebResponse;
            var cookie = httpResponse.Cookies[FormsAuthentication.FormsCookieName];
            httpResponse.Close();
            return cookie;
        }

        //private static void SignalRMessage(string obj)
        //{
        //    Console.WriteLine(obj);
        //}
    }

    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
