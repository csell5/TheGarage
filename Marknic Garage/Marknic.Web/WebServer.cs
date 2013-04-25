using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Marknic.Web.Interfaces;
using Marknic.Web.RequestResponse;
using Marknic.Web.Utility;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;

namespace Marknic.Web
{
    /// <summary>
    /// Multithreaded webserver for .NET Micro Framework.
    /// 
    /// Usage:
    /// - Instantiate a new instance of WebServer:
    ///   WebServer server = new WebServer(80);
    ///   
    /// - Add a handler to the CommandReceived event:
    ///   server.CommandReceived += server_CommandReceived;
    ///   
    /// - Start the server:
    ///   server.Start();
    ///   
    /// </summary>
    public class WebServer : IWebServer
    {
        private bool _cancel;
        private readonly Thread _serverThread;

        #region Constructors

        /// <summary>
        /// Instantiates a new webserver.
        /// </summary>
        /// <param name="port">Port number to listen on.</param>
        public WebServer(int port)
        {
            Port = port;

            _serverThread = new Thread(StartServer);

            Debug.Print("Web Server started on port: " + port);
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate for the CommandReceived event.
        /// </summary>
        public delegate void CommandReceivedHandler(object source, WebCommandEventArgs e);

        /// <summary>
        /// CommandReceived event is triggered when a valid command (plus parameters) is received.
        /// Valid commands are defined in the AllowedCommands property.
        /// </summary>
        public event CommandReceivedHandler CommandReceived;

        #endregion

        #region Public and private methods

        /// <summary>
        /// Start the multithreaded server.
        /// </summary>
        public void Start()
        {
            // List ethernet interfaces, so we can determine the server's address
            ListNetworkInterfaces();

            // start server
            _cancel = false;

            _serverThread.Start();

            Debug.Print("Started server in thread " + _serverThread.GetHashCode());
        }

        public void Stop()
        {
            _cancel = true;
        }


        /// <summary>
        /// Starts the web server .
        /// </summary>
        private void StartServer()
        {
            using (var webServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                webServer.Bind(new IPEndPoint(IPAddress.Any, Port));
                webServer.Listen(1);

                while (!_cancel)
                {
                    using (var connectionSocket = webServer.Accept())
                    {
                        if (!connectionSocket.Poll(-1, SelectMode.SelectRead)) continue;

                        // Create buffer and receive raw bytes.
                        var bytes = new byte[connectionSocket.Available];

                        var bytesRead = connectionSocket.Receive(bytes);

                        Thread.Sleep(20);

                        var bytesAvailable = connectionSocket.Available;

                        while (bytesAvailable > 0)
                        {
                            if (bytes.Length < (bytesRead + bytesAvailable))
                            {
                                var newBytes = new byte[bytesRead + bytesAvailable];
                                bytes.CopyTo(newBytes, 0);
                                bytes = newBytes;
                            }

                            bytesRead += connectionSocket.Receive(bytes, bytesRead, connectionSocket.Available, SocketFlags.None);

                            bytesAvailable = connectionSocket.Available;
                        }

                        if (bytesRead <= 0) continue;

                        // Convert to string, will include HTTP headers.
                        var rawData = new string(Encoding.UTF8.GetChars(bytes));
                        var command = new RequestData(rawData);  // InterpretRequest(rawData);
                        if ((command.HttpVerb == "POST") && (command.Body == null))
                        {
                            var x = 0;
                        }
                        if (command.ArgumentCount > 0)
                        {
                            var fileSearchA = command.Query.Replace('/', '\\').ToLower();
                            var fileSearchB = FileUtility.FileStore + fileSearchA;

                            // GET and resource is a file and it was found
                            if ((command.HttpVerb == RequestData.HttpGet) && File.Exists(fileSearchA))
                            {
                                SendFile(fileSearchA, connectionSocket);
                            }
                            else if ((command.HttpVerb == RequestData.HttpGet) && File.Exists(fileSearchB))
                            {
                                SendFile(fileSearchB, connectionSocket);
                            }
                            // Command received event has been created
                            else if (CommandReceived != null)
                            {
                                var args = new WebCommandEventArgs(command);

                                CommandReceived(this, args);

                                var returnDocument = HtmlSupport.FormatResponse(args.ReturnString);
                                var returnBytes = Encoding.UTF8.GetBytes(returnDocument);
                                try
                                {
                                    connectionSocket.Send(returnBytes, 0, returnBytes.Length, SocketFlags.None);
                                }
                                catch (Exception ex)
                                {
                                    Debug.Print("Socket error responding to command: " + ex.Message);
                                }
                            }
                            else
                            {
                                using (var sourceFile = new FileStream(fileSearchB, FileMode.Open))
                                {
                                    var output = new byte[sourceFile.Length];
                                    var returnBytesRead = sourceFile.Read(output, 0, (int)sourceFile.Length);

                                    try
                                    {
                                        connectionSocket.Send(output, 0, returnBytesRead, SocketFlags.None);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.Print("Socket error sending file: " + ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var failed = true;
                            var indexFile = FileUtility.FileStore + "index.htm";

                            try
                            {
                                failed = !SendFile(indexFile, connectionSocket);
                            }
                            catch (Exception ex)
                            {
                                Debug.Print("SendFile failed: " + ex.Message);
                            }

                            if (failed)
                            {
                                // Show default page
                                var returnBytes = Encoding.UTF8.GetBytes(HtmlSupport.FormatResponse(HtmlSupport.ShowErrorPage, "htm"));
                                connectionSocket.Send(returnBytes, 0, returnBytes.Length, SocketFlags.None);
                            }
                        }
                    }
                }
            }
        }

        private static bool SendFile(string filePath, Socket connection)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (!File.Exists(filePath))
            {
                return false;
            }


            using (var sourceFile = new FileStream(filePath, FileMode.Open))
            {
                var extension = HtmlSupport.GetResourceExtension(filePath);

                if (extension != string.Empty)
                {
                    var contentType = HtmlSupport.GetContentType(extension);

                    var outString = "HTTP/1.1 200 OK" + HtmlSupport.CrLf
                                    + "Connection: Keep-Alive" + HtmlSupport.CrLf
                                    + "Content-Type: " + contentType + HtmlSupport.CrLf
                                    + "Content-Length: " + sourceFile.Length + HtmlSupport.CrLf
                                    + HtmlSupport.CrLf;

                    var output = Encoding.UTF8.GetBytes(outString);

                    try
                    {
                        connection.Send(output);

                        output = new byte[5000];
                        int bytesRead;

                        while ((bytesRead = sourceFile.Read(output, 0, 5000)) > 0)
                        {
                            connection.Send(output, 0, bytesRead, SocketFlags.None);
                        }
                    }
                    catch (SocketException e)
                    {
                        Debug.Print("SOCKET EXCEPTION CAUGHT: " + e.ErrorCode + " - " + e.Message);
                    }
                }
            }

            return true;
        }

        private static void ListNetworkInterfaces()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            Debug.Print("Network Interface Count: " + networkInterfaces.Length);

            Debug.Print("IP Addresses:");

            foreach (var netInterface in networkInterfaces)
            {
                Debug.Print(netInterface.IPAddress + " - " + netInterface.SubnetMask);
            }
        }



        #endregion

        #region Properties

        /// <summary>
        /// The server port
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// List of commands that can be handled by the server.
        /// Because of the limited support for generics on the .NET micro framework,
        /// this property is implemented as an ArrayList. Make sure you only add
        /// objects of type WebCommand to this list.
        /// </summary>
        public readonly System.Collections.ArrayList AllowedCommands = new System.Collections.ArrayList();

        #endregion
    }
}
