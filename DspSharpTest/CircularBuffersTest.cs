// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffersTest.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using DspSharp.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public class CircularBuffersTest
    {
        [TestMethod]
        public void TestCircularArray()
        {
            double[] init = {.1, .2, .3, .4, .5, .6, .7, .8};

            var test = new CircularArray<double>(init);
            Assert.IsTrue(test.Length == 8);

            Assert.AreEqual(test.GetValue(), .1);
            Assert.AreEqual(test.GetValue(), .2);
            Assert.AreEqual(test.GetValue(), .3);
            DspAssert.ListsAreReasonablyClose(test.GetRange(10), new[] {.4, .5, .6, .7, .8, .1, .2, .3, .4, .5});
            DspAssert.ListsAreReasonablyClose(test.GetRange(2), new[] {.6, .7});
            Assert.IsTrue(test.Position == 7);
        }

        [TestMethod]
        public unsafe void TestCircularBlockBuffer()
        {
            double[] init = {.1, .2, .3, .4, .5, .6, .7, .8};
            var ret = new double[3];

            var test = new CircularBlockBuffer(init);
            fixed (double* pRet = ret)
            {
                test.GetBlock((byte*)pRet, 3);
            }

            DspAssert.ListsAreReasonablyClose(ret, new[] {.1, .2, .3});

            fixed (double* pRet = ret)
            {
                test.GetBlock((byte*)pRet, 3);
            }

            DspAssert.ListsAreReasonablyClose(ret, new[] {.4, .5, .6});

            fixed (double* pRet = ret)
            {
                test.GetBlock((byte*)pRet, 3);
            }

            DspAssert.ListsAreReasonablyClose(ret, new[] {.7, .8, .1});
        }

        [TestMethod]
        public void TestCircularBuffer()
        {
            double[] init = {.1, .2, .3, .4, .5, .6, .7, .8};
            double[] input1 = {1, 2, 3, 4, 5};
            double[] input2 = {1, 2, 3, 4, 5, 6, 7, 8, 9};

            var test = new CircularBuffer<double>(8);
            Assert.IsTrue(test.Length == 8);

            test.Store(init);

            Assert.AreEqual(test.Peek(0), .1);
            Assert.AreEqual(test.Peek(-2), .7);
            Assert.AreEqual(test.Peek(2), .3);

            DspAssert.ListsAreReasonablyClose(test.StoreAndRetrieve(input1), new[] {.1d, .2, .3, .4, .5});
            DspAssert.ListsAreReasonablyClose(test.StoreAndRetrieve(input2), new[] {.6d, .7, .8, 1, 2, 3, 4, 5, 1});
        }

        [TestMethod]
        public unsafe void TestDoubleBlockBuffer()
        {
            double[] a1 = {.1, .2, .3};
            double[] a2 = {.4, .5, .6};
            double[] a3 = {.7, .8, .9};
            double[] a4 = {.10, .11, .12};
            double[] a5 = {.13, .14, .15};
            double[] a6 = {.16, .17, .18};

            var test = new DoubleBuffer(8 * sizeof(double));

            test.BufferSwitch += this.TestOnBufferSwitch1;

            fixed (double* pRet = a1)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }
            fixed (double* pRet = a2)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }
            fixed (double* pRet = a3)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }

            test.BufferSwitch -= this.TestOnBufferSwitch1;
            test.BufferSwitch += this.TestOnBufferSwitch2;

            fixed (double* pRet = a4)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }
            fixed (double* pRet = a5)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }
            fixed (double* pRet = a6)
            {
                test.InputBlock((byte*)pRet, 3 * sizeof(double));
            }
        }

        private unsafe void TestOnBufferSwitch1(object sender, BufferSwitchEventArgs bufferSwitchEventArgs)
        {
            var buffer = bufferSwitchEventArgs.NewWorkBuffer;
            DspAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray((double*)buffer, 8), new[] {.1, .2, .3, .4, .5, .6, .7, .8});
        }

        private unsafe void TestOnBufferSwitch2(object sender, BufferSwitchEventArgs bufferSwitchEventArgs)
        {
            var buffer = bufferSwitchEventArgs.NewWorkBuffer;
            DspAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray((double*)buffer, 8), new[] {.9, .10, .11, .12, .13, .14, .15, .16});
        }
    }
}