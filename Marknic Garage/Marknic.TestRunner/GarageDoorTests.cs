using Marknic.NdGarageDoorLightsController;
using Marknic.TestRunner.Rigging;
using Marknic.TestRunner.Stubs;
using Microsoft.SPOT.Hardware;

namespace Marknic.TestRunner
{
    public class GarageDoorTests
    {
        public void Test_GarageDoorTests_WhenActivateDoorSwitchCalledShouldTurnDoorSwitchOnThenOff()
        {
            PortManager.Reset();

            PortManager.AddInPort("TopSensor0", new TestInPort());
            PortManager.AddInPort("BottomSensor0", new TestInPort());
            PortManager.AddOutPort("DoorSwitch0", new OutPortStub());

            var garageDoor = new GarageDoor(0, "Garage", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1);

            garageDoor.ActivateDoorSwitch();
        }

        public void Test_GarageDoorTests_WhenSwitchDoorCalledWithNullTopSensorShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("TopSensor0", new TestInPort());
            PortManager.AddInPort("BottomSensor0", new TestInPort());
            PortManager.AddOutPort("DoorSwitch0", new OutPortStub());

            var garageDoor = new GarageDoor(0, "Garage", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1);

            // Remove the ports and add the bottom sensor but not the top
            PortManager.Reset();
            PortManager.AddInPort("BottomSensor0", new TestInPort());

            var result = garageDoor.SwitchDoor(true);

            Assert.IsFalse(result);
        }

        public void Test_GarageDoorTests_WhenSwitchDoorCalledWithNullBottomSensorShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("TopSensor0", new TestInPort());
            PortManager.AddInPort("BottomSensor0", new TestInPort());
            PortManager.AddOutPort("DoorSwitch0", new OutPortStub());

            var garageDoor = new GarageDoor(0, "Garage", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1);

            // Remove the ports and add the top sensor
            PortManager.Reset();
            PortManager.AddInPort("TopSensor0", new TestInPort());

            var result = garageDoor.SwitchDoor(true);

            Assert.IsFalse(result);
        }

        public void Test_GarageDoorTests_WhenSwitchDoorCalledWithNoSensorsShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("TopSensor0", new TestInPort());
            PortManager.AddInPort("BottomSensor0", new TestInPort());
            PortManager.AddOutPort("DoorSwitch0", new OutPortStub());

            var garageDoor = new GarageDoor(0, "Garage", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1);

            // Remove the ports
            PortManager.Reset();

            var result = garageDoor.SwitchDoor(true);

            Assert.IsFalse(result);
        }

    }
}
