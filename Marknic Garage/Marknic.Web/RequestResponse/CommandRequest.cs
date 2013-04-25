using System;
using Marknic.Web.Interfaces;
using Marknic.Web.Utility;

namespace Marknic.Web.RequestResponse
{
    public class CommandRequest : ICommandRequest 
    {
        private const string InvalidCommandJson = "Invalid Command JSON: ";

        /// <summary>
        /// Name of the device component to control
        /// </summary>
        public string Component { get; private set; }

        /// <summary>
        /// ID of the component if there is an array (doors, lights, etc.)
        /// </summary>
        public int ComponentNumber { get; private set; }

        /// <summary>
        /// Command to be executed
        /// </summary>
        public string Command { get; private set; }

        /// <summary>
        /// Extra command information if required
        /// </summary>
        public string CommandParms { get; private set; }

        /// <summary>
        /// ctor: Parses JSON input into the command request components
        /// </summary>
        /// <param name="jsonInput">JSON string input to be parsed</param>
        public CommandRequest(string jsonInput)
        {
            if (jsonInput == null) return;

            var parmCounter = 0;
            jsonInput = jsonInput.Trim(new[] {'{', '}'});

            var commandParts = jsonInput.Split(',');

            if (commandParts.Length != 3) throw new ArgumentException(InvalidCommandJson + jsonInput);

            foreach (var commandPart in commandParts)
            {
                var commandVals = commandPart.Split(':');

                if (commandVals.Length != 2) throw new ArgumentException(InvalidCommandJson + jsonInput);

                var commandParm = commandVals[0].Trim(new [] {'\"', ' '});

                switch (commandParm)
                {
                    case WebServerConstants.Component:
                        parmCounter++;
                        Component = commandVals[1].Trim(new[] { '\"', ' ' });
                        break;

                    case WebServerConstants.ComponentNumber:
                        parmCounter++;
                        ComponentNumber = int.Parse(commandVals[1].Trim());
                        break;

                    case WebServerConstants.Command:
                        parmCounter++;
                        Command = commandVals[1].Trim(new[] { '\"', ' ' });
                        break;
                }
            }

            if (parmCounter != 3) throw new ArgumentException("Invalid Command JSON");
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="component">Name of the device component to control</param>
        /// <param name="componentNumber">ID of the component if there is an array (doors, lights, etc.)</param>
        /// <param name="command">Command to be executed</param>
        /// <param name="commandParms">Extra command information if required</param>
        public CommandRequest(string component, int componentNumber, string command, string commandParms)
        {
            Component = component;
            ComponentNumber = componentNumber;
            Command = command;
            CommandParms = commandParms;
        }
    }
}
