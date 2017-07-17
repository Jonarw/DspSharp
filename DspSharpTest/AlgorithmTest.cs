// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AlgorithmTest.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class AlgorithmTest
    {
        [TestMethod]
        public void TestCircularShift()
        {
            var x = new[] {1.0, 2, 3, 4, 5, 6, 7, 8};

            var output = x.CircularShift(0).ToReadOnlyList();
            DspAssert.ListsAreEqual(x, output);

            output = x.CircularShift(-2).ToReadOnlyList();
            DspAssert.ListsAreEqual(new[] {7, 8, 1.0, 2, 3, 4, 5, 6}, output);

            output = x.CircularShift(2).ToReadOnlyList();
            DspAssert.ListsAreEqual(new[] {3, 4, 5, 6, 7, 8, 1.0, 2}, output);

            Assert.IsTrue(new List<double>().CircularShift(2).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => VectorOperations.CircularShift<double>(null, 2).ToReadOnlyList());
        }
    }
}