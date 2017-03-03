using System;
using System.Linq;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class MlsTest
    {
        //[TestMethod]
        public void TestMls()
        {
            for (int i = 2; i < SignalGenerators.MlsFeedbackTaps.Count; i++)
            {
                var sequence = SignalGenerators.Mls(i);

                Assert.IsTrue(sequence.Count() == Math.Pow(2, i) - 1);
            }

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.Mls(1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.Mls(SignalGenerators.MlsFeedbackTaps.Count).ToReadOnlyList());
        }
    }
}