using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class TestInterpolation
    {
        [TestMethod]
        public void TestAdaptiveInterpolation()
        {
            var x = new[] {0.0, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 3.0, 20.0};
            var y = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
            var x2 = new[] {0.0, 1.0, 2.0, 3, 4, 5, 6, 7, 8, 9, 10};

            var y2 = Interpolation.AdaptiveInterpolation(x, y, x2).ToReadOnlyList();

            Assert.IsTrue(y2.Count == x2.Length);
            Assert.IsTrue(y2[0] == y[0]);
            FilterAssert.ListIsMonotonouslyRising(y2);
            Assert.IsTrue(y2[y2.Count - 1] < 9.0);
            Assert.IsTrue(y2[y2.Count - 1] > 8.0);

            Assert.IsTrue(Interpolation.AdaptiveInterpolation(x, y, new List<double>()).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.AdaptiveInterpolation(x, y, null).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.AdaptiveInterpolation(x, (IReadOnlyList<double>)null, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.AdaptiveInterpolation(null, y, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(() => Interpolation.AdaptiveInterpolation(x2, y, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(
                () => Interpolation.AdaptiveInterpolation(new List<double>(), new List<double>(), x2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestAdaptiveInterpolationComplex()
        {
            var x = new[] {0.0, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 3.0, 20.0};
            var m = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
            double[] p =
            {
                0,
                Math.PI * 0.25,
                Math.PI * 0.5,
                Math.PI * 0.75,
                Math.PI,
                Math.PI * 1.25,
                Math.PI * 1.5,
                Math.PI * 1.75,
                Math.PI * 2,
                Math.PI * 2.25
            };

            var x2 = new[] {0.0, 1.0, 2.0, 3, 4, 5, 6, 7, 8, 9, 10};

            var y = m.Zip(p, Complex.FromPolarCoordinates).ToReadOnlyList();

            var y2 = Interpolation.AdaptiveInterpolation(x, y, x2).ToReadOnlyList();

            Assert.IsTrue(y2.Count == x2.Length);
            Assert.IsTrue(y2[0] == y[0]);
            FilterAssert.ListIsMonotonouslyRising(y2.Select(c => c.Magnitude));
            Assert.IsTrue(y2[y2.Count - 1].Magnitude < 9.0);
            Assert.IsTrue(y2[y2.Count - 1].Magnitude > 8.0);

            Assert.IsTrue(Interpolation.AdaptiveInterpolation(x, y, new List<double>()).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.AdaptiveInterpolation(x, y, null).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(
                () => Interpolation.AdaptiveInterpolation(x, (IReadOnlyList<Complex>)null, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.AdaptiveInterpolation(null, y, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(() => Interpolation.AdaptiveInterpolation(x2, y, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(
                () => Interpolation.AdaptiveInterpolation(new List<double>(), new List<Complex>(), x2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestInterpolateComplex()
        {
            double[] x = {1, 2, 3, 4};

            Complex[] y =
            {
                Complex.FromPolarCoordinates(1, 1),
                Complex.FromPolarCoordinates(2, 2),
                Complex.FromPolarCoordinates(3, 3),
                Complex.FromPolarCoordinates(4, 4)
            };

            double[] x2 = {0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5};

            var result = Interpolation.InterpolateComplex(x, y, x2).ToReadOnlyList();

            Assert.IsTrue(result.Count == x2.Length);
            FilterAssert.ListIsMonotonouslyRising(result.Select(c => c.Magnitude));
            Assert.IsTrue(result[0].Magnitude >= 0);
            Assert.IsTrue(result[result.Count - 1].Magnitude >= 4);
            Assert.IsTrue(result[0].Magnitude <= 1);
            Assert.IsTrue(result[result.Count - 1].Magnitude <= 5);

            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.InterpolateComplex(null, y, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.InterpolateComplex(x, null, x2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.InterpolateComplex(x, y, null).ToReadOnlyList());
            ThrowsAssert.Throws<Exception>(() => Interpolation.InterpolateComplex(x2, y, x2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestSmooth()
        {
            double[] x = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            double[] y = {1, 2, 3, 4, 5, 6, 7, 8, 9};

            var result = Interpolation.Smooth(x, y, 1).ToReadOnlyList();

            Assert.IsTrue(result.Count == x.Length);
            Assert.IsTrue(result[0] >= 1);
            Assert.IsTrue(result[result.Count - 1] <= 9);
            FilterAssert.ListIsMonotonouslyRising(result);

            FilterAssert.ListsAreReasonablyClose(Interpolation.Smooth(x, y, 0).ToReadOnlyList(), y);

            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.Smooth(null, y, 1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => Interpolation.Smooth(x, null, 1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Interpolation.Smooth(x, y, -1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(() => Interpolation.Smooth(new List<double> {1}, y, -1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(() => Interpolation.Smooth(new List<double>(), new List<double>(), -1).ToReadOnlyList());
        }
    }
}