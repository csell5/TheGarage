using System;
using Marknic.NdGarageDoorLightsController;

namespace Marknic.TestRunner
{
    public class OutPortStub : IOutPort
    {
        private int _counter;

        public void Write(bool state)
        {
            switch (_counter)
            {
                case 0:
                    if (!state) throw new ApplicationException("Invalid Step - state should have been true.");
                    _counter++;
                    break;
                    
                case 1:
                    if (state) throw new ApplicationException("Invalid Step - state should have been false.");
                    _counter++;
                    break;
            }
        }
    }
}
