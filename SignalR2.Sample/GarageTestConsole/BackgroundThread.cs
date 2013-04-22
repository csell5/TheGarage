using System;
using System.Threading;
using GarageTestConsole.Devices;
using Newtonsoft.Json;

namespace GarageTestConsole
{
    public class BackgroundThread
    {
        public static void Start()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    try
                    {
                        foreach (var device in Application.DeviceList)
                        {
                            var status = HttpHelper.GetStatus(device.Value.DeviceUrl + "status");

                            var garage = JsonConvert.DeserializeObject<Garage>(status);

                            Console.WriteLine(status);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError("SignalR error thrown: {0}", ex);
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            // ReSharper disable FunctionNeverReturns
            });
            // ReSharper restore FunctionNeverReturns
        }
    }
}
