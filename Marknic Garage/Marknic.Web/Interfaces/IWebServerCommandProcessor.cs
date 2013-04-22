using Marknic.Web.RequestResponse;

namespace Marknic.Web.Interfaces
{
    public interface IWebServerCommandProcessor
    {
        /// <summary>
        /// Web server to be created and run to process the Web Server Commands
        /// </summary>
        WebServer WebServer { get; }

        /// <summary>
        /// Event triggered when a command is received
        /// </summary>
        /// <param name="source">Web Server sending the command</param>
        /// <param name="e">Command arguments</param>
        void WebServerCommandReceived(object source, WebCommandEventArgs e);

        /// <summary>
        /// Delegate to execute when a command is received
        /// </summary>
        DoCommandRequest ExecuteCommandRequest { get; set; }
    }
}
