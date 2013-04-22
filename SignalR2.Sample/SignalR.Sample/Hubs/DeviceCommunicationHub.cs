using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SignalR.Sample.HubModels;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SignalR.Sample.Hubs
{
    public class DeviceCommunicationHub : Hub
    {
        public byte[] Key { get; set; }
        public byte[] InitializationVector { get; set; }
        public string Password { get; set; }
        public static string ConsoleKey { get; set; }

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
        }

        public void OnLightChange(int id, string command)
        {
            Clients.All.OnLightChange(id, command);
        }

        public void OnLockChange(string name, bool garage, bool hardlock, bool softlock)
        {
            Clients.All.OnLockChange(name, garage, hardlock, softlock);
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

        [Obsolete("Moved to the onconnected for the connection", true)]
        public void RequestStatus(string id)
        {
            Clients.Client(ConsoleKey).RequestStatus(id);
        }

        public override Task OnConnected()
        {
            Clients.Client(ConsoleKey).RequestStatus(base.Context.ConnectionId);
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

        public async Task<FromServerToClientData> RequestAsync(FromClientToServerData request)
        {
            var response = new FromServerToClientData { Text = "Responding to: " + request.Text };

            await Task.Delay(TimeSpan.FromSeconds(1));

            return response;
        }

        public async Task RequestWithCallbackAsync(FromClientToServerData request)
        {
            var response = new FromServerToClientData { Text = "Responding to: " + request.Text };

            await Task.Delay(TimeSpan.FromSeconds(5));

            Clients.Others.OthersCallback(response);
        }

        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }
    }

}