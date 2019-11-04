using System;

namespace FlaxCommunity.UnitTesting.Editor
{
    /// <summary>
    /// A test case
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TestCase : Attribute
    {
        public TestCaseData TestCaseData { get; } = new TestCaseData(null);
        public object ExpectedResult
        {
            get => TestCaseData.ExpectedResult;
            set => TestCaseData.ExpectedResult = value;
        }
        public TestCase(object T1)
        {
            TestCaseData.Attributes = new object[] { T1 };
        }

        public TestCase(object T1, object T2)
        {
            TestCaseData.Attributes = new object[] { T1, T2 };
        }

        public TestCase(object T1, object T2, object T3)
        {
            TestCaseData.Attributes = new object[] { T1, T2, T3 };
        }

        public TestCase(object T1, object T2, object T3, object T4)
        {
            TestCaseData.Attributes = new object[] { T1, T2, T3, T4 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5)
        {
            TestCaseData.Attributes = new object[] { T1, T2, T3, T4, T5 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5, object T6)
        {
            TestCaseData.Attributes = new object[] { T1, T2, T3, T4, T5, T6 };
        }

        public TestCase(object T1, object T2, object T3, object T4, object T5, object T6, object T7)
        {
            TestCaseData.Attributes = new object[] { T1, T2, T3, T4, T5, T6, T7 };
        }
    }

    /// <summary>
    /// Specifies a test case source method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class TestCaseSource : Attribute
    {
        public readonly string MemberName;
        public readonly Type SourceClassType;
        public TestCaseSource(string memberName) : this(null, memberName)
        {
        }

        public TestCaseSource(Type sourceClassType, string memberName)
        {
            SourceClassType = sourceClassType;
            MemberName = memberName;
        }
    }

    /// <summary>
    /// A single test
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class Test : Attribute
    {

    }

    /// <summary>
    /// Executed before every single test
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SetUp : Attribute
    {
    }

    /// <summary>
    /// Executed after every single test
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class TearDown : Attribute
    {
    }

    /// <summary>
    /// Executed before all tests 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OneTimeSetUp : Attribute
    {
    }

    /// <summary>
    /// Executed after all tests
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OneTimeTearDown : Attribute
    {
    }

    /// <summary>
    /// Specifies a class as a unit test class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TestFixture : Attribute
    {
    }
}