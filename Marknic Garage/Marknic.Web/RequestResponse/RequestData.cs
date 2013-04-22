using System.Collections;
using Marknic.Web.Utility;

namespace Marknic.Web.RequestResponse
{
    public class RequestData
    {
        public const string HttpGet = "GET";
        public const string HttpPost = "POST";
        public const string HttpDelete = "DELETE";
        public const string HttpPut = "PUT";
        public const string FavIcon = "favicon.ico";

        // Read only Properties
        public string Query { get; private set; }

        public ArrayList QueryParameters { get; private set; }

        public string HttpVerb { get; private set; }

        /// <summary>
        /// Number of arguments needed with this command.
        /// </summary>
        public int ArgumentCount { get; private set; }

        /// <summary>
        /// When a command is received, this property holds the actual argument values.
        /// </summary>
        public string[] Arguments { get; private set; }

        /// <summary>
        /// Host making the request
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// Version of HTTP in the request
        /// </summary>
        public string HttpVersion { get; private set; }

        /// <summary>
        /// Request body
        /// </summary>
        public string Body { get; private set; }

        /// <summary>
        /// CTOR - parses the raw query string into its usable parts
        /// </summary>
        /// <param name="rawUrlQuery">Raw HTTP request string</param>
        public RequestData(string rawUrlQuery)
        {
            rawUrlQuery = rawUrlQuery.Trim();

            var verbPos = rawUrlQuery.IndexOf(' ');
            var httpPos = rawUrlQuery.IndexOf(' ', verbPos + 1);
// ReSharper disable StringIndexOfIsCultureSpecific.2
            var bodyPos = rawUrlQuery.IndexOf("\r\n\r\n", 0);
// ReSharper restore StringIndexOfIsCultureSpecific.2

            Host = string.Empty;
            HttpVerb = rawUrlQuery.Substring(0, verbPos).Trim();
            Query = rawUrlQuery.Substring(verbPos + 1, httpPos - verbPos - 1).Trim();

            // Is there a body?
            if (bodyPos > -1)
            {
                Body = rawUrlQuery.Substring(bodyPos + 4);
            }

            if (Query.Length == 1)
            {
                ArgumentCount = 0;
            }
            else if (Query.Length > 1)
            {
                if (Query[0] == '/')
                {
                    Query = Query.Substring(1);
                }

                Arguments = Query.Split('/');

                ArgumentCount = Arguments.Length;

                if ((Arguments[Arguments.Length - 1] != null) && (Arguments[Arguments.Length - 1] == string.Empty))
                {
                    var tmpArray = new string[Arguments.Length - 1];

                    for (var i = 0; i < tmpArray.Length; i++)
                    {
                        tmpArray[i] = Arguments[i];
                    }

                    Arguments = tmpArray;
                    ArgumentCount = tmpArray.Length;
                }
            }

            var rawQueryParms = rawUrlQuery.Substring(httpPos + 1);

            var rawQueryParmArray = rawQueryParms.Split("\r\n".ToCharArray());

            var queryParmString = string.Empty;

            foreach (var parm in rawQueryParmArray)
            {
                if ((parm == string.Empty) || (parm.Length < 5)) continue;

                if (parm.Substring(0, 5).ToLower() == "http/")
                {
                    HttpVersion = parm;
                }
                else if (parm.Substring(0, 4).ToLower() == "host")
                {
                    Host = parm.Substring(6);
                    break;
                }
            }

            if (rawQueryParmArray.Length > 2)
            {
                Host = rawQueryParmArray[2];    
            }

            switch (HttpVerb)
            {
                case HttpPost:
                    queryParmString = rawQueryParmArray[rawQueryParmArray.Length - 1].Trim();
                    break;

                default:
                    if (ArgumentCount > 0)
                    {
                        var parms = Arguments[ArgumentCount - 1];

                        var parmsArray = parms.Split('?');
                        Arguments[ArgumentCount - 1] = parmsArray[0];

                        if (parmsArray.Length == 2)
                        {
                            queryParmString = HttpUtility.UrlDecode(parmsArray[1]).Trim();
                        }
                    }
                    break;
            }

            ArrayList queryParms = null;

            if (queryParmString != string.Empty)
            {
                queryParmString = queryParmString.Remove('{').Remove('}').Remove('\"');

                var queryParmList = queryParmString.Split(',');

                queryParms = new ArrayList();

                for (var i = 0; i < queryParmList.Length; i++)
                {
                    var set = queryParmList[i].Split(':');

                    var convertedVal = HttpUtility.UrlDecode(set[1]);

                    queryParms.Add(new DictionaryEntry(set[0], convertedVal));
                }
            }

            QueryParameters = queryParms;
        }

        /// <summary>
        /// Returns a query parameter as a string based on it's key
        /// </summary>
        /// <param name="key">Query parameter name</param>
        /// <returns>Query parameter value if it exists, null otherwise</returns>
        public string GetQueryParameter(string key)
        {
            foreach (DictionaryEntry queryParameter in QueryParameters)
            {
                if ((string)queryParameter.Key == key)
                {
                    return queryParameter.Value as string;
                }
            }

            return null;
        }
    }
}
