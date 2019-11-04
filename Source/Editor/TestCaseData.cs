using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaxCommunity.UnitTesting.Editor
{
    public class TestCaseData
    {
        public TestCaseData(params object[] attributes)
        {
            Attributes = attributes;
        }

        public object[] Attributes { get; set; }

        public object ExpectedResult { get; set; }

        public TestCaseData Returns(object expectedResult)
        {
            ExpectedResult = expectedResult;
            return this;
        }
    }
}
