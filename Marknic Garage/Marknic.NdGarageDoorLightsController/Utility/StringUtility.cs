namespace Marknic.NdGarageDoorLightsController.Utility
{
    public static class StringUtility
    {
        public static string TrimIt(this string stringToTrim)
        {
            int i; int j;

            for (i = 0; (stringToTrim[i] == ' ' || stringToTrim[i] == '\t'); i++) { }

            for (j = stringToTrim.Length - 1; (stringToTrim[j] == ' ' || stringToTrim[j] == '\t'); j--) { }

            return stringToTrim.Substring(i, j - i + 1);
        }

        public static bool IsNullOrEmpty(this string stringToCheck)
        {
            if (stringToCheck == null) return true;

            return stringToCheck.Trim() == string.Empty;
        }
    }
}
