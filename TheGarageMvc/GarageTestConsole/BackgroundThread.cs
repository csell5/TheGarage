﻿using GarageTestConsole.Devices;
using Newtonsoft.Json;
using System;
using System.Threading;

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

                            if (Application.LastGarageStatus == null)
                            {
                                Application.LastGarageStatus = new Garage();

                                Garage.Copy(garage, Application.LastGarageStatus);

                                Application.Proxy.Invoke("OnLockChange", garage.Name, garage.Locked, garage.HardwareLock, garage.SoftLock);

                                for (var i = 0; i < garage.Door.Length; i++)
                                {
                                    Application.Proxy.Invoke("OnDoorChange", i, garage.Door[i].Status);
                                }

                                for (var i = 0; i < garage.Light.Length; i++)
                                {
                                    Application.Proxy.Invoke("OnLightChange", i, garage.Light[i].Status);
                                }
                            }
                            else
                            {
                                if (garage.IsLockChanged(Application.LastGarageStatus))
                                {
                                    Application.Proxy.Invoke("OnLockChange", garage.Name, garage.Locked, garage.HardwareLock, garage.SoftLock);
                                }

                                if (garage.IsDoorChanged(Application.LastGarageStatus))
                                {
                                    for (var i = 0; i < garage.Door.Length; i++)
                                    {
                                        Application.Proxy.Invoke("OnDoorChange", i, garage.Door[i].Status);
                                    }
                                }

                                if (garage.IsLightChanged(Application.LastGarageStatus))
                                {
                                    for (var i = 0; i < garage.Light.Length; i++)
                                    {
                                        Application.Proxy.Invoke("OnLightChange", i, garage.Light[i].Status);
                                    }
                                }
                            }

                            Garage.Copy(garage, Application.LastGarageStatus);

                            //Console.WriteLine(status);
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
