using Filter.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilterTest
{
    [TestClass]
    public class TestVectorArithmeticD
    {
        private readonly double[] list1 = {1, 2, 3, 4};
        private readonly double[] list2 = {.1, .2, .3, .4, .5, .6, .7, .8};

        [TestMethod]
        public void TestAdd()
        {
            double[] target = {1.1, 2.2, 3.3, 4.4};

            FilterAssert.ListsAreReasonablyClose(target, this.list2.Add(this.list1).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.list1.Add(this.list2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestAddFull()
        {
            double[] target = {1.1, 2.2, 3.3, 4.4, .5, .6, .7, .8};

            FilterAssert.ListsAreReasonablyClose(target, this.list2.AddFull(this.list1).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.list1.AddFull(this.list2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestAddFullWithOffset()
        {
            double[] target1 = {1.1, 2.2, 3.3, 4.4, .5, .6, .7, .8};
            FilterAssert.ListsAreReasonablyClose(target1, this.list2.AddFullWithOffset(this.list1, 0).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target1, this.list1.AddFullWithOffset(this.list2, 0).ToReadOnlyList());

            double[] target2 = {1, 2, 3.1, 4.2, .3, .4, .5, .6, .7, .8};
            FilterAssert.ListsAreReasonablyClose(target2, this.list1.AddFullWithOffset(this.list2, 2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target2, this.list2.AddFullWithOffset(this.list1, -2).ToReadOnlyList());

            double[] target3 = {.1, .2, 1.3, 2.4, 3.5, 4.6, .7, .8};
            FilterAssert.ListsAreReasonablyClose(target3, this.list1.AddFullWithOffset(this.list2, -2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target3, this.list2.AddFullWithOffset(this.list1, 2).ToReadOnlyList());

            double[] target4 = {1, 2, 3, 4, 0, .1, .2, .3, .4, .5, .6, .7, .8};
            FilterAssert.ListsAreReasonablyClose(target4, this.list1.AddFullWithOffset(this.list2, 5).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target4, this.list2.AddFullWithOffset(this.list1, -5).ToReadOnlyList());

            double[] target5 = {.1, .2, .3, .4, .5, .6, 1.7, 2.8, 3, 4};
            FilterAssert.ListsAreReasonablyClose(target5, this.list1.AddFullWithOffset(this.list2, -6).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target5, this.list2.AddFullWithOffset(this.list1, 6).ToReadOnlyList());
        }

        [TestMethod]
        public void TestAddS()
        {
            double[] target = {3, 4, 5, 6};

            FilterAssert.ListsAreReasonablyClose(target, this.list1.Add(2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestDivide()
        {
            double[] target1 = {10, 10, 10, 10};
            double[] target2 = {.1, .1, .1, .1};

            FilterAssert.ListsAreReasonablyClose(target1, this.list1.Divide(this.list2).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target2, this.list2.Divide(this.list1).ToReadOnlyList());
        }

        [TestMethod]
        public void TestDivideS()
        {
            double[] target1 = {.5, 1, 1.5, 2};
            double[] target2 = {2, 1, 2d / 3, .5};

            FilterAssert.ListsAreReasonablyClose(target1, this.list1.Divide(2d).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target2, 2d.Divide(this.list1).ToReadOnlyList());
        }

        [TestMethod]
        public void TestMultiply()
        {
            double[] target = {.1, .4, .9, 1.6};

            FilterAssert.ListsAreReasonablyClose(target, this.list2.Multiply(this.list1).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.list1.Multiply(this.list2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestMultiplyS()
        {
            double[] target = {2, 4, 6, 8};

            FilterAssert.ListsAreReasonablyClose(target, this.list1.Multiply(2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestSubtract()
        {
            double[] target = {.9, 1.8, 2.7, 3.6};

            FilterAssert.ListsAreReasonablyClose(target, this.list2.Subtract(this.list1).Negate().ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.list1.Subtract(this.list2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestSubtractFull()
        {
            double[] target = {.9, 1.8, 2.7, 3.6, -.5, -.6, -.7, -.8};

            FilterAssert.ListsAreReasonablyClose(target, this.list2.SubtractFull(this.list1).Negate().ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, this.list1.SubtractFull(this.list2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestSubtractS()
        {
            double[] target = {-1, 0, 1, 2};

            FilterAssert.ListsAreReasonablyClose(target, this.list1.Subtract(2d).ToReadOnlyList());
            FilterAssert.ListsAreReasonablyClose(target, 2d.Subtract(this.list1).Negate().ToReadOnlyList());
        }
    }
}