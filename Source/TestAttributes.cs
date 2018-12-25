using System;

namespace FlaxEngine.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCase : Attribute
    {
        public readonly object[] Attributes;
        public object ExpectedResult { get; set; }
        public TestCase(object T1)
        {
            Attributes = new object[] { T1 };
        }

        public TestCase(object T1, object T2)
        {
            Attributes = new object[] { T1, T2 };
        }

        public TestCase(object T1, object T2, object T3)
        {
            Attributes = new object[] { T1, T2, T3 };
        }

        public TestCase(object T1, object T2, object T3, object T4)
        {
            Attributes = new object[] { T1, T2, T3, T4 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5)
        {
            Attributes = new object[] { T1, T2, T3, T4, T5 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5, object T6)
        {
            Attributes = new object[] { T1, T2, T3, T4, T5, T6 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5, object T6, object T7)
        {
            Attributes = new object[] { T1, T2, T3, T4, T5, T6, T7 };
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Test : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class SetUp : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TearDown : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TestFixture : Attribute
    {
    }
}