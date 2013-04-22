using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Xml.Linq;
using GarageTestConsole.Security;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;

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
    public class Request
    {
        public int Version { get; set; }
        public RequestType MessageType { get; set; }
        public string Payload { get; set; }
    }

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

    class Application
    {
        public static readonly Dictionary<string, Device> DeviceList = new Dictionary<string, Device>(); 

        static void ActivateDoor(int id, string command)
        {
            var commandToSend = new GarageComponentCommand {component = "door", componentNumber = id, command = command};
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(Application.DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        static void ActivateLight(int id, string command)
        {
            var commandToSend = new GarageComponentCommand { component = "light", componentNumber = id, command = command };
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(Application.DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        static void ActivateSoftLock(string command)
        {
            var commandToSend = new GarageComponentCommand { component = "softlock", componentNumber = 0, command = command };
            var jsonToSend = JsonConvert.SerializeObject(commandToSend);

            HttpHelper.SendJsonCommand(Application.DeviceList["Garage 1"].DeviceUrl + "trigger", jsonToSend);

            Console.WriteLine("JSON Sent to Garage: " + jsonToSend);
        }

        //static void Broadcast(FromServerToClientData data)
        //{
        //    Console.WriteLine("Broadcast: {0} {1} {2}", data.Now, data.Integer, data.Text);
        //}

        //static void BroadcastToGroup(string value)
        //{
        //    Console.WriteLine("BroadcastToGroup: {0}", value);
        //}

        //static void OthersCallback(FromServerToClientData data)
        //{
        //    Console.WriteLine("Command Called: {0} - {1}", data.Text, data.Command);

        //    //HttpHelper.SendJsonCommand(Application.DeviceList["Garage 1"].DeviceUrl + "trigger", data.Command);
        //}

        static void ReadConfiguration()
        {
            if (File.Exists("Device.config"))
            {
                var configDoc = XDocument.Load("Device.config");

                foreach (var device in configDoc.Descendants("Device"))
                {
                    DeviceList.Add(device.Attribute("name").Value,
                                    new Device(device.Attribute("name").Value, device.Attribute("deviceType").Value,
                                               device.Attribute("deviceUrl").Value));
                }
                

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
            var connection = new HubConnection(parentHub);
            var proxy = connection.CreateHubProxy("DeviceCommunicationHub");

            proxy.On<int, string>("ActivateDoor", ActivateDoor);
            proxy.On<int, string>("ActivateLight", ActivateLight);
            proxy.On<string>("ActivateSoftLock", ActivateSoftLock);

            //proxy.On<FromServerToClientData>("Broadcast", Broadcast);
            //proxy.On<string>("BroadcastToGroup", BroadcastToGroup);
            //proxy.On<FromServerToClientData>("OthersCallback", OthersCallback);
            connection.Start().Wait();

            var identityPayload = new DeviceHubIdentity(new NetworkCredential { UserName = locationId, Password = pw }, connection.ConnectionId)
                .EncryptAndEncode(key, initializationVector);

            var signOnRequest = new SignOnRequest { LocationId = locationId, Identity = identityPayload };

            proxy.Invoke<Response>("SignOn", signOnRequest).ContinueWith(task =>
            {
                var response = task.Result;
                Console.WriteLine("Response: {0}", response.Payload);
            });

            BackgroundThread.Start();
            //proxy.Invoke("JoinGroup", "ConsoleApp").ContinueWith(task => Console.WriteLine("JoinGroup completed"));

            Console.ReadLine();

            
        }

        //private static void SignalRMessage(string obj)
        //{
        //    Console.WriteLine(obj);
        //}
    }
}
