// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComplexVectorTests.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public class ComplexVectorTests
    {
        private readonly Complex[] input =
        {
            new Complex(1, 2),
            new Complex(0, 1),
            new Complex(-1, 0),
            new Complex(2, -1)
        };

        [TestMethod]
        public void TestComplexConjugate()
        {
            var ret = this.input.ComplexConjugate().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(
                ret,
                new[]
                {
                    new Complex(1, -2),
                    new Complex(0, -1),
                    new Complex(-1, 0),
                    new Complex(2, 1)
                });
        }

        [TestMethod]
        public void TestImaginary()
        {
            var ret = this.input.Imaginary().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {2d, 1, 0, -1});
        }

        [TestMethod]
        public void TestInterleave()
        {
            var ret = this.input.Interleave().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {1, 2, 0, 1, -1, 0, 2, -1d});
            DspAssert.ListsAreReasonablyClose(ret.UnInterleaveComplex().ToReadOnlyList(), this.input);
        }

        [TestMethod]
        public void TestMagnitude()
        {
            var ret = this.input.Magitude().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {Math.Sqrt(5), 1, 1, Math.Sqrt(5)});
        }

        [TestMethod]
        public void TestPhase()
        {
            var ret = this.input.Phase().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {Math.Atan2(2, 1), Math.PI / 2, Math.PI, Math.Atan2(-1, 2)});
        }

        [TestMethod]
        public void TestReal()
        {
            var ret = this.input.Real().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {1d, 0, -1, 2});
        }

        [TestMethod]
        public void TestToComplex()
        {
            var ret = this.input.Real().ToComplex().ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(ret, new[] {Complex.One, 0, -1, 2});
        }
    }
}