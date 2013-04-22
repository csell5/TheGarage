using Microsoft.AspNet.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalR.Sample.Hubs
{
    public class SampleHub : Hub
    {     
        public FromServerToClientData Request(FromClientToServerData request)
        {
            FromServerToClientData response = new FromServerToClientData();
            response.Text = "Responding to: " + request.Text;
            
            return response;
        }

        public async Task<FromServerToClientData> RequestAsync(FromClientToServerData request)
        {
            FromServerToClientData response = new FromServerToClientData();
            response.Text = "Responding to: " + request.Text;

            await Task.Delay(TimeSpan.FromSeconds(5));

            return response;
        }

        public async Task RequestWithCallbackAsync(FromClientToServerData request)
        {
            FromServerToClientData response = new FromServerToClientData();
            response.Text = "Responding to: " + request.Text;

            await Task.Delay(TimeSpan.FromSeconds(5));

            Clients.Others.OthersCallback(response);
        }

        public void JoinGroup(string groupName)
        {
            Groups.Add(Context.ConnectionId, groupName);
        }
    }
}