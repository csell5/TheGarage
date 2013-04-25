using System;
using Marknic.NdGarageDoorLightsController.Interfaces;
using Marknic.NdGarageDoorLightsController.Utility;
using Microsoft.SPOT.Hardware;

namespace Marknic.NdGarageDoorLightsController
{
    /// <summary>
    /// Manages a switched circuit through a relay
    /// </summary>
    public class GarageLight : IGarageLight
    {
        private readonly string _switchPortName;
        private readonly string _relayPortName;

        public string Name { get; private set; }

        public IStatus Status { get; private set; }

        public bool CircuitOnValue { get; private set; }

        /// <summary>
        /// Create a switched circuit using a relay
        /// </summary>
        /// <param name="lightNumber">Index number of the light</param>
        /// <param name="name">Name of the light circuit</param>
        /// <param name="switchPin">input pin used to determine the circuit switch value</param>
        /// <param name="relayPin">output pin used to control the circuit relay</param>
        public GarageLight(int lightNumber, string name, Cpu.Pin switchPin, Cpu.Pin relayPin)
            : this(lightNumber, name, switchPin, relayPin, true)
        {
        }

        /// <summary>
        /// Create a switched circuit using a relay
        /// </summary>
        /// <param name="lightNumber">Index number of the light</param>
        /// <param name="name">Name of the light circuit</param>
        /// <param name="switchPin">input pin used to determine the circuit switch value</param>
        /// <param name="relayPin">output pin used to control the circuit relay</param>
        /// <param name="initialState">Should the switch circuit be turned on during start up</param>
        public GarageLight(int lightNumber, string name, Cpu.Pin switchPin, Cpu.Pin relayPin, bool initialState)
        {
            _switchPortName = "SwitchPort" + lightNumber;
            _relayPortName = "RelayPort" + lightNumber;

            Name = name;

            var switchPort = PortManager.GetInPort(_switchPortName) ??
                              PortManager.CreateInputPort(_switchPortName, switchPin, false, Port.ResistorMode.PullUp);

            if (PortManager.GetOutPort(_relayPortName) == null)
            {
                PortManager.CreateOutputPort(_relayPortName, relayPin, !initialState);
            }

            CircuitOnValue = initialState ? switchPort.Read() : !switchPort.Read();

            Status = new Status(initialState ? StatusValues.LightOff : StatusValues.LightOn, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Initialize (force) the circuit (relay) to "Off"
        /// </summary>
        public bool CircuitRelayOff()
        {
            var switchPort = PortManager.GetInPort(_switchPortName);
            var relayPort = PortManager.GetOutPort(_relayPortName);

            if ((switchPort == null) || (relayPort == null)) return false;

            CircuitOnValue = !switchPort.Read();

            relayPort.Write(true);

            return true;
        }

        /// <summary>
        /// Switch the circuit to the provided value on/true or off/false
        /// </summary>
        /// <param name="turnOn">on/off indicator (true = on)</param>
        public bool SwitchCircuit(bool turnOn)
        {
            var switchPort = PortManager.GetInPort(_switchPortName);
            var relayPort = PortManager.GetOutPort(_relayPortName);

            if ((switchPort == null) || (relayPort == null)) return false;

            var switchPortValue = switchPort.Read();

            // Switch the relay
            relayPort.Write(turnOn);

            // Set the "On" value
            CircuitOnValue = !turnOn ? switchPortValue : !switchPortValue;

            return true;
        }

        /// <summary>
        /// Turn the existing circuit off
        /// </summary>
        public bool CircuitOff()
        {
            return SwitchCircuit(true);
        }

        /// <summary>
        /// Turn the existing circuit on
        /// </summary>
        public bool CircuitOn()
        {
            return SwitchCircuit(false);
        }

        public IStatus UpdateStatus()
        {
            var switchPort = PortManager.GetInPort(_switchPortName);

            if (switchPort == null) return null;

            var switchVal = switchPort.Read();
            var newStatus = StatusValues.LightOff;

            if (switchVal == CircuitOnValue)
            {
                CircuitOn();
                newStatus = StatusValues.LightOn;
            }
            else
            {
                CircuitOff();
            }

            Status.UpdateState(newStatus, DateTime.Now.Ticks);

            return Status;
        }

        public string SerializeJson()
        {
            return "{\"Name\": \"" + Name + "\", \"Status\": \"" + Status.State + "\", \"Duration\": " + TimeUtility.ConvertTimeSpanToSeconds(Status.Duration) + " }";
        }
    }
}