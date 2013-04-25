using System;
using System.Collections;
using System.Reflection;
using Microsoft.SPOT;

namespace Marknic.TestRunner.Rigging
{
    public class Result
    {
        public string MethodName { get; set; }
        public bool Succeeded { get; set; }
        public Exception Exception { get; set; }
    }

    public static class Assert
    {
        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception(message);
            }
        }

        public static void IsFalse(bool condition, string message)
        {
            if (condition)
            {
                throw new Exception(message);
            }
        }

        public static void IsTrue(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Result should have been true - result was false");
            }
        }

        public static void IsFalse(bool condition)
        {
            if (condition)
            {
                throw new Exception("Result should have been false - result was true");
            }
        }

        public static void AreEqual(int expected, int actual)
        {
            if (expected != actual)
            {
                throw new Exception("Expected value: " + expected + " - Actual value: " + actual);
            }
        }

        public static void AreEqual(long expected, long actual)
        {
            if (expected != actual)
            {
                throw new Exception("Expected value: " + expected + " - Actual value: " + actual);
            }
        }

        public static void AreEqual(string expected, string actual)
        {
            if (expected != actual)
            {
                throw new Exception("Expected value: " + expected + " - Actual value: " + actual);
            }
        }

        public static void IsNotNull(object actual)
        {
            if (actual == null)
            {
                throw new Exception("Object tested was null");
            }
        }

        public static void IsNull(object actual)
        {
            if (actual != null)
            {
                throw new Exception("Object tested was not null");
            }
        }

        public static void AreEqual(object expected, object actual)
        {
            if (expected == null)
            {
                throw new Exception("Expected object was null");
            }

            if (actual == null)
            {
                throw new Exception("actual object was null");
            }

            if (expected != actual)
            {
                throw new Exception("Objects are not the same: " + expected + " - Actual: " + actual);
            }
        }
    }

    public static class TestRig
    {
        private static readonly ArrayList ResultList = new ArrayList();
        private static int _totalCount;
        private static int _failureCount;

        public static void ExecuteTests(Assembly assembly)
        {
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                var methodList = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var method in methodList)
                {
                    // Do we have a test?
                    if (!method.Name.StartsWith("Test")) continue;

                    _totalCount++;

                    var result = new Result { MethodName = method.Name, Succeeded = true };

                    // Are we testing that the method throws an exception?
                    if (method.Name.StartsWith("TestEx"))
                    {
                        result.Succeeded = false;
                    }

                    try
                    {
                        method.Invoke(null, null);

                        // If an exception was expected then create one for reporting
                        if (!result.Succeeded)
                        {
                            result.Exception = new Exception("An exception should have been thrown");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Succeeded = !result.Succeeded;
                        result.Exception = ex;
                    }

                    if (!result.Succeeded) _failureCount++;

                    ResultList.Add(result);
                }
            }

            DisplayTestResults();
        }


        private static void DisplayTestResults()
        {
            Debug.Print("\n");
            Debug.Print("-------------------------------------------------------------");
            Debug.Print("Test Results");
            Debug.Print("-------------------------------------------------------------");
            Debug.Print("     Total Tests: " + _totalCount);
            Debug.Print("Successful Tests: " + (_totalCount - _failureCount));
            Debug.Print("   Test Failures: " + _failureCount);
            Debug.Print("\n");

            Debug.Print("Failed Tests:");
            Debug.Print("-------------------------------------------------------------");
            foreach (Result result in ResultList)
            {
                if (!result.Succeeded)
                {
                    Debug.Print("Name: " + result.MethodName + " - Reason: " + result.Exception.Message);
                }
            }
            Debug.Print("-------------------------------------------------------------\n");
        }
    }
}
