using System.Reflection;
using Marknic.TestRunner.Rigging;

namespace Marknic.TestRunner
{
    public class TestRunner
    {        
        public static void Main()
        {
            TestRig.ExecuteTests(Assembly.GetExecutingAssembly());
        }
    }
}