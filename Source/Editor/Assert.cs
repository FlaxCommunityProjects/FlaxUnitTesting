using System;

namespace FlaxCommunity.UnitTesting.Editor
{
    /// <summary>
    /// Special type of exception that is used to terminate the test case early <seealso cref="Assert.Pass"/>
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SuccessException : Exception
    {
        public SuccessException()
        {
        }

        public SuccessException(string message) : base(message)
        {
        }

        public SuccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// Exception thrown when an assertion fails
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AssertException : Exception
    {
        public AssertException()
        {
        }

        public AssertException(string message) : base(message)
        {
        }

        public AssertException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static class Assert
    {
        public static void Pass() => throw new SuccessException();
        public static void Fail() => throw new AssertException("Fail");

        public static void AreEqual(object a, object b) { if (!Equals(a, b)) throw new AssertException($"{a} does not equal {b}"); }
        public static void AreNotEqual(object a, object b) { if (Equals(a, b)) throw new Exception($"{a} is equal to {b}"); }

        public static void True(bool a) { if (!a) throw new Exception($"{a} is not true"); }
        public static void False(bool a) { if (a) throw new Exception($"{a} is not false"); }
    }
}