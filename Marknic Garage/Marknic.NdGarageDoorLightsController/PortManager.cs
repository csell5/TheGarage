using System;
using System.Collections;
using Marknic.NdGarageDoorLightsController.Utility;
using Microsoft.SPOT.Hardware;

namespace Marknic.NdGarageDoorLightsController
{
    public static class PortManager
    {
        private static IDictionary _dictionary;

        static PortManager()
        {
            _dictionary = new Hashtable();
        }

        public static void Reset()
        {
            _dictionary = new Hashtable();
        }

        public static object GetPort(string portName)
        {
            if (portName.IsNullOrEmpty()) throw new ArgumentNullException("portName");

            return _dictionary.Contains(portName) ? _dictionary[portName] : null;
        }

        public static IInPort GetInPort(string portName)
        {
            if (portName.IsNullOrEmpty()) throw new ArgumentNullException("portName");

            return (IInPort) (_dictionary.Contains(portName) ? _dictionary[portName] : null);
        }

        public static IOutPort GetOutPort(string portName)
        {
            if (portName.IsNullOrEmpty()) throw new ArgumentNullException("portName");

            return (IOutPort)(_dictionary.Contains(portName) ? _dictionary[portName] : null);
        }

        public static void AddInPort(string portName, IInPort port)
        {
            if (portName.IsNullOrEmpty()) throw new ArgumentNullException("portName");

            if (port == null) throw new ArgumentNullException("port");

            _dictionary.Add(portName, port);
        }

        public static void AddOutPort(string portName, IOutPort port)
        {
            if (portName.IsNullOrEmpty()) throw new ArgumentNullException("portName");

            if (port == null) throw new ArgumentNullException("port");

            _dictionary.Add(portName, port);
        }

        public static IInPort CreateInputPort(string portName, Cpu.Pin pin, bool glitchFilter, Port.ResistorMode resistorMode)
        {
            var port = new InPort(pin, glitchFilter, resistorMode);
            
            _dictionary.Add(portName, port);

            return port;
        }

        public static IOutPort CreateOutputPort(string portName, Cpu.Pin pin, bool initialState)
        {
            var port = new OutPort(pin, initialState);

            _dictionary.Add(portName, port);

            return port;
        }
    }

    public interface IInPort
    {
        bool Read();
    }

    public class InPort : IInPort
    {
        private readonly InputPort _port;

        public InPort(Cpu.Pin pin, bool glitchFilter, Port.ResistorMode resistorMode)
        {
            _port = new InputPort(pin, glitchFilter, resistorMode);
        }

        public bool Read()
        {
            var portValue = _port.Read();

            return portValue;
        }
    }

    public interface IOutPort
    {
        void Write(bool state);
    }

    public class OutPort : IOutPort
    {
        private readonly OutputPort _port;

        public OutPort(Cpu.Pin pin, bool initialState)
        {
            _port = new OutputPort(pin, initialState);
        }

        public void Write(bool state)
        {
            _port.Write(state);
        }
    }
}
