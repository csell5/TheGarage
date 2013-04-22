using MarkNic.JsonSupport.Interfaces;

namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public interface IGarageLight : IJsonSerialize, IComponentStatus
    {
        /// <summary>
        /// Name of the garage light ("Indoors", "Outdoors", etc.)
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Initialize (force) the circuit (relay) to "Off"
        /// </summary>
        bool CircuitRelayOff();

        /// <summary>
        /// Switch the circuit to the provided value on/true or off/false
        /// </summary>
        /// <param name="turnOn">on/off indicator (true = on)</param>
        bool SwitchCircuit(bool turnOn);

        /// <summary>
        /// Turn the existing circuit off
        /// </summary>
        bool CircuitOff();

        /// <summary>
        /// Turn the existing circuit on
        /// </summary>
        bool CircuitOn();
    }
}