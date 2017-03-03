using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class TestSignalGenerators
    {
        [TestMethod]
        public void TestAlignedLogSweep()
        {
            var sweep = SignalGenerators.AlignedLogSweep(20, 2000, .1, SignalGenerators.SweepAlignments.NegativeOne, 44100).ToReadOnlyList();

            Assert.IsTrue(sweep.Count == 4410);
            Assert.IsTrue(sweep[0] == 0);
            FilterAssert.ListContainsPlausibleValues(sweep);

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(
                () => SignalGenerators.AlignedLogSweep(-1, 2000, .1, SignalGenerators.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(
                () => SignalGenerators.AlignedLogSweep(20, -1, .1, SignalGenerators.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(
                () => SignalGenerators.AlignedLogSweep(20, 2000, -1, SignalGenerators.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(
                () => SignalGenerators.AlignedLogSweep(20, 2000, .1, SignalGenerators.SweepAlignments.NegativeOne, -1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(
                () => SignalGenerators.AlignedLogSweep(20, 20, .1, SignalGenerators.SweepAlignments.NegativeOne, -1).ToReadOnlyList());
        }

        [TestMethod]
        public void TestGenerateSlope()
        {
            var x = new[] {1.0, 2, 3, 4, 5, 6, 7, 8};

            var output = SignalGenerators.Slope(x, 2, 7, -10, 20).ToReadOnlyList();

            Assert.IsTrue(output.Count == x.Length);
            Assert.IsTrue(output[0] == -10);
            Assert.IsTrue(output[output.Count - 1] == 20);
            FilterAssert.ListIsMonotonouslyRising(output);

            var output2 = SignalGenerators.Slope(x, 7, 2, 20, -10).ToReadOnlyList();
            FilterAssert.ListsAreEqual(output, output2);

            Assert.IsTrue(SignalGenerators.Slope(new List<double>(), 2, 7, 10, 20).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => SignalGenerators.Slope(null, 2, 7, 10, 20).ToReadOnlyList());
        }

        [TestMethod]
        public void TestHalfSinc()
        {
            var target = new[]
            {
                1.000000000000000,
                0.996620203891810,
                0.986521922547816,
                0.969827881788617,
                0.946740650292526,
                0.917539710739792,
                0.882577424213710,
                0.842273951367101,
                0.797111210188940,
                0.747625965210988,
                0.694402156431365
            };

            var result = SignalGenerators.HalfSinc(1000, 44100);
            FilterAssert.ListsAreReasonablyClose(target, result.Take(11).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.HalfSinc(-1, 44100).Take(1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.HalfSinc(1000, -1).Take(1).ToReadOnlyList());
        }

        [TestMethod]
        public void TestLinSeries()
        {
            double[] target = {1, 2, 3, 4, 5};
            var result = SignalGenerators.LinSeries(1, 5, 5).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LinSeries(1, 5, -1));
        }

        [TestMethod]
        public void TestLogSeries()
        {
            double[] target = {1, 2, 4, 8, 16};
            var result = SignalGenerators.LogSeries(1, 16, 5).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSeries(1, 16, 1));
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSeries(-1, 16, 5));
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSeries(1, -1, 5));
        }

        [TestMethod]
        public void TestLogSweep()
        {
            double[] target =
            {
                0,
                0.66258621556149377,
                0.99520589035076168,
                0.78024867880980453,
                0.095263614110726466,
                -0.66767505170041352,
                -0.99989473380398375,
                -0.623644065661702,
                0.23685049283432291,
                0.92546586035440737,
                0.83301701674256923,
                -0.0229613721483027,
                -0.87443748695979051,
                -0.84204415342236749,
                0.11108116990225904,
                0.95239941352460167,
                0.6383075415659536,
                -0.5105817844711058,
                -0.97264500776464125,
                -0.024385189497044598,
                0.97153101197467506
            };

            var result = SignalGenerators.LogSweep(5000, 10000, 21.0 / 44100, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            result = SignalGenerators.LogSweep(10000, 5000, 21.0 / 44100, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target.Reverse().ToReadOnlyList(), result);

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSweep(-1, 10000, .1, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSweep(5000, -1, .1, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSweep(5000, 10000, -1, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.LogSweep(5000, 10000, .1, -1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentException>(() => SignalGenerators.LogSweep(20, 20, .1, 44100).ToReadOnlyList());
        }

        [TestMethod]
        public void TestWhiteNoise()
        {
            var noise = SignalGenerators.WhiteNoise().Take(1000000).ToReadOnlyList();
            var noise1 = SignalGenerators.WhiteNoise().Take(1).ToReadOnlyList();

            Assert.IsTrue(noise1[0] != noise[0]);
            Assert.AreEqual(noise.Average(), 0, 1e-2);
            Assert.AreEqual(noise.Variance(), 1, 1e-2);
        }

        [TestMethod]
        public void TestWindowedSinc()
        {
            double[] target =
            {
                0.104585930423594,
                0.019945236742838,
                -0.096769267570006,
                -0.193040073125182,
                -0.211861318358862,
                -0.114556337964799,
                0.101048856819576,
                0.394859675978971,
                0.694402156431365,
                0.917539710739792,
                1.000000000000000,
                0.917539710739792,
                0.694402156431365,
                0.394859675978971,
                0.101048856819576,
                -0.114556337964799,
                -0.211861318358862,
                -0.193040073125182,
                -0.096769267570006,
                0.019945236742838,
                0.104585930423594
            };

            var result = SignalGenerators.WindowedSinc(5000, 44100, 21, -10);
            FilterAssert.ListsAreReasonablyClose(result.ToReadOnlyList(), target);

            Assert.IsTrue(SignalGenerators.WindowedSinc(5000, 44100, 0, -10).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.WindowedSinc(-1, 44100, 21, -10).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.WindowedSinc(5000, -1, 21, -10).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.WindowedSinc(5000, 44100, -1, -10).ToReadOnlyList());
        }
    }
}