using System.Linq;
using Filter.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilterTest
{
    [TestClass]
    public class TestVectorOperations
    {
        private readonly double[] input = {1, 2, 3, 4, 5, 6, 7, 8};

        [TestMethod]
        public void TestCircularShift()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.CircularShift(0).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.CircularShift(8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.CircularShift(-8).ToReadOnlyList());

            double[] target = {3, 4, 5, 6, 7, 8, 1, 2};
            FilterAssert.ListsAreReasonablyClose(target, this.input.CircularShift(2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.input.CircularShift(10).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.input.CircularShift(-6).ToReadOnlyList());
        }

        [TestMethod]
        public void TestGetCircularRange()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.GetCircularRange(0, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.GetCircularRange(8, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.GetCircularRange(-8, 8).ToReadOnlyList());

            double[] target = {3, 4, 5, 6, 7, 8, 1, 2};
            FilterAssert.ListsAreReasonablyClose(target, this.input.GetCircularRange(2, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.input.GetCircularRange(10, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.input.GetCircularRange(-6, 8).ToReadOnlyList());

            double[] target2 = {3, 4};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.GetCircularRange(2, 2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target2, this.input.GetCircularRange(10, 2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target2, this.input.GetCircularRange(-6, 2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestGetPaddedRange()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.GetPaddedRange(0, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(SignalGenerators.GetZeros(8), this.input.GetPaddedRange(8, 8).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(SignalGenerators.GetZeros(8), this.input.GetPaddedRange(-8, 8).ToReadOnlyList());

            double[] target = {3, 4, 5, 6, 7, 8, 0, 0};
            FilterAssert.ListsAreReasonablyClose(target, this.input.GetPaddedRange(2, 8).ToReadOnlyList());

            double[] target2 = {0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.GetPaddedRange(-2, 12).ToReadOnlyList());
        }

        [TestMethod]
        public void TestGetRangeOptimized()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.GetRangeOptimized(0, 8).ToReadOnlyList());

            Assert.IsTrue(this.input.GetRangeOptimized(10, 10).ToReadOnlyList().Count == 0);

            double[] target = {3, 4, 5, 6, 7, 8};
            FilterAssert.ListsAreReasonablyClose(target, this.input.GetRangeOptimized(2, 8).ToReadOnlyList());

            double[] target2 = {3, 4, 5, 6, 7};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.GetRangeOptimized(2, 5).ToReadOnlyList());
        }

        [TestMethod]
        public void TestInterleaveEnumerations()
        {
            double[] target = {1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8};
            FilterAssert.ListsAreReasonablyClose(target, this.input.InterleaveEnumerations(this.input).ToReadOnlyList());

            double[] target2 = {1, 1, 2, 2, 3, 3, 4, 4};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.InterleaveEnumerations(this.input.Take(4)).ToReadOnlyList());
        }

        [TestMethod]
        public void TestLoop()
        {
            var target = this.input.Concat(this.input).Concat(this.input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, this.input.Loop(3).ToReadOnlyList());

            Assert.IsTrue(this.input.Loop(0).ToReadOnlyList().Count == 0);
        }

        [TestMethod]
        public void TestPadLeft()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.PadLeft(0).ToReadOnlyList());

            double[] target = {0, 0, 1, 2, 3, 4, 5, 6, 7, 8};
            FilterAssert.ListsAreReasonablyClose(target, this.input.PadLeft(2).ToReadOnlyList());

            double[] target2 = {1, 2, 3, 4, 5, 6, 7, 8, 0, 0};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.PadRight(2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestSparseSequence()
        {
            FilterAssert.ListsAreReasonablyClose(this.input, this.input.SparseSequence(1).ToReadOnlyList());

            double[] target = {2, 4, 6, 8};
            FilterAssert.ListsAreReasonablyClose(target, this.input.SparseSequence(2).ToReadOnlyList());

            double[] target2 = {3, 6};
            FilterAssert.ListsAreReasonablyClose(target2, this.input.SparseSequence(3).ToReadOnlyList());

            Assert.IsTrue(this.input.SparseSequence(9).ToReadOnlyList().Count == 0);
        }
    }
}