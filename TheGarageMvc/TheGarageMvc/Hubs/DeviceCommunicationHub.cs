using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TheGarageMvc.HubModels;
using TheGarageMvc.PushNotifications;

namespace TheGarageMvc.Hubs
{
    public class DeviceCommunicationHub : Hub
    {
        public byte[] Key { get; set; }
        public byte[] InitializationVector { get; set; }
        public string Password { get; set; }
        public static string ConsoleKey { get; set; }

        readonly NotificationBroker _notificationBroker = new NotificationBroker();

        public DeviceCommunicationHub()
        {
            Key = Convert.FromBase64String(ConfigurationManager.AppSettings["SharedSecret"]);
            InitializationVector = Convert.FromBase64String(ConfigurationManager.AppSettings["SharedIV"]);
            Password = ConfigurationManager.AppSettings["Password"];
        }

        public FromServerToClientData Request(FromClientToServerData request)
        {
            var response = new FromServerToClientData { Text = "Responding to: " + request.Text };

            if (request.Text == "SendMessage")
            {
                var sendResponse = new FromServerToClientData { Text = "WooHoo!" };

                Clients.Others.OthersCallback(sendResponse);
            }

            if (request.Text == "SendConsole")
            {
                var consoleResponse = new FromServerToClientData { Text = "Calling Mr. Console!", Command = request.Command };

                Clients.Client(ConsoleKey).ActivateDoor(0, "on");

                Clients.Client(ConsoleKey).OthersCallback(consoleResponse);
            }

            return response;
        }

        public void OnDoorChange(int id, string command)
        {
            Clients.All.OnDoorChange(id, command);

            _notificationBroker.Send("Garage Door", command);
        }

        public void OnLightChange(int id, string command)
        {
            Clients.All.OnLightChange(id, command);

            switch (id)
            {
                case 0:
                    _notificationBroker.Send("Inside Lights", command);
                    break;

                case 1:
                    _notificationBroker.Send("Outside Lights", command);
                    break;
            }
        }

        public void OnLockChange(string name, bool garage, bool hardlock, bool softlock)
        {
            Clients.All.OnLockChange(name, garage, hardlock, softlock);

            if ( hardlock || softlock ) 
                _notificationBroker.Send("Garage", "Locked");
            else 
                _notificationBroker.Send("Garage", "UnLocked");
        }

        public void ActivateDoor(int id, string command)
        {
            Clients.Client(ConsoleKey).ActivateDoor(id, command);
        }

        public void ActivateLight(int id, string command)
        {
            Clients.Client(ConsoleKey).ActivateLight(id, command);
        }

        public void ActivateSoftLock(string command)
        {
            Clients.Client(ConsoleKey).ActivateSoftLock(command);
        }

        public override Task OnConnected()
        {
            Clients.Client(ConsoleKey).RequestStatus(Context.ConnectionId);
            return base.OnConnected();
        }

        public Response SignOn(SignOnRequest signOnRequest)
        {
            var response = new Response { Version = 1 };

            var identity = DeviceHubIdentity.Decrypt(signOnRequest.Identity, Key, InitializationVector);

            response.Payload = JsonConvert.SerializeObject(identity.Credential.Password == Password ? "Success" : "Unauthorized");

            ConsoleKey = identity.ConnectionId;

            return response;
        }

        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }
    }

}