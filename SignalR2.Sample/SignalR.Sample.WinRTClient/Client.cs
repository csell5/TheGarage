using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SignalR.Sample.WinRTClient
{
    public class Client
    {
        public static MainPage Page { get; set; }
        public static SynchronizationContext Context { get; set; }

        public async void RunAsync()
        {
            HubConnection connection = new HubConnection("http://localhost:18628/");
            IHubProxy proxy = connection.CreateHubProxy("SampleHub");
            proxy.On<FromServerToClientData>("Broadcast", Broadcast);
            proxy.On<string>("BroadcastToGroup", BroadcastToGroup);
            proxy.On<FromServerToClientData>("OthersCallback", OthersCallback);
            await connection.Start();

            FromServerToClientData request = new FromServerToClientData();
            request.Text = "This is a request from a WindowsStoreApp!";

            proxy.Invoke<FromClientToServerData>("Request", request).ContinueWith(task =>
            {
                FromClientToServerData response = task.Result;
                Context.Post(delegate
                {
                    TextBox textBox = (TextBox)Page.FindName("ResponseText");
                    textBox.Text = string.Format("Response: {0}", response.Text);
                }, null);
            });

            proxy.Invoke<FromClientToServerData>("RequestAsync", request).ContinueWith(task =>
            {
                FromClientToServerData response = task.Result;
                Context.Post(delegate
                {
                    TextBox textBox = (TextBox)Page.FindName("ResponseAsyncText");
                    textBox.Text = string.Format("ResponseAsync: {0}", response.Text);
                }, null);
            });

            proxy.Invoke("RequestWithCallbackAsync", request);
            proxy.Invoke("JoinGroup", "WindowsStoreApp");
        }

        void Broadcast(FromServerToClientData data)
        {
            Context.Post(delegate
                {
                    TextBox textBox = (TextBox)Page.FindName("BroadcastText");
                    textBox.Text = string.Format("Broadcast: {0} {1} {2}", data.Now, data.Integer, data.Text);
                }, null);
        }

        void BroadcastToGroup(string value)
        {
            Context.Post(delegate
            {
                TextBox textBox = (TextBox)Page.FindName("BroadcastToGroupText");
                textBox.Text = string.Format("BroadcastToGroup: {0}", value);
            }, null);
        }

        void OthersCallback(FromServerToClientData data)
        {
            Context.Post(delegate
            {
                TextBox textBox = (TextBox)Page.FindName("OthersCallbackText");
                textBox.Text = string.Format("OthersCallback: {0}", data.Text);
            }, null);
        }
    }
}
