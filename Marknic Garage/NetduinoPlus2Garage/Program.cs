using System.Threading;
using Marknic.NdGarageDoorLightsController;
using Marknic.NdGarageDoorLightsController.Utility;
using Marknic.Web;
using Marknic.Web.Interfaces;
using Marknic.Web.Utility;
using Microsoft.SPOT;

namespace NetduinoPlus2Garage
{
    public class Program
    {
        private static WebServerCommandProcessor _webServerCommandProcessor;
        
        private static EventBasedTimer _eventTimer;

        public static void Main()
        {
            //var date = HttpUtility.NtpTime("time-a.nist.gov", -6);

            //DateTime.Now.SetFromNetwork(new TimeSpan(-5, 0, 0));

            HardwareSetup();

            // Instantiate a new web server on port 80.
            _webServerCommandProcessor = new WebServerCommandProcessor(new WebServer(80));

            // Start the web server.
            _webServerCommandProcessor.WebServer.Start();

            // Set the local event that will execute when a command is received
            _webServerCommandProcessor.ExecuteCommandRequest = ExecuteCommandRequest;
            
            // write your code here
            _eventTimer = new EventBasedTimer(5000, TimerExecutionType.Constant, CurrentStateTimerTick);
            _eventTimer.Start();

            while (true)
            {
                //Debug.Print("Netduino still running...");
                Thread.Sleep(10000);
            }

            //Thread.Sleep(50);

            //Debug.Print("I'm Done.");
        
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        /// <summary>
        /// Set up the garage hardware from the configuration file
        /// </summary>
        private static void HardwareSetup()
        {
            // Get the configuration of the garage (Doors/Lights/Etc.)
            var filename = FileUtility.FileStore + WebServerCommandProcessor.ControllerConfigFile;
            
            // Set up the controller by reading the configuration file
            //  Newly created garage controller is accessed via static property: GarageController.Garage
            GarageController.ReadXmlLocal(filename);

            //var json = GarageController.Garage.SerializeJson();
        }

        /// <summary>
        /// Delegate that is called periodically (local timer) to check the current state of the hardware
        /// </summary>
        /// <param name="timer">source timer that made the call</param>
        static void CurrentStateTimerTick(EventBasedTimer timer)
        {
            GarageController.Garage.UpdateStatus();

            Debug.Print("Current State of " + GarageController.Garage.Name + " is: " + GarageController.Garage.Status.State);

            for (var i = 0; i < GarageController.Garage.DoorCount; i++)
            {
                var door = GarageController.Garage.GetDoor(i);

                Debug.Print("Current State of door '" + door.Name + "' is " + door.Status.State);
            }

            for (var i = 0; i < GarageController.Garage.LightCount; i++)
            {
                var light = GarageController.Garage.GetLight(i);

                Debug.Print("Current State of light '" + light.Name + "' is " + light.Status.State);
            }

            Debug.Print("Controller still running...");

            Debug.Print("running thread count: " + ThreadState.Background);
            Debug.Print("Free Memory: " + Debug.GC(true));

        }

        #region Web Server Request Routine

        /// <summary>
        /// Delegate called by the Web Server Command Processor to handle command requests and manupulate the actual hardware
        /// </summary>
        /// <param name="commandRequest">command to process</param>
        public static void ExecuteCommandRequest(ICommandRequest commandRequest)
        {
            //Debug.Print("Execute Command: " + commandRequest.Command);
            
            // Make sure we have the latest status of the hardware
            GarageController.Garage.UpdateStatus();

            switch (commandRequest.Component.ToLower())
            {
                case GarageComponents.SoftLock:
                    switch (commandRequest.Command)
                    {
                        case GarageCommands.Lock:
                            GarageController.Garage.SoftLock = true;
                            break;

                        case GarageCommands.Unlock:
                            GarageController.Garage.SoftLock = false;
                            break;
                    }

                    break;

                // Re-initialize the controller card if the configuration has been changed
                case GarageComponents.Garage:
                    if (commandRequest.Command == GarageCommands.Reset)
                    {
                        HardwareSetup();
                    }
                    break;

                case GarageComponents.Door:
                    if (!(GarageController.Garage.LockoutIsActivated || GarageController.Garage.SoftLock))
                    {
                        var door = GarageController.Garage.GetDoor(commandRequest.ComponentNumber);

                        var activateDoor = false;

                        switch (commandRequest.Command)
                        {
                            case GarageCommands.DoorCommandOpen:
                                activateDoor = door.Status.State == StatusValues.DoorDown;
                                break;

                            case GarageCommands.DoorCommandClose:
                                activateDoor = door.Status.State == StatusValues.DoorUp;
                                break;

                            case GarageCommands.DoorCommandToggle:
                                activateDoor = true;
                                break;
                        }

                        if (activateDoor)
                        {
                            GarageController.Garage.ActivateDoorSwitch(commandRequest.ComponentNumber);
                        }
                    }
                    break;

                case GarageComponents.Light:
                        switch (commandRequest.Command)
                        {
                            case GarageCommands.LightCommandOff:
                                GarageController.Garage.DeactivateLightSwitch(commandRequest.ComponentNumber);
                                break;

                            case GarageCommands.LightCommandOn:
                                GarageController.Garage.ActivateLightSwitch(commandRequest.ComponentNumber);
                                break;
                        }
                    break;
            }
        }
        
        #endregion
    }
}
