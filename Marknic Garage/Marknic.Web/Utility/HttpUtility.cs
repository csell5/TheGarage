using System;
using System.Text;

namespace Marknic.Web.Utility
{
    public static class HttpUtility
    {
        public static string UrlDecode(string rawUrlString)
        {
            if (rawUrlString == null) return null;
            if (rawUrlString.Length < 1) return rawUrlString;

            var chars = rawUrlString.ToCharArray();
            var bytes = new byte[chars.Length * 2];
            var count = chars.Length;
            var dstIndex = 0;
            var srcIndex = 0;

            while (true)
            {
                if (srcIndex >= count)
                {
                    if (dstIndex < srcIndex)
                    {
                        var sizedBytes = new byte[dstIndex];
                        Array.Copy(bytes, 0, sizedBytes, 0, dstIndex);
                        bytes = sizedBytes;
                    }
                    return new string(Encoding.UTF8.GetChars(bytes));
                }

                if (chars[srcIndex] == '+')
                {
                    bytes[dstIndex++] = (byte)' ';
                    srcIndex += 1;
                }
                else if (chars[srcIndex] == '%' && srcIndex < count - 2)
                    if (chars[srcIndex + 1] == 'u' && srcIndex < count - 5)
                    {
                        var ch1 = HexToInt(chars[srcIndex + 2]);
                        var ch2 = HexToInt(chars[srcIndex + 3]);
                        var ch3 = HexToInt(chars[srcIndex + 4]);
                        var ch4 = HexToInt(chars[srcIndex + 5]);

                        if (ch1 >= 0 && ch2 >= 0 && ch3 >= 0 && ch4 >= 0)
                        {
                            bytes[dstIndex++] = (byte)((ch1 << 4) | ch2);
                            bytes[dstIndex++] = (byte)((ch3 << 4) | ch4);
                            srcIndex += 6;
                        }
                    }
                    else
                    {
                        var ch1 = HexToInt(chars[srcIndex + 1]);
                        var ch2 = HexToInt(chars[srcIndex + 2]);

                        if (ch1 >= 0 && ch2 >= 0)
                        {
                            bytes[dstIndex++] = (byte)((ch1 << 4) | ch2);
                            srcIndex += 3;
                        }
                    }
                else
                {
                    var charBytes = Encoding.UTF8.GetBytes(chars[srcIndex++].ToString());
                    charBytes.CopyTo(bytes, dstIndex);
                    dstIndex += charBytes.Length;
                }
            }
        }

        public static int HexToInt(char ch)
        {
            return
                (ch >= '0' && ch <= '9') ? ch - '0' :
                (ch >= 'a' && ch <= 'f') ? ch - 'a' + 10 :
                (ch >= 'A' && ch <= 'F') ? ch - 'A' + 10 :
                -1;
        }

 
    }
}
