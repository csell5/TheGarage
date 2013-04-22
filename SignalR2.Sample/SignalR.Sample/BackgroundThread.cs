using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading;
using SignalR.Sample.Hubs;

namespace SignalR.Sample
{
    public class BackgroundThread
    {     
        private static Random _random = new Random();

        public static void Start()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<SampleHub>();
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<DeviceCommunicationHub>();

                while (true)
                {
                    FromServerToClientData data = new FromServerToClientData();
                    data.Now = DateTime.Now;
                    data.Integer = _random.Next(100);
                    data.Text = new string('a', data.Integer);

                    try
                    {
                        //hubContext.Clients.All.Broadcast(data);
                        //hubContext.Clients.Group("WebApp").BroadcastToGroup(string.Format("WebApp {0}", DateTime.Now));
                        //hubContext.Clients.Group("ConsoleApp").BroadcastToGroup(string.Format("ConsoleApp {0}", DateTime.Now));
                        //hubContext.Clients.Group("WindowsStoreApp").BroadcastToGroup(string.Format("WindowsStoreApp {0}", DateTime.Now));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("SignalR error thrown: {0}", ex);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            });
        }
    }
}