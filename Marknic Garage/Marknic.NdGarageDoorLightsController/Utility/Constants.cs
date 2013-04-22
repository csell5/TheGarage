namespace Marknic.NdGarageDoorLightsController.Utility
{
    public static class GarageComponents
    {
        public const string SoftLock = "softlock";
        public const string Door = "door";
        public const string Light = "light";
        public const string Garage = "garage";

    } 

    public static class GarageCommands
    {
        public const string DoorCommandOpen = "open";
        public const string DoorCommandClose = "close";
        public const string DoorCommandToggle = "toggle";
        public const string LightCommandOn = "on";
        public const string LightCommandOff = "off";
        public const string Reset = "reset";
        public const string Lock = "lock";
        public const string Unlock = "unlock";
    }

    public static class StatusValues
    {
        public const string Initialized = "initialized";

        public const string GarageLocked = "locked";
        public const string GarageUnlocked = "unlocked";

        public const string DoorUp = "up";
        public const string DoorDown = "down";
        public const string DoorUnknown = "unknown";

        public const string LightOn = "on";
        public const string LightOff = "off";

    }

}
