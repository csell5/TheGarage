using Microsoft.SPOT;
using GT = Gadgeteer;

namespace GadgeteerGarageSimulator
{
    public partial class Program
    {
        private static Garage _garage;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            _garage = new Garage(irReceiver);

            _garage.Start();

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");
        }
    }
}
