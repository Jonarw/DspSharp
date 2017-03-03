using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    internal static class FilterAssert
    {
        internal static void ListContainsPlausibleValues(IReadOnlyList<double> list)
        {
            double prev = 0;
            var different = false;
            foreach (var d in list)
            {
                if (double.IsInfinity(d) || double.IsNaN(d))
                {
                    Assert.Fail("The list contains invalid values.");
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (d != prev)
                {
                    different = true;
                }
            }

            if (!different)
            {
                Assert.Fail();
            }
        }

        internal static void ListGreaterOrEqual(IEnumerable<double> list, double value)
        {
            foreach (var d in list)
            {
                Assert.IsTrue(d >= value);
            }
        }

        internal static void ListLessOrEqual(IEnumerable<double> list, double value)
        {
            foreach (var d in list)
            {
                Assert.IsTrue(d <= value);
            }
        }

        internal static void ListIsMonotonouslyRising(IEnumerable<double> list)
        {
            var last = double.NegativeInfinity;
            foreach (var d in list)
            {
                Assert.IsTrue(d >= last);
                last = d;
            }
        }

        internal static void ListIsMonotonouslyFalling(IEnumerable<double> list)
        {
            var last = double.PositiveInfinity;
            foreach (var d in list)
            {
                Assert.IsTrue(d <= last);
                last = d;
            }
        }

        internal static void ListContainsPlausibleValues(IReadOnlyList<Complex> list)
        {
            ListContainsPlausibleValues(list.Select(c => c.Magnitude).ToReadOnlyList());
        }

        internal static void ListContainsOnlyZeroes(IReadOnlyList<double> list)
        {
            foreach (var d in list)
            {
                Assert.IsTrue(d == 0.0);
            }
        }

        internal static void ListContainsOnlyZeroes(IReadOnlyList<Complex> list)
        {
            foreach (var d in list)
            {
                Assert.IsTrue(d == Complex.Zero);
            }
        }

        internal static void ListsAreEqual<T>(IReadOnlyList<T> list1, IReadOnlyList<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                Assert.Fail("The lists are different length.");
            }

            for (var i = 0; i < list1.Count; i++)
            {
                Assert.IsTrue(list1[i].Equals(list2[i]));
            }
        }

        internal static void ListsAreReasonablyClose(IReadOnlyList<double> list1, IReadOnlyList<double> list2,
            double threshold = 1e-14)
        {
            if (list1.Count != list2.Count)
            {
                Assert.Fail("The lists are different length.");
            }

            for (var i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i], list2[i], threshold);
            }
        }

        internal static void ListsAreReasonablyClose(IReadOnlyList<Complex> list1, IReadOnlyList<Complex> list2,
            double threshold = 1e-13)
        {
            if (list1.Count != list2.Count)
            {
                Assert.Fail("The lists are different length.");
            }

            for (var i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i].Imaginary, list2[i].Imaginary, threshold);
                Assert.AreEqual(list1[i].Real, list2[i].Real, threshold);
            }
        }
    }
}