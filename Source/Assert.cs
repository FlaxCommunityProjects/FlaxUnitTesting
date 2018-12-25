using System;

namespace FlaxEngine.UnitTesting
{
    /// <summary>
    /// Special type of exception that is used to terminate the test case early <seealso cref="Assert.Pass"/>
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class SuccessException : Exception { }

    public static class Assert
    {
        public static void Pass() => throw new SuccessException();
        public static void Fail() => throw new Exception();

        // TODO: use Equals instead of ==
        public static void AreEqual(object a, object b) { if (a != b) throw new Exception(); }
        public static void AreNotEqual(object a, object b) { if (a == b) throw new Exception(); }

        public static void True(bool a) { if (!a) throw new Exception(); }
        public static void False(bool a) { if (a) throw new Exception(); }
    }
}