using Marknic.Web.Utility;

namespace NetduinoPlus2Garage.Support
{
    internal static class Html
    {
        public const string CrLf = "\r\n";

        public static string HtmlPage(string title, string body)
        {
            return  HtmlSupport.HtmlStartBasic + title + HtmlStartB + body + HtmlSupport.HtmlEnd;
        }

        public static string HtmlPage(string title, string body, string footer)
        {
            return HtmlSupport.HtmlStartBasic + title + HtmlStartB + body + footer + HtmlSupport.HtmlEnd;
        }

        public const string HtmlStart =
            "<!doctype html>\n<html lang=\"en\">\n<head>" + CrLf
            + "<title>Garage Controller</title>" + CrLf
            + "<meta charset=\"UTF-8\">" + CrLf
            + "<meta name=\"description\" content=\"Marknic Garage Controller\">" + CrLf
            + "<link rel=\"stylesheet\" href=\"content/garage.css\">" + CrLf
            + "<script src=\"scripts/jquery-1.7.1.js\"></script>" + CrLf
            + "</head>\r\n<body>" + CrLf;

        public const string HtmlStartB =
            "<meta charset=\"UTF-8\">" + CrLf
            + "<meta name=\"description\" content=\"Marknic Garage Controller\">" + CrLf
            + "<link rel=\"stylesheet\" href=\"content/garage.css\">" + CrLf
            + "<script src=\"scripts/jquery-1.7.1.js\"></script>" + CrLf
            + "</head>\r\n<body>" + CrLf;


        public static string MakeHtmlPageWithHome(string title, string body)
        {
            return HtmlSupport.HtmlStartBasic + title + HtmlStartB + body + HtmlSupport.GoHomeFooter + HtmlSupport.HtmlEnd;
        }

        public static string MakeHtmlPageWithHome(string body)
        {
            return HtmlStart + body + HtmlSupport.GoHomeFooter + HtmlSupport.HtmlEnd;
        }

        public static string MakeHtmlPage(string body)
        {
            return HtmlStart + body + HtmlSupport.HtmlEnd;
        }

    }
}
