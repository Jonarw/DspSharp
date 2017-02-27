//using System;
//using Filter.CircularBuffers;
//using Filter.Extensions;
//using NUnit.Framework;

//namespace FilterTests
//{
//    [TestFixture]
//    public class CircularBuffersTest
//    {
//        [Test]
//        public void TestCircularArray()
//        {
//            double[] init = {.1, .2, .3, .4, .5, .6, .7, .8};

//            var test = new CircularArray<double>(init);
//            Assert.That(test.Length == 8);

//            Assert.AreEqual(test.GetValue(), .1);
//            Assert.AreEqual(test.GetValue(), .2);
//            Assert.AreEqual(test.GetValue(), .3);
//            FilterAssert.ListsAreReasonablyClose(test.GetRange(10), new[] {.4, .5, .6, .7, .8, .1, .2, .3, .4, .5});
//            FilterAssert.ListsAreReasonablyClose(test.GetRange(2), new[] {.6, .7});
//            Assert.That(test.Position == 8);
//        }

//        [Test]
//        public void TestCircularBuffer()
//        {
//            double[] init = {.1, .2, .3, .4, .5, .6, .7, .8};
//            double[] input1 = {1, 2, 3, 4, 5};
//            double[] input2 = {1, 2, 3, 4, 5, 6, 7, 8, 9};

//            var test = new CircularBuffer<double>(8);
//            Assert.That(test.Length == 8);

//            test.Store(init);

//            Assert.AreEqual(test.Peek(0), .1);
//            Assert.AreEqual(test.Peek(2), .7);

//            FilterAssert.ListsAreReasonablyClose(test.PeekRange(3), new[] {.5d, .6, .7});
//            FilterAssert.ListsAreReasonablyClose(test.StoreAndRetrieve(input1), new[] {.1d, .2, .3, .4, .5});
//            FilterAssert.ListsAreReasonablyClose(test.StoreAndRetrieve(input2), new[] {.6d, .7, .8, 1, 2, 3, 4, 5, 1, 2});
//        }

//        [Test]
//        public unsafe void TestCircularBlockBuffer()
//        {
//            double[] init = { .1, .2, .3, .4, .5, .6, .7, .8 };
//            var ret = new double[3];

//            var test = new CircularBlockBuffer(init, 3);
//            fixed (double* pRet = ret)
//            {
//                test.GetBlock(pRet);
//            }

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .1, .2, .3 });

//            fixed (double* pRet = ret)
//            {
//                test.GetBlock(pRet);
//            }

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .4, .5, .6 });

//            fixed (double* pRet = ret)
//            {
//                test.GetBlock(pRet);
//            }

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .7, .8, .1 });
//        }

//        [Test]
//        public unsafe void TestDoubleBlockBuffer()
//        {
//            double[] init = { .1, .2, .3};
//            double[] a1 = { .4, .5, .6};
//            double[] a2 = { .7, .8, .9};
//            double[] a3 = { .11, .12, .13};
//            double[] a4 = { .14, .15, .16};
//            double[] a5 = { .17, .18, .19};
//            var ret = new double[3];

//            var test = new DoubleBlockBuffer(8, 3);

//            test.BufferSwitch += this.TestOnBufferSwitch1;

//            fixed (double* pRet = a1)
//            {
//                test.InputBlock((byte*)pRet);
//            }
//            fixed (double* pRet = a2)
//            {
//                test.InputBlock((byte*)pRet);
//            }
//            fixed (double* pRet = a3)
//            {
//                test.InputBlock((byte*)pRet);
//            }

//            test.BufferSwitch -= this.TestOnBufferSwitch1;
//            test.BufferSwitch += this.TestOnBufferSwitch2;

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .1, .2, .3 });

//            fixed (double* pRet = ret)
//            {
//                test.GetBlock(pRet);
//            }

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .4, .5, .6 });

//            fixed (double* pRet = ret)
//            {
//                test.GetBlock(pRet);
//            }

//            FilterAssert.ListsAreReasonablyClose(ret, new[] { .7, .8, .1 });
//        }

//        private unsafe void TestOnBufferSwitch1(DoubleBlockBuffer sender, byte* buffer)
//        {
//            FilterAssert.ListsAreReasonablyClose(UnsafeConversions.ToManagedArray((double*)buffer, 8), new[] { .1, .2, .3, .4, .5, .6, .7, .8 });
//        }

//        private unsafe void TestOnBufferSwitch2(DoubleBlockBuffer sender, byte* buffer)
//        {
//            FilterAssert.ListsAreReasonablyClose(UnsafeConversions.ToManagedArray((double*)buffer, 8), new[] { .9, .10, .11, .12, .13, .14, .15, .16 });
//        }
//    }
//}