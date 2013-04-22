using System;
using System.Threading;
using Marknic.NdGarageDoorLightsController.Interfaces;
using Marknic.NdGarageDoorLightsController.Utility;
using Microsoft.SPOT.Hardware;

namespace Marknic.NdGarageDoorLightsController
{
    /// <summary>
    /// Class to manage a garage door using up/down sensors
    /// </summary>
    public class GarageDoor : IGarageDoor
    {
        public string TopSensorName { get; set; }
        public string BottomSensorName { get; set; }
        public string DoorSwitchName { get; set; }
        public string Name { get; private set; }
        public IStatus Status { get; private set; }



        /// <summary>
        /// ctor accepting the upper, lower and door switch pins and creates the appropriate ports
        /// </summary>
        /// <param name="doorNumber">Number of door</param>
        /// <param name="name">Name of the garage door</param>
        /// <param name="topSensorPin">Pin to use as input for the upper garage door sensor</param>
        /// <param name="bottomSensorPin">Pin to use as input for the lower garage door sensor</param>
        /// <param name="doorSwitchPin">Pin to use as output to switch the relay that will open/close the garage door</param>
        public GarageDoor(int doorNumber, string name, Cpu.Pin topSensorPin, Cpu.Pin bottomSensorPin, Cpu.Pin doorSwitchPin)
        {
            TopSensorName = "TopSensor" + doorNumber;
            BottomSensorName = "BottomSensor" + doorNumber;
            DoorSwitchName = "DoorSwitch" + doorNumber;
            
            Name = name;

            var topSensor = PortManager.GetInPort(TopSensorName);

            if (topSensor == null)
            {
                PortManager.CreateInputPort(TopSensorName, topSensorPin, false, Port.ResistorMode.Disabled);
            }

            var bottomSensor = PortManager.GetInPort(BottomSensorName);

            if (bottomSensor == null)
            {
                PortManager.CreateInputPort(BottomSensorName, bottomSensorPin, false, Port.ResistorMode.Disabled);
            }

            var zoorSwitch = PortManager.GetOutPort(DoorSwitchName);

            if (zoorSwitch == null)
            {
                PortManager.CreateOutputPort(DoorSwitchName, doorSwitchPin, false);
            }

            Status = new Status(StatusValues.DoorUnknown, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Turns on the door switch relay for a certain amount of time
        /// </summary>
        public void ActivateDoorSwitch()
        {
            var doorSwitch = PortManager.GetOutPort(DoorSwitchName);

            if (doorSwitch == null) return;

            doorSwitch.Write(true);

            Thread.Sleep(500);

            doorSwitch.Write(false);
        }

        /// <summary>
        /// Will activate the door as long as the door isn't already in the desired location
        /// </summary>
        /// <param name="upSwitch">true = up / false = down</param>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        public bool SwitchDoor(bool upSwitch)
        {
            var topSensor = PortManager.GetInPort(TopSensorName);
            var bottomSensor = PortManager.GetInPort(BottomSensorName);

            if ((topSensor == null) || (bottomSensor == null)) { return false; }

            var isDoorUp = topSensor.Read();
            var isDoorDown = bottomSensor.Read();

            if ((upSwitch && isDoorUp) || (!upSwitch && isDoorDown))
            {
                return false;
            }

            ActivateDoorSwitch();

            return true;
        }

        /// <summary>
        /// Attempts to open the door (as long as the door isn't already in the desired location)
        /// </summary>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        public bool OpenDoor()
        {
            return SwitchDoor(true);
        }

        /// <summary>
        /// Attempts to close the door (as long as the door isn't already in the desired location)
        /// </summary>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        public bool CloseDoor()
        {
            return SwitchDoor(false);
        }

        /// <summary>
        /// Returns the current value of the door's top sensor
        /// </summary>
        /// <returns>true/false</returns>
        public bool TopSensorRead()
        {
            var topSensor = PortManager.GetInPort(TopSensorName);
            
            return topSensor != null && topSensor.Read();
        }

        /// <summary>
        /// Returns the current value of the door's bottom sensor
        /// </summary>
        /// <returns>true/false</returns>
        public bool BottomSensorRead()
        {
            var bottomSensor = PortManager.GetInPort(BottomSensorName);

            return bottomSensor != null && bottomSensor.Read();
        }

        /// <summary>
        /// Update the current status of the door
        /// </summary>
        /// <returns></returns>
        public IStatus UpdateStatus()
        {
            var topSensor = PortManager.GetInPort(TopSensorName);
            var bottomSensor = PortManager.GetInPort(BottomSensorName);

            if ((topSensor == null) || (bottomSensor == null)) { return null; }
            
            var newStatus = StatusValues.DoorUnknown;

            var isDoorUp = !topSensor.Read();
            var isDoorDown = !bottomSensor.Read();

            if (isDoorUp && !isDoorDown)
            {
                newStatus = StatusValues.DoorUp;
            } 
            else if (isDoorDown && !isDoorUp)
            {
                newStatus = StatusValues.DoorDown;
            }

            Status.UpdateState(newStatus, DateTime.Now.Ticks);

            return Status;
        }

        public string SerializeJson()
        {
            return "{\"Name\": \"" + Name + "\", \"Status\": \"" + Status.State + "\", \"Duration\": " + Status.Duration.Seconds + " }";
        }
    }
}