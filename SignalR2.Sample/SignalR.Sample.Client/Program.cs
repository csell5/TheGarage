using Microsoft.AspNet.SignalR.Client.Hubs;
using System;

namespace SignalR.Sample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"
  Quick steps to write a SignalR .NET client

  1. Create a ""Console Application"" project
  2. Add NuGet packages:
      - Microsoft ASP.NET SignalR Client
  3. Create your data types (DataModel.cs)
  4. Create your SignalR Client (Program.cs)
            ");

            HubConnection connection = new HubConnection("http://localhost:18628/");
            //IHubProxy proxy = connection.CreateHubProxy("SampleHub");
            IHubProxy proxy = connection.CreateHubProxy("DeviceCommunicationHub");
            proxy.On<FromServerToClientData>("Broadcast", Broadcast);
            proxy.On<string>("BroadcastToGroup", BroadcastToGroup);
            proxy.On<FromServerToClientData>("OthersCallback", OthersCallback);
            connection.Start().Wait();

            FromServerToClientData request = new FromServerToClientData();
            request.Text = "This is a request from a ConsoleApp!";

            proxy.Invoke<FromClientToServerData>("Request", request).ContinueWith(task =>
            {
                FromClientToServerData response = task.Result;
                Console.WriteLine("Response: {0}", response.Text);
            });

            proxy.Invoke<FromClientToServerData>("RequestAsync", request).ContinueWith(task =>
            {
                FromClientToServerData response = task.Result;
                Console.WriteLine("ResponseAsync: {0}", response.Text);
            });

            proxy.Invoke("RequestWithCallbackAsync", request).ContinueWith(task =>
            {
                Console.WriteLine("RequestWithCallbackAsync completed");
            });

            proxy.Invoke("JoinGroup", "ConsoleApp").ContinueWith(task =>
            {
                Console.WriteLine("JoinGroup completed");
            });

            Console.ReadLine();
        }

        static void Broadcast(FromServerToClientData data)
        {
            Console.WriteLine("Broadcast: {0} {1} {2}", data.Now, data.Integer, data.Text);
        }

        static void BroadcastToGroup(string value)
        {
            Console.WriteLine("BroadcastToGroup: {0}", value);
        }

        static void OthersCallback(FromServerToClientData data)
        {
            Console.WriteLine("OthersCallback: {0}", data.Text);
        }
    }
}
