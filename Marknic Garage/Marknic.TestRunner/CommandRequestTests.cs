using Marknic.TestRunner.Rigging;
using Marknic.Web.RequestResponse;

namespace Marknic.TestRunner
{
    public class CommandRequestTests
    {
        public void TestEx_CommandRequestTests_WhenCreatingCommandRequestWithoutThreeValuesShouldThrowException()
        {
            const string badJson = "{\"component\": \"door\", \"componentNumber\": 0 }";

            var commandRequest = new CommandRequest(badJson);
        }

        public void TestEx_CommandRequestTests_WhenCreatingCommandRequestWithoutPairsOfValuesShouldThrowException()
        {
            const string badJson = "{\"component\": \"door\", \"componentNumber\" 0 , \"command\": \"toggle\" }";

            var commandRequest = new CommandRequest(badJson);
        }

        public void Test_CommandRequestTests_WhenCreatingCommandRequestWithGoodValuesShouldReturnObjectWithCorrectValues()
        {
            const string badJson = "{ \"component\": \"door\", \"componentNumber\": 0, \"command\": \"toggle\"}";

            var commandRequest = new CommandRequest(badJson);

            Assert.AreEqual("door", commandRequest.Component);
            Assert.AreEqual(0, commandRequest.ComponentNumber);
            Assert.AreEqual("toggle", commandRequest.Command);
        }

        public void TestEx_CommandRequestTests_WhenCreatingCommandRequestWithNoComponentNumberShouldThrowException()
        {
            const string badJson = "{\"component\": \"door\", \"componentNo\": 0, \"command\": \"toggle\"}";

            var commandRequest = new CommandRequest(badJson);
        }

        public void TestEx_CommandRequestTests_WhenCreatingCommandRequestWithNoComponentShouldThrowException()
        {
            const string badJson = "{\"componentt\": \"door\", \"componentNumber\": 0, \"command\": \"toggle\"}";

            var commandRequest = new CommandRequest(badJson);
        }

        public void TestEx_CommandRequestTests_WhenCreatingCommandRequestWithNoCommandShouldThrowException()
        {
            const string badJson = "{\"component\": \"door\", \"componentNumber\": 0, \"commmand\": \"toggle\"}";

            var commandRequest = new CommandRequest(badJson);
        }
    }
}
