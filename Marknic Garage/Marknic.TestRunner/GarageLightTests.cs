using Marknic.NdGarageDoorLightsController;
using Marknic.TestRunner.Rigging;
using Marknic.TestRunner.Stubs;
using Microsoft.SPOT.Hardware;

namespace Marknic.TestRunner
{
    public class GarageLightTests
    {

        public void Test_GarageLightTests_WhenSwitchCircuitCalledWithNullSwitchPortShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("SwitchPort0", new TestInPort());
            PortManager.AddOutPort("RelayPort0", new OutPortStub());

            var garagelight = new GarageLight(0, "Outdoors", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, false);

            // Remove the ports and add the bottom sensor but not the top
            PortManager.Reset();
            PortManager.AddOutPort("RelayPort0", new OutPortStub());

            var result = garagelight.SwitchCircuit(true);

            Assert.IsFalse(result);
        }

        public void Test_GarageLightTests_WhenSwitchCircuitCalledWithNullRelayPortShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("SwitchPort0", new TestInPort());
            PortManager.AddOutPort("RelayPort0", new OutPortStub());

            var garagelight = new GarageLight(0, "Outdoors", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, false);

            // Remove the ports and add the bottom sensor but not the top
            PortManager.Reset();
            PortManager.AddInPort("SwitchPort0", new TestInPort());
            
            var result = garagelight.SwitchCircuit(true);

            Assert.IsFalse(result);
        }

        public void Test_GarageLightTests_WhenSwitchCircuitCalledWithNoPortsShouldReturnFalse()
        {
            PortManager.Reset();

            // add the ports so the garage door will create
            PortManager.AddInPort("SwitchPort0", new TestInPort());
            PortManager.AddOutPort("RelayPort0", new OutPortStub());

            var garagelight = new GarageLight(0, "Outdoors", Cpu.Pin.GPIO_Pin1, Cpu.Pin.GPIO_Pin1, false);

            // Remove the ports and add the bottom sensor but not the top
            PortManager.Reset();

            var result = garagelight.SwitchCircuit(true);

            Assert.IsFalse(result);
        }
    }
}
