using System;
using System.IO;
using System.Text;
using Marknic.NdGarageDoorLightsController;
using Marknic.NdGarageDoorLightsController.Utility;
using Marknic.Web;
using Marknic.Web.Interfaces;
using Marknic.Web.RequestResponse;
using Marknic.Web.Utility;
using WebServerConstants = NetduinoPlus2Garage.Support.WebServerConstants;

namespace NetduinoPlus2Garage
{
    public class WebServerCommandProcessor : IWebServerCommandProcessor
    {
        // Name of the configuration file on the local SD card
        public const string ControllerConfigFile = "marknic.controller.config";

        /// <summary>
        /// Delegate to execute when a command is received
        /// </summary>
        public DoCommandRequest ExecuteCommandRequest { get; set; }

        /// <summary>
        /// ctor: sets up the command processor.  The command processor is specific to the device being controlled
        /// </summary>
        /// <param name="webServer"></param>
        public WebServerCommandProcessor(WebServer webServer)
        {
            WebServer = webServer;
            webServer.CommandReceived += WebServerCommandReceived;
        }

        /// <summary>
        /// Web server to be created and run to process the Web Server Commands
        /// </summary>
        public WebServer WebServer { get; private set; }

        /// <summary>
        /// Event triggered when a command is received
        /// </summary>
        /// <param name="source">Web Server sending the command</param>
        /// <param name="e">Command arguments</param>
        public void WebServerCommandReceived(object source, WebCommandEventArgs e)
        {
            switch (e.RequestData.HttpVerb)
            {
                case RequestData.HttpGet:

                    e.ReturnString = DoGet(e.RequestData);

                    break;

                case RequestData.HttpPost:

                    e.ReturnString = DoPost(e.RequestData);

                    break;

                default:
                    e.ReturnString = Support.Html.MakeHtmlPageWithHome("Query was invalid.");
                    break;
            }
        }

        /// <summary>
        /// Handles HTTP POST's
        /// </summary>
        /// <param name="requestData">data parsed from the request</param>
        /// <returns>string to return to the requestor</returns>
        private string DoPost(RequestData requestData)
        {
            var returnString = String.Empty;
            string filename;
            
            switch (requestData.Arguments[0].ToLower())
            {
                case WebServerConstants.Trigger:

                    ExecuteCommandRequest(new CommandRequest(requestData.Body));

                    return WebServerConstants.JsonSuccess;

                case WebServerConstants.Reset:

                    GarageController.CreateGarage("Garage Name", false);

                    GarageController.Garage.AddDoor("Door1");

                    filename = FileUtility.FileStore + ControllerConfigFile;

                    GarageController.Garage.WriteXml(filename);

                    return WebServerConstants.JsonSuccess;

                case WebServerConstants.SetValue:
                    
                    int lightCount;
                    int doorCount;
                    
                    if (requestData.QueryParameters.Count < 5)
                    {
                        returnString = Support.Html.MakeHtmlPageWithHome("Not enough information.  Query parameters are missing.");
                        break;
                    }

                    var garageName = requestData.GetQueryParameter("garageName");
                    var garageLocked = requestData.GetQueryParameter("locked").ToLower() == "true";

                    GarageController.CreateGarage(garageName, garageLocked);

                    var convertSuccess = requestData.GetQueryParameter("doorCount").TryParseInt(out doorCount);

                    if (convertSuccess == false)
                    {
                        returnString =
                            Support.Html.MakeHtmlPageWithHome("Invalid requestData parameter (door count).");
                        break;
                    }

                    convertSuccess = requestData.GetQueryParameter("lightCount").TryParseInt(out lightCount);

                    if (convertSuccess == false)
                    {
                        returnString =
                            Support.Html.MakeHtmlPageWithHome("Invalid requestData parameter (door count).");
                        break;
                    }

                    if (GarageController.Garage == null) break;

                    for (var i = 1; i <= doorCount; i++)
                    {
                        GarageController.Garage.AddDoor(requestData.GetQueryParameter("doorName" + i));
                    }


                    for (var i = 1; i <= lightCount; i++)
                    {
                        GarageController.Garage.AddLight(requestData.GetQueryParameter("lightName" + i), requestData.GetQueryParameter("lightSetting" + i).ToLower() == "on");
                    }

                    filename = FileUtility.FileStore + ControllerConfigFile;

                    GarageController.Garage.WriteXml(filename);

                    // Configuration reset, now reinitialize the hardware
                    ExecuteCommandRequest(new CommandRequest(GarageComponents.Garage, 0, GarageCommands.Reset, null));

                    returnString = "<br/><div class=\"resultDisplay\">\n"
                                   + "<h3>Garage Configuration Update Successful</h3>\n"
                                   + "<h5>Return To:</h5>\n"
                                   + "<h6><a href=\"/index.htm\">Garage Control</a></h6>\n"
                                   + "<h6><a href=\"/setvalue.htm\">Garage Setup</a></h6>\n"
                                   + "</div>\n";

                    break;
            }

            returnString = Support.Html.MakeHtmlPage(returnString);

            return returnString;
        }

      

        private static string DoGet(RequestData requestData)
        {
            string returnString;

            switch (requestData.Arguments[0])
            {
                case WebServerConstants.Configuration:

                    returnString = GarageController.Garage.SerializeJson();

                    break;

                case WebServerConstants.Status:
                    GarageController.Garage.UpdateStatus();

                    returnString = GarageController.Garage.SerializeJson();

                    break;

                case WebServerConstants.Disk:
                    returnString = string.Empty;

                    var path = "\\" + requestData.Query;

                    path = path.Replace('/', '\\');

                    if (Directory.Exists(path))
                    {
                        var body = new StringBuilder();

                        body.Append("<table>");

                        body.Append("<tr><th>Directories in " + path + "</th><th>&nbsp;</th></tr>");
                        
                        var directories = Directory.GetDirectories(path);
                        var files = Directory.GetFiles(path);

                        if (requestData.ArgumentCount > 1)
                        {
                            var parent = "\\";

                            for (var i = 0; i < requestData.ArgumentCount - 1; i++)
                            {
                                parent += requestData.Arguments[i] + "\\";
                            }
                            body.Append("<tr><td><a href = \"" + parent + "\">Up Directory</a></td><td>&nbsp;</td></tr>");
                        }

                        foreach (var dir in directories)
                        {
                            body.Append("<tr><td><a href = \"" + dir.Replace('\\', '/') + "\">" +
                                        dir.Substring(dir.LastIndexOf('\\') + 1) + "</a></td><td>&nbsp;</td></tr>");
                        }
                        
                        body.Append("<tr><td>&nbsp;</td><td>&nbsp;</td></tr>");
                        body.Append("<tr><th>Files in " + path + "</th><th>Size (bytes)</th></tr>");

                        foreach (var file in files)
                        {
                            body.Append("<tr><td><a href = \"" + file.Replace('\\', '/').Substring(4) + "\">" +
                                        file.Substring(file.LastIndexOf('\\') + 1) + "</a></td><td>" +
                                        new FileInfo(file).Length + "</td></tr>");
                        }

                        body.Append("</table>");

                        var page = Support.Html.HtmlPage("<title>Netduino: " + path + "</title>", body.ToString(),
                                                HtmlSupport.GoHomeFooter);

                        returnString = page;
                    }

                    break;

                default:
                    returnString = Support.Html.MakeHtmlPageWithHome("Invalid Command");
                    break;
            }

            return returnString;
        }
    }
}
