using MarkNic.JsonSupport.Interfaces;

namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public interface IGarageDoor : IJsonSerialize, IComponentStatus
    {
        string Name { get; }

        /// <summary>
        /// Turns on the door switch relay for a certain amount of time
        /// </summary>
        void ActivateDoorSwitch();

        /// <summary>
        /// Will activate the door as long as the door isn't already in the desired location
        /// </summary>
        /// <param name="upSwitch">true = up / false = down</param>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        bool SwitchDoor(bool upSwitch);

        /// <summary>
        /// Attempts to open the door (as long as the door isn't already in the desired location)
        /// </summary>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        bool OpenDoor();

        /// <summary>
        /// Attempts to close the door (as long as the door isn't already in the desired location)
        /// </summary>
        /// <returns>true if the door is switched - false if the command is ignored</returns>
        bool CloseDoor();

        /// <summary>
        /// Returns the current value of the door's top sensor
        /// </summary>
        /// <returns>true/false</returns>
        bool TopSensorRead();

        /// <summary>
        /// Returns the current value of the door's bottom sensor
        /// </summary>
        /// <returns>true/false</returns>
        bool BottomSensorRead();
    }
}