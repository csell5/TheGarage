using System;
using Marknic.NdGarageDoorLightsController.Utility;
using Marknic.TestRunner.Rigging;

namespace Marknic.TestRunner
{
    public class TimeUtilityTests
    {
        public void Test_TimeUtilityTests_WhenConvertingWithDaysShouldRespondWithCorrectSeconds()
        {
            var timeSpan = new TimeSpan(10, 10, 10, 10);
            var result = TimeUtility.ConvertTimeSpanToSeconds(timeSpan);

            Assert.AreEqual(10 * 86400 + 10 * 3600 + 10 * 60 + 10, result);
        }

        public void Test_TimeUtilityTests_WhenConvertingWithHoursShouldRespondWithCorrectSeconds()
        {
            var timeSpan = new TimeSpan(0, 10, 10, 10);
            var result = TimeUtility.ConvertTimeSpanToSeconds(timeSpan);

            Assert.AreEqual(10 * 3600 + 10 * 60 + 10, result);
        }

        public void Test_TimeUtilityTests_WhenConvertingWithMinutesShouldRespondWithCorrectSeconds()
        {
            var timeSpan = new TimeSpan(0, 0, 10, 10);
            var result = TimeUtility.ConvertTimeSpanToSeconds(timeSpan);

            Assert.AreEqual(10 * 60 + 10, result);
        }

        public void Test_TimeUtilityTests_WhenConvertingWithSecondsShouldRespondWithCorrectSeconds()
        {

            var timeSpan = new TimeSpan(0, 0, 0, 10);
            var result = TimeUtility.ConvertTimeSpanToSeconds(timeSpan);

            Assert.AreEqual(10, result);
        }

        public void Test_TimeUtilityTests_WhenRunningTestsShouldFailOnPurposeAsExample()
        {

            var timeSpan = new TimeSpan(0, 0, 0, 10);
            var result = TimeUtility.ConvertTimeSpanToSeconds(timeSpan);

            Assert.AreEqual(100, result);
        }
    }
}
