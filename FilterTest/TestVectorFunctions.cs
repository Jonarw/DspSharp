using Filter.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilterTest
{
    [TestClass]
    public class TestVectorFunctions
    {
        private readonly double[] input = {5, 6, 1, 100, 2, 3};

        [TestMethod]
        public void TestMaxIndex()
        {
            Assert.AreEqual(this.input.MaxIndex(), 3);
        }

        [TestMethod]
        public void TestMinIndex()
        {
            Assert.AreEqual(this.input.MinIndex(), 2);
        }
    }
}