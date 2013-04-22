using System;
using MarkNic.JsonSupport.Interfaces;

namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public interface IStatus : IJsonSerialize
    {
        /// <summary>
        /// Current state of the component
        /// </summary>
        string State { get; }
        
        /// <summary>
        /// Amount of time the component has been in the current state
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Update the component with the current or new state
        /// </summary>
        /// <param name="newState">"new" (current) state of the component</param>
        /// <param name="currentTickCount"># of ticks since the last update</param>
        /// <returns></returns>
        TimeSpan UpdateState(string newState, long currentTickCount);
    }
}
