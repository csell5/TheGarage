using System;
using System.Text;

namespace Marknic.Web.Utility
{
    public static class StringUtility
    {

        public static string Replace(this string stringIn, char charToReplace, char replacementChar)
        {
            var stringBuilder = new StringBuilder(stringIn);
            
            stringBuilder.Replace(charToReplace, replacementChar);
            
            return stringBuilder.ToString();
        }

        public static string Remove(this string stringIn, char charToRemove)
        {
            int removePos;

            while ((removePos = stringIn.IndexOf(charToRemove)) > -1)
            {
                if (removePos == 0)
                {
                    if (stringIn.Length == 1) return string.Empty;

                    stringIn = stringIn.Substring(1);
                }
                else
                {
                    var endString = stringIn.Substring(removePos + 1);

                    stringIn = stringIn.Substring(0, removePos) + endString;
                }
            }

            return stringIn;
        }


        public static string Remove(this string stringIn, int startChar, int endChar)
        {
            var stringBuilder = new StringBuilder(stringIn);

            stringBuilder.Remove(startChar, endChar);

            return stringBuilder.ToString();
        }

        public static bool TryParseInt(this string numberString, out int value)
        {
            try
            {
                var convertedNumber = int.Parse(numberString);

                value = convertedNumber;

                return true;
            }
            catch (Exception)
            {
                value = 0;

                return false;
            }
        }
    }
}
