using System;

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

        public bool IsLockChanged(Garage otherGarage)
        {
            if (Name != otherGarage.Name) return true;

            if (Locked != otherGarage.Locked) return true;

            if (HardwareLock != otherGarage.HardwareLock) return true;

            return SoftLock != otherGarage.SoftLock;
        }

        public bool IsDoorChanged(Garage otherGarage)
        {
            for (var i = 0; i < Door.Length; i++)
            {
                if (Door[0].Name != otherGarage.Door[i].Name) return true;
                if (Door[0].Status != otherGarage.Door[i].Status) return true;
            }

            return false;
        }

        public bool IsLightChanged(Garage otherGarage)
        {
            for (var i = 0; i < Light.Length; i++)
            {
                if (Light[0].Name != otherGarage.Light[i].Name) return true;
                if (Light[0].Status != otherGarage.Light[i].Status) return true;
            }

            return false;
        }

        public static void Copy(Garage fromGarage, Garage toGarage)
        {
            if (fromGarage == null) throw new ArgumentNullException("fromGarage");
            if (toGarage == null) throw new ArgumentNullException("toGarage");
            if (fromGarage == toGarage) throw new ArgumentException("From and To Garage objects cannot be the same object.");

            toGarage.Name = fromGarage.Name;
            toGarage.Locked = fromGarage.Locked;
            toGarage.HardwareLock = fromGarage.HardwareLock;
            toGarage.SoftLock = fromGarage.SoftLock;

            toGarage.Light = new GarageLight[fromGarage.Light.Length];
            for (var i = 0; i < fromGarage.Light.Length; i++)
            {
                toGarage.Light[i] = fromGarage.Light[i];
            }

            toGarage.Door = new GarageDoor[fromGarage.Door.Length];
            for (var i = 0; i < fromGarage.Door.Length; i++)
            {
                toGarage.Door[i] = fromGarage.Door[i];
            }
        }
    }
}
