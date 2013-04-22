using MarkNic.JsonSupport.Interfaces;

namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public interface IGarageController : IComponentStatus, IJsonSerialize
    {
        /// <summary>
        /// Get the number of doors
        /// </summary>
        int DoorCount { get; }

        /// <summary>
        /// Get the number of lights
        /// </summary>
        int LightCount { get; }

        /// <summary>
        /// Retrieves the GarageDoor object based on the number of the door
        /// </summary>
        /// <param name="doorNumber">door number to retrieve</param>
        /// <returns>garage door object</returns>
        IGarageDoor GetDoor(int doorNumber);

        /// <summary>
        /// Retrieves the GarageDoor object based on the name of the door
        /// </summary>
        /// <param name="doorName">name of the door to retrieve</param>
        /// <returns>garage door object if found - null if not</returns>
        IGarageDoor GetDoor(string doorName);

        /// <summary>
        /// Retrieves the Light object based on the number of the light
        /// </summary>
        /// <param name="lightNumber"></param>
        /// <returns></returns>
        IGarageLight GetLight(int lightNumber);

        /// <summary>
        /// Retrieves the Light object based on the name of the light
        /// </summary>
        /// <param name="lightName">name of the light to retrieve</param>
        /// <returns>light object if found - null if not</returns>
        IGarageLight GetLight(string lightName);

        /// <summary>
        /// turns on a light switch relay based on the number of the light
        /// </summary>
        /// <param name="lightNumber">light number to activate</param>
        void ActivateLightSwitch(int lightNumber);

        /// <summary>
        /// turns off a light switch relay based on the number of the light
        /// </summary>
        /// <param name="lightNumber">light number to deactivate</param>
        void DeactivateLightSwitch(int lightNumber);

        /// <summary>
        /// activates a garage switch based on the number of the door
        /// </summary>
        /// <param name="doorNumber">door number to activate</param>        
        void ActivateDoorSwitch(int doorNumber);

        /// <summary>
        /// Read-only property indicating if the lockout switch has been activated
        /// </summary>
        bool LockoutIsActivated { get; }

        /// <summary>
        /// Indicates if the upper (top) sensor is activated meaning the garage door is in the up position
        /// </summary>
        /// <param name="doorNumber">door number to check</param>
        /// <returns>true if the sensor is activated</returns>
        bool TopSensorIsActivated(int doorNumber);

        /// <summary>
        /// Indicates if the lower (bottom) sensor is activated meaning the garage door is in the down position
        /// </summary>
        /// <param name="doorNumber">door number to check</param>
        /// <returns>true if the sensor is activated</returns>
        bool BottomSensorIsActivated(int doorNumber);

        /// <summary>
        /// Indicator to determine if lights have been created in the controller
        /// </summary>
        /// <returns>true if lights have been created</returns>
        bool LightsExist();

        /// <summary>
        /// Indicator to determine if doors have been created in the controller
        /// </summary>
        /// <returns>true if doors have been created</returns>
        bool DoorsExist();

    }
}