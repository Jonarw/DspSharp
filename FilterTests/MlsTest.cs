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
            for (int i = 2; i < Mls.FeedbackTaps.Count; i++)
            {
                var sequence = Mls.GenerateMls(i);

                Assert.That(sequence.Count() == Math.Pow(2, i) - 1);
            }

            Assert.Catch<ArgumentOutOfRangeException>(() => Mls.GenerateMls(1).ToReadOnlyList());
            Assert.Catch<ArgumentOutOfRangeException>(() => Mls.GenerateMls(Mls.FeedbackTaps.Count).ToReadOnlyList());
        }
    }
}