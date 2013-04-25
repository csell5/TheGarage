using System;

namespace Marknic.NdGarageDoorLightsController.Utility
{
    public static class TimeUtility
    {
        public static long ConvertTimeSpanToSeconds(TimeSpan timespan)
        {
            return timespan.Days * 86400 + timespan.Hours * 3600 + timespan.Minutes * 60 + timespan.Seconds;
        }
    }
}
