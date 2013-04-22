using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using Marknic.NdGarageDoorLightsController.Interfaces;
using Marknic.NdGarageDoorLightsController.Utility;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Marknic.NdGarageDoorLightsController
{
    /// <summary>
    /// Class to manage garage doors (1-2) and light circuits (0-2)
    /// </summary>
    public class GarageController : IGarageController
    {
        private const string GarageSetting = "garageSetting";
        private const string DoorSetting = "doorSetting";
        private const string LightSetting = "lightSetting";
        private const string Add = "add";
        private const string Key = "key";
        private const string Value = "value";
        private const string InitialSetting = "initialSetting";
        private const string NameKey = "name";
        private const string LockedKey = "locked";
        private const string LockoutSensor = "LockoutSensor";

        private IGarageLight[] _garageLights;
        private IGarageDoor[] _garageDoors;

        public bool SoftLock { get; set; }

        public string Name { get; private set; }

        public IStatus Status { get; private set; }

        public static GarageController Garage { get; private set; }

        /// <summary>
        /// Get the number of doors
        /// </summary>
        public int DoorCount
        {
            get { return _garageDoors == null ? 0 : _garageDoors.Length; }
        }

        /// <summary>
        /// Get the number of light
        /// </summary>
        public int LightCount
        {
            get { return _garageLights == null ? 0 : _garageLights.Length; }
        }

        
        /// <summary>
        /// Factory method for creating a garage object
        /// </summary>
        /// <param name="name">Name of the garage to create</param>
        /// <param name="softLock">Indicates if the garage should be locked when it starts</param>
        /// <returns></returns>
        public static IGarageController CreateGarage(string name, bool softLock)
        {
            var garageController = new GarageController(name, softLock);

            Garage = garageController;

            return garageController;
        }

        /// <summary>
        /// ctor: create a garage controller
        /// </summary>
        /// <param name="name">name to give the garage</param>
        /// <param name="softlock">lock mechanism through software</param>
        public GarageController(string name, bool softlock)
        {
            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");
            }

            var lockoutSensor = PortManager.GetInPort(LockoutSensor);

            if (lockoutSensor == null)
            {
                PortManager.CreateInputPort(LockoutSensor, Pins.GPIO_PIN_D0, false, Port.ResistorMode.PullUp);
            }

            _garageDoors = null;

            _garageLights = null;

            SoftLock = softlock;

            Name = name;

            Status = new Status(StatusValues.GarageUnlocked, DateTime.Now.Ticks);
        }

        /// <summary>
        /// Add a garage door to the controller
        /// </summary>
        /// <param name="name">Name to give the door</param>
        /// <param name="topSensorDoor">pin for the upper (open) sensor of the Door</param>
        /// <param name="bottomSensorDoor">pin for the lower (closed) sensor of the Door</param>
        /// <param name="switchDoor">pin for the garage door switch to toggle the Door</param>
        /// <returns></returns>
        public int AddDoor(string name, Cpu.Pin topSensorDoor, Cpu.Pin bottomSensorDoor, Cpu.Pin switchDoor)
        {
            IGarageDoor[] tmpGarageDoors;
            var length = 0;

            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");
            }

            if (topSensorDoor == Cpu.Pin.GPIO_NONE || bottomSensorDoor == Cpu.Pin.GPIO_NONE ||
                switchDoor == Cpu.Pin.GPIO_NONE)
            {
                throw new ArgumentException("All pins must have valid settings - at least one was set to Cpu.Pin.GPIO_NONE");
            }

            if (_garageDoors == null)
            {
                tmpGarageDoors = new IGarageDoor[1];
            }
            else
            {
                length = _garageDoors.Length;

                tmpGarageDoors = new IGarageDoor[length + 1];

                _garageDoors.CopyTo(tmpGarageDoors, 0);
            }

            tmpGarageDoors[length] = new GarageDoor(length, name, topSensorDoor, bottomSensorDoor, switchDoor);

            return length;
        }

        /// <summary>
        /// Add a garage door to the controller
        /// </summary>
        /// <param name="name">Name to give the door</param>
        /// <returns></returns>
        public IGarageDoor AddDoor(string name)
        {
            GarageDoor door;
            IGarageDoor[] tmpGarageDoors;
            var length = 0;

            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");
            }

            if (_garageDoors == null)
            {
                tmpGarageDoors = new IGarageDoor[1];
            }
            else
            {
                length = _garageDoors.Length;

                tmpGarageDoors = new IGarageDoor[length + 1];

                _garageDoors.CopyTo(tmpGarageDoors, 0);
            }


            switch (length)
            {
                case 0:
                    door = new GarageDoor(length, name, Pins.GPIO_PIN_D11, Pins.GPIO_PIN_D10, Pins.GPIO_PIN_D13);
                    break;

                case 1:
                    door = new GarageDoor(length, name, Pins.GPIO_PIN_D9, Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D12);
                    break;

                default:
                    throw new ArgumentException("Error attempting to create a garage door - too many doors for this controller");
            }

            _garageDoors = tmpGarageDoors;

            _garageDoors[length] = door;

            return door;
        }

        /// <summary>
        /// Add a light switch circuit to the controller
        /// </summary>
        /// <param name="name">name to give the light circuit</param>
        /// <param name="initialValue">initial value of the light true == on</param>
        /// <returns></returns>
        public IGarageLight AddLight(string name, bool initialValue)
        {
            GarageLight light;
            IGarageLight[] tmpGarageLights;
            var length = 0;

            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");
            }

            if (_garageLights == null)
            {
                tmpGarageLights = new IGarageLight[1];
            }
            else
            {
                length = _garageLights.Length;

                tmpGarageLights = new IGarageLight[length + 1];

                _garageLights.CopyTo(tmpGarageLights, 0);
            }

            switch (length)
            {
                case 0:
                    light = new GarageLight(length, name, Pins.GPIO_PIN_D7, Pins.GPIO_PIN_D3, initialValue);
                    break;

                case 1:
                    light = new GarageLight(length, name, Pins.GPIO_PIN_D6, Pins.GPIO_PIN_D2, initialValue);
                    break;

                default:
                    throw new ArgumentException("Error attempting to create a garage light - too many lights for this controller");
            }

            _garageLights = tmpGarageLights;

            _garageLights[length] = light;

            return light;
        }


        /// <summary>
        /// Retrieves the GarageDoor object based on the number of the door
        /// </summary>
        /// <param name="doorNumber">door number to retrieve</param>
        /// <returns>garage door object</returns>
        public IGarageDoor GetDoor(int doorNumber)
        {
            if (doorNumber < 0) throw new ArgumentException("doorNumber must be > 0");

            if (_garageDoors == null) return null;

            if (doorNumber > _garageDoors.Length - 1)
            {
                throw new ArgumentException("doorNumber is greater than the number of doors");
            }

            var door = _garageDoors[doorNumber];

            return door;
        }

        public IGarageDoor GetDoor(string doorName)
        {
            if (doorName.IsNullOrEmpty())
            {
                throw new ArgumentNullException("doorName");
            }

            if (_garageDoors == null) return null;

            foreach (var garageDoor in _garageDoors)
            {
                if (garageDoor.Name.ToLower() == doorName.ToLower())
                {
                    return garageDoor;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the Light object based on the number of the light
        /// </summary>
        /// <param name="lightNumber"></param>
        /// <returns></returns>
        public IGarageLight GetLight(int lightNumber)
        {
            if (lightNumber < 0) throw new ArgumentException("lightNumber must be > 0");

            if (_garageLights == null) return null;

            if (lightNumber > _garageLights.Length - 1)
            {
                throw new ArgumentException("lightNumber is greater than the number of doors");
            }

            var light = _garageLights[lightNumber];

            return light;
        }

        /// <summary>
        /// Retrieves the Light object based on the name of the light
        /// </summary>
        /// <param name="lightName">name of the light to retrieve</param>
        /// <returns>light object if found - null if not</returns>
        public IGarageLight GetLight(string lightName)
        {
            if (lightName.IsNullOrEmpty())
            {
                throw new ArgumentNullException("lightName");
            }

            if (_garageLights == null) return null;

            foreach (var garageLight in _garageLights)
            {
                if (garageLight.Name.ToLower() == lightName.ToLower())
                {
                    return garageLight;
                }
            }

            return null;
        }


        /// <summary>
        /// turns on a light switch relay based on the number of the light
        /// </summary>
        /// <param name="lightNumber">light number to activate</param>
        public void ActivateLightSwitch(int lightNumber)
        {
            if (lightNumber < 0) throw new ArgumentException("lightNumber must be > 0");

            if (_garageLights == null) return;

            var light = _garageLights[lightNumber];

            if (light == null)
                throw new ArgumentException("Invalid light number: " + lightNumber);

            light.CircuitOn();
        }

        /// <summary>
        /// turns off a light switch relay based on the number of the light
        /// </summary>
        /// <param name="lightNumber">light number to deactivate</param>
        public void DeactivateLightSwitch(int lightNumber)
        {
            if (lightNumber < 0) throw new ArgumentException("lightNumber must be > 0");

            if (_garageLights == null) return;

            if (lightNumber > _garageLights.Length - 1)
            {
                throw new ArgumentException("lightNumber is greater than the number of doors");
            }

            var light = _garageLights[lightNumber];

            if (light == null)
                throw new ArgumentException("Invalid light number: " + lightNumber);

            light.CircuitOff();
        }

        /// <summary>
        /// activates a garage switch based on the number of the door
        /// </summary>
        /// <param name="doorNumber">door number to activate</param>        
        public void ActivateDoorSwitch(int doorNumber)
        {
            if (doorNumber < 0) throw new ArgumentException("doorNumber must be > 0");

            if (_garageDoors == null) return;

            if (doorNumber > _garageDoors.Length - 1)
            {
                throw new ArgumentException("doorNumber is greater than the number of doors");
            }

            var door = _garageDoors[doorNumber];

            if (door == null)
                throw new ArgumentException("Invalid door number: " + doorNumber);

            new Thread(door.ActivateDoorSwitch).Start();
        }

        public bool HardwareLockIsActivated
        {
            get
            {
                return ((PortManager.GetInPort(LockoutSensor)).Read());
            }
        }


        /// <summary>
        /// Read-only property indicating if the lockout switch has been activated
        /// </summary>
        public bool LockoutIsActivated
        {
            get
            {
                var lockoutValue = false;

                var lockoutSensor = PortManager.GetInPort(LockoutSensor);
                if (lockoutSensor != null)
                {
                    lockoutValue = lockoutSensor.Read();
                }

                return (lockoutValue || SoftLock);
            }
        }

        /// <summary>
        /// Indicates if the upper (top) sensor is activated meaning the garage door is in the up position
        /// </summary>
        /// <param name="doorNumber">door number to check</param>
        /// <returns>true if the sensor is activated</returns>
        public bool TopSensorIsActivated(int doorNumber)
        {
            if (doorNumber < 0) throw new ArgumentException("doorNumber must be > 0");

            if (_garageDoors == null) return false;

            if (doorNumber > _garageDoors.Length - 1)
            {
                throw new ArgumentException("doorNumber is greater than the number of doors");
            }

            var door = _garageDoors[doorNumber];

            if (door == null)
                throw new ArgumentException("Invalid door number: " + doorNumber);

            return !door.TopSensorRead();
        }

        /// <summary>
        /// Indicates if the lower (bottom) sensor is activated meaning the garage door is in the down position
        /// </summary>
        /// <param name="doorNumber">door number to check</param>
        /// <returns>true if the sensor is activated</returns>
        public bool BottomSensorIsActivated(int doorNumber)
        {
            if (doorNumber < 0) throw new ArgumentException("doorNumber must be > 0");

            if (_garageDoors == null) return false;

            if (doorNumber > _garageDoors.Length - 1)
            {
                throw new ArgumentException("doorNumber is greater than the number of doors");
            }

            var door = _garageDoors[doorNumber];

            if (door == null)
                throw new ArgumentException("Invalid door number: " + doorNumber);

            return !door.BottomSensorRead();
        }

        /// <summary>
        /// Indicator to determine if lights have been created in the controller
        /// </summary>
        /// <returns>true if lights have been created</returns>
        public bool LightsExist()
        {
            return ((_garageLights != null) && _garageLights.Length > 0);
        }

        /// <summary>
        /// Indicator to determine if doors have been created in the controller
        /// </summary>
        /// <returns>true if doors have been created</returns>
        public bool DoorsExist()
        {
            return ((_garageDoors != null) && _garageDoors.Length > 0);
        }

        public IStatus UpdateStatus()
        {
            var newStatus = StatusValues.GarageUnlocked;

            if (LockoutIsActivated || SoftLock)
            {
                newStatus = StatusValues.GarageLocked;
            }

            Status.UpdateState(newStatus, DateTime.Now.Ticks);

            foreach (var garageLight in _garageLights)
            {
                garageLight.UpdateStatus();
            }

            foreach (var garageDoor in _garageDoors)
            {
                garageDoor.UpdateStatus();
            }

            return Status;
        }

        public static GarageController ReadXmlLocal(string filename)
        {
            Garage = ReadXml(filename);

            return Garage;
        }

        /// <summary>
        /// Reads an XML configuration file and creates a garage controller object.  The object is then stored in the static instance location of the class.
        /// </summary>
        /// <param name="filename">Full path to the configuration file.</param>
        /// <returns>Returns the generated garage controller object</returns>
        public static GarageController ReadXml(string filename)
        {
            var garageName = string.Empty;
            var doors = new string[2];
            var lights = new DictionaryEntry[2];
            var softLock = false;

            if (filename.IsNullOrEmpty())
            {
                throw new ArgumentNullException("filename");
            }

            using (var configFile = new FileStream(filename, FileMode.Open))
            {
                var reader = XmlReader.Create(configFile);

                while (reader.ReadElement())
                {
                    switch (reader.Name)
                    {
                        case GarageSetting:

                            while (reader.ReadElement())
                            {
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }

                                if (reader.Name != Add) continue;

                                var key = reader.GetAttribute(Key);
                                var value = reader.GetAttribute(Value);

                                switch (key.ToLower())
                                {
                                    case NameKey:
                                        garageName = value;
                                        break;

                                    case LockedKey:
                                        softLock = value.ToLower() == "true";
                                        break;
                                }
                            }
                            break;

                        case DoorSetting:

                            while (reader.ReadElement())
                            {
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }

                                if (reader.Name != Add) continue;

                                var value = reader.GetAttribute(Value);

                                for (var i = 0; i < doors.Length; i++)
                                {
                                    if ((doors[i] != null) && (doors[i] != string.Empty)) continue;

                                    doors[i] = value;
                                    break;
                                }
                            }
                            break;

                        case LightSetting:

                            while (reader.ReadElement())
                            {
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    break;
                                }

                                if (reader.Name != Add) continue;

                                var value = reader.GetAttribute(Value);
                                var initialSetting = reader.GetAttribute(InitialSetting);

                                var light = new DictionaryEntry(value, initialSetting);

                                for (var i = 0; i < lights.Length; i++)
                                {
                                    if (lights[i] != null) continue;

                                    lights[i] = light;
                                    break;
                                }
                            }
                            break;
                    }
                }
            }

            var garageController = new GarageController(garageName, softLock);

            for (var i = 0; i < doors.Length; i++)
            {
                if ((doors[i] != null) && (doors[i] != string.Empty))
                {
                    garageController.AddDoor(doors[i]);
                }
            }

            for (var i = 0; i < lights.Length; i++)
            {
                if (lights[i] != null)
                {
                    garageController.AddLight((string)lights[i].Key, ((string)lights[i].Value).ToLower() == StatusValues.LightOn);
                }
            }

            return garageController;
        }


        public void WriteXml(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
                writer.WriteLine("<configuration>");
                writer.WriteLine("  <garageSetting>");
                writer.WriteLine("    <add key=\"name\" value=\"" + Name + "\" />");
                writer.WriteLine("    <add key=\"locked\" value=\"" + LockoutIsActivated.ToString().ToLower() + "\"/>");
                writer.WriteLine("  </garageSetting>");

                writer.WriteLine("  <doorSetting>");
                for (var i = 0; i < _garageDoors.Length; i++)
                {
                    writer.WriteLine("    <add key=\"" + i + "\" value=\"" + _garageDoors[i].Name + "\"/>");
                }
                writer.WriteLine("  </doorSetting>");

                writer.WriteLine("  <lightSetting>");
                for (var i = 0; i < _garageLights.Length; i++)
                {
                    writer.WriteLine("    <add key=\"" + i + "\" value=\"" + _garageLights[i].Name + "\" initialSetting=\"" + _garageLights[i].Status.State + "\" />");
                }
                writer.WriteLine("  </lightSetting>");

                writer.WriteLine("</configuration>");
            }
        }

        public string SerializeJson()
        {
            if (Garage == null) return string.Empty;

            var jsonResponse = new StringBuilder();

            jsonResponse.Append("{ \"Name\": \"" + Name + "\", \"Locked\": " + LockoutIsActivated.ToString().ToLower() + ", \"HardwareLock\": " + HardwareLockIsActivated.ToString().ToLower() + ", \"SoftLock\": " + SoftLock.ToString().ToLower() + ", ");

            var doorsExist = (_garageDoors != null) && (_garageDoors.Length > 0);

            if (doorsExist)
            {
                jsonResponse.Append(" \"Door\": [ ");

                for (var i = 0; i < _garageDoors.Length; i++)
                {
                    jsonResponse.Append(_garageDoors[i].SerializeJson());

                    jsonResponse.Append(i < _garageDoors.Length - 1 ? ", " : " ] ");
                }
            }

            if ((_garageLights != null) && (_garageLights.Length > 0))
            {
                if (doorsExist)
                {
                    jsonResponse.Append(", ");
                }

                jsonResponse.Append(" \"Light\": [ ");

                for (var i = 0; i < _garageLights.Length; i++)
                {
                    jsonResponse.Append(_garageLights[i].SerializeJson());

                    jsonResponse.Append(i < _garageLights.Length - 1 ? ", " : " ] ");
                }
            }

            jsonResponse.Append(" }");

            var response = jsonResponse.ToString();

            return response;
        }
    }
}