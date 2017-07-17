// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestVectorArithmeticC.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public class TestVectorArithmeticC
    {
        private readonly IReadOnlyList<Complex> list1 = new[]
        {
            new Complex(1, -3),
            new Complex(2, -2),
            new Complex(3, -1),
            new Complex(4, 0)
        };

        private readonly IReadOnlyList<Complex> list2 = new[]
        {
            new Complex(.1, -.3),
            new Complex(.2, -.2),
            new Complex(.3, -.1),
            new Complex(.4, 0),
            new Complex(.5, .1),
            new Complex(.6, .2)
        };

        [TestMethod]
        public void TestAdd()
        {
            var target = new[]
            {
                new Complex(1.1, -3.3),
                new Complex(2.2, -2.2),
                new Complex(3.3, -1.1),
                new Complex(4.4, 0)
            };

            DspAssert.ListsAreReasonablyClose(target, this.list1.Add(this.list2).ToReadOnlyList());
            DspAssert.ListsAreReasonablyClose(target, this.list2.Add(this.list1).ToReadOnlyList());
        }

        [TestMethod]
        public void TestDivide()
        {
            var target = new[]
            {
                new Complex(10, 0),
                new Complex(10, 0),
                new Complex(10, 0),
                new Complex(10, 0),
            };

            var target2 = new[]
            {
                new Complex(.1, 0),
                new Complex(.1, 0),
                new Complex(.1, 0),
                new Complex(.1, 0),
            };

            var result = this.list1.Divide(this.list2).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target, result);
            var result2 = this.list2.Divide(this.list1).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target2, result2);
        }

        [TestMethod]
        public void TestMultiply()
        {
              var target = new[]
            {
                new Complex(-.8, -.6),
                new Complex(0, -.8),
                new Complex(.8, -.6),
                new Complex(1.6, 0),
            };

            var result = this.list1.Multiply(this.list2).ToReadOnlyList();
            var result2 = this.list2.Multiply(this.list1).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target, result);
            DspAssert.ListsAreReasonablyClose(target, result2);
        }

        [TestMethod]
        public void TestNegate()
        {
            var target = new[]
            {
                new Complex(-1, 3),
                new Complex(-2, 2),
                new Complex(-3, 1),
                new Complex(-4, 0)
            };

            DspAssert.ListsAreReasonablyClose(target, this.list1.Negate().ToReadOnlyList());
        }

        [TestMethod]
        public void TestSubtract()
        {
            var target = new[]
            {
                new Complex(.9, -2.7),
                new Complex(1.8, -1.8),
                new Complex(2.7, -0.9),
                new Complex(3.6, 0)
            };

            DspAssert.ListsAreReasonablyClose(target, this.list1.Subtract(this.list2).ToReadOnlyList());
            DspAssert.ListsAreReasonablyClose(target, this.list2.Subtract(this.list1).Negate().ToReadOnlyList());
        }
    }
}