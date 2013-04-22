using Marknic.NdGarageDoorLightsController;

namespace Marknic.TestRunner.Stubs
{
    public class TestInPort : IInPort
    {
        public bool Read()
        {
            return true;
        }
    }
}