namespace Marknic.NdGarageDoorLightsController.Interfaces
{
    public interface IComponentStatus
    {
        /// <summary>
        /// Returns current status values of a component
        /// </summary>
        IStatus Status { get; }

        /// <summary>
        /// Initiates an update of the component's status
        /// </summary>
        /// <returns>Current status values of a component</returns>
        IStatus UpdateStatus();
    }
}
