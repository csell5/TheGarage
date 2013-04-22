namespace Marknic.Web.Interfaces
{
    public interface ICommandRequest
    {
        /// <summary>
        /// Name of the device component to control
        /// </summary>
        string Component { get; }

        /// <summary>
        /// ID of the component if there is an array (doors, lights, etc.)
        /// </summary>
        int ComponentNumber { get; }

        /// <summary>
        /// Command to be executed
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Extra command information if required
        /// </summary>
        string CommandParms { get; }
    }
}