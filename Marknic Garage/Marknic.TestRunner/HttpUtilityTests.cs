using Marknic.TestRunner.Rigging;
using Marknic.Web.Utility;

namespace Marknic.TestRunner
{
    public class HttpUtilityTests
    {
        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCode0()
        {
            var result = HttpUtility.HexToInt('0');

            Assert.AreEqual(0, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCode9()
        {
            var result = HttpUtility.HexToInt('9');

            Assert.AreEqual(9, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCodeA()
        {
            var result = HttpUtility.HexToInt('A');

            Assert.AreEqual(10, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCodeF()
        {
            var result = HttpUtility.HexToInt('F');

            Assert.AreEqual(15, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCodea()
        {
            var result = HttpUtility.HexToInt('a');

            Assert.AreEqual(10, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnAppropriateHexCodef()
        {
            var result = HttpUtility.HexToInt('f');

            Assert.AreEqual(15, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnNegative1G()
        {
            var result = HttpUtility.HexToInt('G');

            Assert.AreEqual(-1, result);
        }

        public void Test_HttpUtilityTests_WhenUsingHexToIntShouldReturnNegative1g()
        {
            var result = HttpUtility.HexToInt('g');

            Assert.AreEqual(-1, result);
        }

        public void Test_HttpUtilityTests_WhenUrlDecodeCalledWithNullParmShouldReturnNull()
        {
            var result = HttpUtility.UrlDecode(null);

            Assert.IsNull(result);
        }

        public void Test_HttpUtilityTests_WhenUrlDecodeCalledWithEmptyParmShouldReturnParm()
        {
            var result = HttpUtility.UrlDecode(string.Empty);

            Assert.AreEqual(string.Empty, result);
        }

    }
}
