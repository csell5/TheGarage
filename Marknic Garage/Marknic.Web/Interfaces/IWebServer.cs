namespace Marknic.Web.Interfaces
{
    public interface IWebServer
    {
        /// <summary>
        /// CommandReceived event is triggered when a valid command (plus parameters) is received.
        /// Valid commands are defined in the AllowedCommands property.
        /// </summary>
        event WebServer.CommandReceivedHandler CommandReceived;

        /// <summary>
        /// The server port
        /// </summary>
        int Port { get; }

        /// <summary>
        /// Start the multithreaded web server.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the web server
        /// </summary>
        void Stop();
    }
}