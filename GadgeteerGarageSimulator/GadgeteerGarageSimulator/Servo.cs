using Gadgeteer;
using Gadgeteer.Interfaces;

namespace GadgeteerGarageSimulator
{
    public class Servo
    {
        /// <summary>                
        /// PWM handle                
        /// </summary>                
        private readonly PWMOutput _servo;

        /// <summary>                
        /// Timings range                
        /// </summary>                
        private readonly int[] _range = new int[2];

        /// <summary>               
        /// Set servo inversion                 
        /// </summary>                
        public bool Inverted;

        /// <summary>                
        /// Create the PWM pin, set it low and configure timings                
        /// </summary>
        /// <param name="servoSocket"></param>
        /// <param name="pin"></param>                
        public Servo(Socket servoSocket, Socket.Pin pin)
        {
            // Init the PWM pin                        
            _servo = new PWMOutput(servoSocket, pin, false, null)
            {
                Active = false
            };

            // Typical settings                        
            _range[0] = 1605000;  // Was 1415000
            _range[1] = 2250000;  // Was 2250000
        }

        /// <summary>                
        /// Allow the user to set cutom timings                
        /// </summary>                
        /// <param name="fullLeft"></param>                
        /// <param name="fullRight"></param>                
        public void SetRange(int fullLeft, int fullRight)
        {
            _range[1] = fullLeft;
            _range[0] = fullRight;
        }

        /// <summary>                
        /// Disengage the servo.                 
        /// The servo motor will stop trying to maintain an angle                
        /// </summary>                
        public void Disengage()
        {
            // See what the Netduino team say about this...                         
            _servo.Active = false;
        }

        /// <summary>                
        /// Set the servo degree                
        /// </summary>                
        public double Degree
        {
            set
            {
                // Range checks                                
                if (value > 90)
                    value = 90;

                if (value < 0)
                    value = 0;

                // Are we inverted?                                
                if (Inverted)
                    value = 90 - value;

                // Set the pulse                                
                _servo.SetPulse(20000000, (uint)Map((long)value, 0, 90, _range[0], _range[1]));
            }
        }

        /// <summary>                
        /// Used internally to map a value of one scale to another                
        /// </summary>                
        /// <param name="x"></param>                
        /// <param name="inMin"></param>                
        /// <param name="inMax"></param>                
        /// <param name="outMin"></param>                
        /// <param name="outMax"></param>                
        /// <returns></returns>                
        private static long Map(long x, long inMin, long inMax, long outMin, long outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}




