using System.Threading;
using Gadgeteer.Interfaces;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;
using GT = Gadgeteer;

namespace GadgeteerGarageSimulator
{
    public class Garage
    {
        private const int DownPosition = 0;
        private const int UpPosition = 90;
        private const int StepCount = 60;
        private const int StepDelay = 50;

        private readonly GT.Timer _scanTimer;
        private GT.Timer _obstructionTimer;

        private readonly GT.Socket _garageSocket;
        private readonly Servo _servoMotor;
        private readonly DigitalInput _garageSwitch;
        private readonly AnalogInput _obstructionSensor;
        private readonly DigitalOutput _obstructionSignal;
        private readonly IR_Receiver _irReceiver;

        private bool _breakLoop;
        private bool _triggerSwitch;
        private DoorStates _doorStates = DoorStates.Down;
        private int _lastposition;
        
        private bool _buttonDown;

        public Garage(IR_Receiver irReceiver)
        {
            _garageSocket = GT.Socket.GetSocket(3, true, null, null);

            _servoMotor = new Servo(_garageSocket, GT.Socket.Pin.Nine);

            _garageSwitch = new DigitalInput(_garageSocket, GT.Socket.Pin.Three, GlitchFilterMode.Off, ResistorMode.PullUp, null);

            _obstructionSensor = new AnalogInput(_garageSocket, GT.Socket.Pin.Four, null);

            _obstructionSignal = new DigitalOutput(_garageSocket, GT.Socket.Pin.Six, false, null);

            _irReceiver = irReceiver;

            _irReceiver.IREvent += IrEvent;

            // Initialize position
            _lastposition = 45;
            _servoMotor.Degree = _lastposition;

            Thread.Sleep(500);

            _lastposition = MoveServoSlow(_servoMotor, 0, _lastposition, StepCount, StepDelay);

            _scanTimer = new GT.Timer(100);
            _scanTimer.Tick += ScanTimerTick;
        }

        public void Start()
        {
            _scanTimer.Start();
        }

        private void IrEvent(object sender, IR_Receiver.IREventArgs e)
        {
            switch ((ButtonPresses)e.Button)
            {
                case ButtonPresses.OnOff:
                    _breakLoop = true;
                    _triggerSwitch = true;
                    break;

                //case ButtonPresses.Up:
                //    return "Up";

                //case ButtonPresses.Down:
                //    return "Down";

                //case ButtonPresses.Mute:
                //    return "Mute";

                //case ButtonPresses.Left:
                //    return "Left";

                //case ButtonPresses.Right:
                //    return "Right";

                //case ButtonPresses.AvTv:
                //    return "AvTv";

                //default:
                //    return "Invalid Button";
            }
        }

        private void PrintDoorState()
        {
            switch (_doorStates)
            {
                case DoorStates.Down:
                    Debug.Print("DoorStates.Down");
                    break;

                case DoorStates.MovingUp:
                    Debug.Print("DoorStates.MovingUp");
                    break;

                case DoorStates.MovingUpStopped:
                    Debug.Print("DoorStates.MovingUpStopped");
                    _lastposition = MoveServoSlow(_servoMotor, DownPosition, _lastposition, StepCount,
                                                  StepDelay);
                    break;

                case DoorStates.MovingDown:
                    Debug.Print("DoorStates.MovingDown");
                    break;

                case DoorStates.Up:
                    Debug.Print("DoorStates.Up");
                    break;
            }
        }

        private void ScanTimerTick(GT.Timer timer)
        {
            PrintDoorState();

            if (_triggerSwitch)
            {
                _triggerSwitch = false;

                switch (_doorStates)
                {
                    case DoorStates.Down:
                        _doorStates = DoorStates.MovingUp;
                        _lastposition = MoveServoSlow(_servoMotor, UpPosition, _lastposition, StepCount,
                                                      StepDelay);
                        break;

                    case DoorStates.MovingUp:
                        _doorStates = DoorStates.MovingUpStopped;
                        break;

                    case DoorStates.MovingUpStopped:
                        _doorStates = DoorStates.MovingDown;
                        _lastposition = MoveServoSlow(_servoMotor, DownPosition, _lastposition, StepCount,
                                                      StepDelay);
                        break;

                    case DoorStates.MovingDown:
                        _doorStates = DoorStates.MovingUp;
                        _lastposition = MoveServoSlow(_servoMotor, UpPosition, _lastposition, StepCount,
                          StepDelay);
                        break;

                    case DoorStates.Up:
                        _doorStates = DoorStates.MovingDown;
                        _lastposition = MoveServoSlow(_servoMotor, DownPosition, _lastposition, StepCount,
                                                      StepDelay);
                        break;
                }
            }

            ScanAndDelay();
        }


