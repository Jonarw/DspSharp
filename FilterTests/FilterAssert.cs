using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Extensions;
using NUnit.Framework;

namespace FilterTests
{
    internal static class FilterAssert
    {
        internal static void ListContainsPlausibleValues(IReadOnlyList<double> list)
        {
            double prev = 0;
            bool different = false;
            foreach (var d in list)
            {
                if ((d == double.NegativeInfinity) || (d == double.PositiveInfinity) || (d == double.NaN))
                {
                    Assert.Fail("The list contains invalid values.");
                }

                if (d != prev)
                {
                    different = true;
                }
            }

            if (!different)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        internal static void ListIsGreaterThan(IEnumerable<double> list, double value)
        {
            foreach (var d in list)
            {
                Assert.GreaterOrEqual(d, value);
            }
        }

        internal static void ListIsLessThan(IEnumerable<double> list, double value)
        {
            foreach (var d in list)
            {
                Assert.LessOrEqual(d, value);
            }
        }

        internal static void ListIsMonotonouslyRising(IEnumerable<double> list)
        {
            double last = double.NegativeInfinity;
            foreach (var d in list)
            {
                Assert.GreaterOrEqual(d, last);
                last = d;
            }
        }

        internal static void ListIsMonotonouslyFalling(IEnumerable<double> list)
        {
            double last = double.PositiveInfinity;
            foreach (var d in list)
            {
                Assert.LessOrEqual(d, last);
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
                Assert.That(d == 0.0);
            }
        }

        internal static void ListContainsOnlyZeroes(IReadOnlyList<Complex> list)
        {
            foreach (var d in list)
            {
                Assert.That(d == Complex.Zero);
            }
        }

        internal static void ListsAreEqual<T>(IReadOnlyList<T> list1, IReadOnlyList<T> list2)
        {
            if (list1.Count != list2.Count)
            {
                Assert.Fail("The lists are different length.");
            }

            for (int i = 0; i < list1.Count; i++)
            {
                Assert.That(list1[i].Equals(list2[i]));
            }
        }

        internal static void ListsAreReasonablyClose(IReadOnlyList<double> list1, IReadOnlyList<double> list2, double threshold = 1e-14)
        {
            if (list1.Count != list2.Count)
            {
                Assert.Fail("The lists are different length.");
            }

            for (int i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i], list2[i], threshold);
            }
        }

        internal static void ListsAreReasonablyClose(IReadOnlyList<Complex> list1, IReadOnlyList<Complex> list2, double threshold = 1e-13)
        {
            if (list1.Count != list2.Count) 
            {
                Assert.Fail("The lists are different length.");
            }

            for (int i = 0; i < list1.Count; i++)
            {
                Assert.AreEqual(list1[i].Imaginary, list2[i].Imaginary, threshold);
                Assert.AreEqual(list1[i].Real, list2[i].Real, threshold);
            }
        }

        internal static void ListMinimumGreaterThan(IReadOnlyList<double> list, double minimum)
        {
            foreach (var d in list)
            {
                Assert.Greater(d, minimum);
            }
        }
    }
}