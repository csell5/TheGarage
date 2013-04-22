using Marknic.Web.Interfaces;

namespace Marknic.Web.RequestResponse
{
    /// <summary>
    /// Delegate used to pass through the command to control the device
    /// </summary>
    /// <param name="commandRequest">Command to be processed</param>
    public delegate void DoCommandRequest(ICommandRequest commandRequest);
}