        private int MoveServoSlow(Servo servo, int newPosition, int lastPosition, int steps, int delay)
        {
            // How far are we from the bottom
            var currentOffset = lastPosition - DownPosition;

            // What, then, is the percentage from the bottom
            var percentComplete = (float)currentOffset / (UpPosition - DownPosition);

            // If we are moving up then we need the percentage remaining to the top
            if (_doorStates == DoorStates.MovingUp)
            {
                percentComplete = 1.0f - percentComplete;
            }

            // How many incremental steps is that
            var remainingSteps = (int)(percentComplete * steps);

            // What is the pulse size for the next position
            if (remainingSteps > 0)
            {
                var pulseStep = (newPosition - lastPosition)/remainingSteps;

                // Reset
                _breakLoop = false;

                // Walk through all of the steps to simulate the time it takes to move
                //  a garage door the full distance
                for (var i = _lastposition; i != newPosition; i += pulseStep)
                {
                    if (_doorStates == DoorStates.MovingUpStopped)
                    {
                        newPosition = i;
                        Thread.Sleep(500);
                        break;
                    }

                    var stepPosition = i;

                    // Moving up or down - have we at the end?
                    if (pulseStep > 0)
                    {
                        if (i > newPosition)
                        {
                            stepPosition = newPosition;
                            i = newPosition;
                            _breakLoop = true;
                        }
                    }
                    else
                    {
                        if (i < newPosition)
                        {
                            stepPosition = newPosition;
                            i = newPosition;
                            _breakLoop = true;
                        }
                    }

                    // set the pulse and move the servo
                    servo.Degree = stepPosition;

                    Thread.Sleep(delay);

                    // insert a time delay and also scan the obstacle sensor
                    if (ScanAndDelay())
                    {
                        // If true then something is in the way
                        if (_doorStates == DoorStates.MovingDown)
                        {
                            // stop the door if it is moving down
                            _triggerSwitch = true;
                            _breakLoop = true;
                        }
                    }

                    // time to quit the loop?
                    if (!_breakLoop) continue;

                    Debug.Print("breakloop");
                    return stepPosition;
                }
            }

            // Door completed the move.  Reset the state
            switch (_doorStates)
            {
                case DoorStates.MovingUp:
                    _doorStates = DoorStates.Up;
                    break;
                case DoorStates.MovingDown:
                    _doorStates = DoorStates.Down;
                    break;
            }

            return newPosition;
        }

        private void ScanForGarageSwitch()
        {
            var buttonPressed = !_garageSwitch.Read();

            if (!_buttonDown && buttonPressed)
            {
                Debug.Print("button down: true");
                _buttonDown = true;
                _breakLoop = true;
                _triggerSwitch = true;
            } 
            else if (_buttonDown && !buttonPressed)
            {
                Debug.Print("button down: false");
                _buttonDown = false;
            }
        }

        private bool ScanAndDelay()
        {
            ScanForGarageSwitch();

            if (IsObstructed() && (_obstructionTimer == null))
            {
                if (_obstructionTimer == null)
                {
                    _obstructionTimer = new GT.Timer(500);
                    _obstructionTimer.Tick += timer =>
                        {
                            if (IsObstructed()) return;

                            _obstructionSignal.Write(false);
            
                            _obstructionTimer = null;
                        };
                    _obstructionTimer.Start();

                    _obstructionSignal.Write(true);

                    return true;
                }
            }

            return false;
        }

        private bool IsObstructed()
        {
            const double obstructionTriggerAmount = 2.6;

            var obstructionValue = _obstructionSensor.ReadVoltage();

            if (obstructionValue < obstructionTriggerAmount)
            {
                Debug.Print("------------------------");
                Debug.Print("Obstruction :" + obstructionValue);
                Debug.Print("------------------------");
            }

            return (obstructionValue < obstructionTriggerAmount);
        }
    }
}
