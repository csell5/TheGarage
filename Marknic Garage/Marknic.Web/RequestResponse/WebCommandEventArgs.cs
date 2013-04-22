namespace Marknic.Web.RequestResponse
{
    public class WebCommandEventArgs
    {
        /// <summary>
        /// ctor 
        /// </summary>
        /// <param name="requestData"></param>
        public WebCommandEventArgs(RequestData requestData)
        {
            PerformShutdown = false;

            RequestData = requestData;
        }

        /// <summary>
        /// Query components for the current request
        /// </summary>
        public RequestData RequestData { get; private set; }

        /// <summary>
        /// String that will be returned to the caller (HTML, JSON, etc.)
        /// </summary>
        public string ReturnString { get; set; }
        
        /// <summary>
        /// Use to indicate the device should shutdown
        /// </summary>
        public bool PerformShutdown { get; set; }
    }
}