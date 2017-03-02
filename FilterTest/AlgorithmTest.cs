using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Algorithms.FftwProvider;
using Filter.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace FilterTest
{
    [TestClass]
    public class AlgorithmTest
    {


        [TestMethod]
        public void TestCircularShift()
        {
            var x = new[] {1.0, 2, 3, 4, 5, 6, 7, 8};

            var output = VectorOperations.CircularShift(x, 0).ToReadOnlyList();
            FilterAssert.ListsAreEqual(x, output);

            output = VectorOperations.CircularShift(x, -2).ToReadOnlyList();
            FilterAssert.ListsAreEqual(new[] {7, 8, 1.0, 2, 3, 4, 5, 6}, output);

            output = VectorOperations.CircularShift(x, 2).ToReadOnlyList();
            FilterAssert.ListsAreEqual(new[] {3, 4, 5, 6, 7, 8, 1.0, 2}, output);

            Assert.IsTrue(VectorOperations.CircularShift(new List<double>(), 2).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => VectorOperations.CircularShift<double>(null, 2).ToReadOnlyList());
        }












    }
}