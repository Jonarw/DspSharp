using System;
using System.Linq;
using Filter.Algorithms;
using Filter.Extensions;
using NUnit.Framework;

namespace FilterTests
{
    [TestFixture]
    public class MlsTest
    {
        //[Test]
        public void TestMls()
        {
            for (int i = 2; i < SignalGenerators.MlsFeedbackTaps.Count; i++)
            {
                var sequence = SignalGenerators.GenerateMls(i);

                Assert.That(sequence.Count() == Math.Pow(2, i) - 1);
            }

            Assert.Catch<ArgumentOutOfRangeException>(() => SignalGenerators.GenerateMls(1).ToReadOnlyList());
            Assert.Catch<ArgumentOutOfRangeException>(() => SignalGenerators.GenerateMls(SignalGenerators.MlsFeedbackTaps.Count).ToReadOnlyList());
        }
    }
}