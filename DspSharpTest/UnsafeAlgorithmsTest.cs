// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsafeAlgorithmsTest.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public unsafe class UnsafeAlgorithmsTest
    {
        [TestMethod]
        public void TestUnsafeDoubleManaged()
        {
            double[] s = {1d, 2, 3, 4, 5};
            var t = new double[5];

            Unsafe.Memcpy(t, s);
            FilterAssert.ListsAreReasonablyClose(t, s);

            Unsafe.Memset(t);
            FilterAssert.ListsAreReasonablyClose(t, new[] {0d, 0, 0, 0, 0});

            Unsafe.Memcpy(t, s, 3);
            FilterAssert.ListsAreReasonablyClose(t, new[] {1d, 2, 3, 0, 0});

            Unsafe.Memset(s, 0, 2);
            FilterAssert.ListsAreReasonablyClose(s, new[] {0d, 0, 3, 4, 5});
        }

        [TestMethod]
        public void TestUnsafeDoubleUnManaged()
        {
            double[] s = {1d, 2, 3, 4, 5};
            double* ps = Unsafe.MallocD(5);
            Unsafe.Memcpy(ps, s);

            FilterAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray(ps, 5), s);

            Unsafe.Memset(ps, 0, 2);
            FilterAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray(ps, 5), new[] {0d, 0, 3, 4, 5});

            Unsafe.Memset(ps, 0, 5);
            Unsafe.Memcpy(ps, s, 2);
            FilterAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray(ps, 5), new[] {1d, 2, 0, 0, 0});

            double* pt = Unsafe.MallocD(5);
            Unsafe.Memcpy(ps, s);
            Unsafe.Memcpy(pt, ps, 2);
            Unsafe.Memcpy(pt + 3, ps + 2, 2);
            FilterAssert.ListsAreReasonablyClose(Unsafe.ToManagedArray(pt, 5), new[] {1d, 2, 0, 3, 4});

            Unsafe.Memcpy(s, pt);
            FilterAssert.ListsAreReasonablyClose(s, new[] {1d, 2, 0, 3, 4});

            Unsafe.Free(pt);
            Unsafe.Free(ps);
        }
    }
}