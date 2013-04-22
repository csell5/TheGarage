namespace GarageTestConsole.Devices
{
    public class Garage
    {
        public string Name;
        public bool Locked;
        public bool HardwareLock;
        public bool SoftLock;
        public long Duration;

        public GarageDoor[] Door;
        public GarageLight[] Light;

        public bool Changed(Garage otherGarage)
        {
            if (Name != otherGarage.Name) return true;

            if (Locked != otherGarage.Locked) return true;

            if (HardwareLock != otherGarage.HardwareLock) return true;

            if (SoftLock != otherGarage.SoftLock) return true;

            if (Door.Length != otherGarage.Door.Length) return true;

            if (Light.Length != otherGarage.Light.Length) return true;

            for (var i = 0; i < Door.Length; i++)
            {
                if (Door[0].Name != otherGarage.Door[i].Name) return true;
                if (Door[0].Status != otherGarage.Door[i].Status) return true;
            }
            for (var i = 0; i < Light.Length; i++)
            {
                if (Light[0].Name != otherGarage.Light[i].Name) return true;
                if (Light[0].Status != otherGarage.Light[i].Status) return true;
            }

            return false;
        }
    }
}
