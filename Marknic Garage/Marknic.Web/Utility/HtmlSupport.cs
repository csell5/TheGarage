namespace Marknic.Web.Utility
{
    public class HtmlSupport
    {
        public const string CrLf = "\r\n";

        public const string ShowErrorPage =
            "<!DOCTYPE html>"
            + "\n<head>"
            + "\n    <title>Microcontroller Web Server</title>"
            + "\n</head>"
            + "\n<body style=\"font-family: Arial, Helvetica, sans-serif;\">"
            + "\n    <div style=\"font-size: large; font-weight: bold; color: #0000FF\">"
            + "\n        Welcome to the Web Server"
            + "\n    </div>"
            + "\n    <br />"
            + "\n    Unfortunately there has been a problem retrieving the specified HTML page"
            + "\n    <br />"
            + "\n</body>"
            + "\n</html>";

        public const string HtmlStartBasic =
            "<!doctype html>\n<html lang=\"en\">\n<head>" + CrLf;

        public const string GoHomeFooter = "<br /><br /><a href=\"/\"> Go to Home Page </a>";
        public const string HtmlEnd = "</body></html>";
        public const string HtmlBr = "<br>";

        public const string HtmlStart =
            "<!doctype html>\n<html lang=\"en\">\n<head>" + CrLf
            + "<title>Web Server</title>" + CrLf
            + "<meta charset=\"UTF-8\">" + CrLf
            + "</head>\r\n<body>" + CrLf;

        public static string FormatResponse(string outDocument)
        {
            if (outDocument == null)
            {
                outDocument = HtmlStart + "<strong>Error - no output</strong>" + GoHomeFooter + HtmlEnd;
            }

            var outString = "HTTP/1.1 200 OK" + CrLf
                            + "Connection: close" + CrLf
                            + "Content-Type: text/html; charset=utf-8" + CrLf
                            + "Content-Length: " + outDocument.Length + CrLf
                            + CrLf
                            + outDocument;

            return outString;
        }

        public static string FormatResponse(string outDocument, string contentType)
        {
            if (outDocument == null)
            {
                outDocument = HtmlStart + "<strong>Error - no output</strong>" + GoHomeFooter + HtmlEnd;
            }

            var outString = "HTTP/1.1 200 OK" + CrLf
                            + "Connection: close" + CrLf
                            + "Content-Type: " + GetContentType(contentType)
                            + "Content-Length: " + outDocument.Length + CrLf
                            + CrLf
                            + outDocument;

            return outString;
        }

        public static string GetResourceExtension(string resourceString)
        {
            var resourceExtension = string.Empty;

            resourceString = resourceString.Trim();

            var lastChar = resourceString[resourceString.Length - 1];

            while ((lastChar == '\\') || (lastChar == '/'))
            {
                var lastCharPos = resourceString.Length - 1;

                resourceString = resourceString.Remove(lastCharPos, lastCharPos);

                lastChar = resourceString[resourceString.Length - 1];
            }

            var lastDot = resourceString.LastIndexOf('.');

            if (lastDot > 0)
            {
                resourceExtension = resourceString.Substring(resourceString.LastIndexOf('.') + 1);
            }

            return resourceExtension;
        }


        public static string GetContentType(string resourceExtension)
        {
            switch (resourceExtension)
            {
                case "html":
                case "htm":
                    return "text/html; charset=utf-8";

                case "css":
                    return "text/css";

                case "config":
                    return "text/xml";

                case "txt":
                    return "text/plain";

                case "js":
                    return "text/javascript";

                case "jpg":
                case "jpeg":
                    return "image/jpeg";
                
                case "png":
                    return "image/png";
                
                case "gif":
                    return "image/gif";
                
                case "ico":
                    return "image/png";
                
                default:
                    return "unknown/unknown";
            }
        }

    }
}
