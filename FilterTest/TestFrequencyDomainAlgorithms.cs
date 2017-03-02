using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace FilterTest
{
    [TestClass]
    public class TestFrequencyDomainAlgorithms
    {
        [TestMethod]
        public void TestApplyDelayToSpectrum()
        {
            var x = new[] {250, 500.0, 1000, 2000};
            var y = new[] {Complex.One, Complex.One, Complex.One, Complex.One};
            Complex[] target = {Complex.ImaginaryOne, -Complex.One, Complex.One, Complex.One};

            var output = FrequencyDomain.ApplyDelayToSpectrum(x, y, 0.001).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(target, output);

            var y2 = new[] {4.0 * Complex.One, 5, 6, 7};
            output = FrequencyDomain.ApplyDelayToSpectrum(x, y2, 2).ToReadOnlyList();
            Assert.IsTrue(output.Count == x.Length);

            Assert.IsTrue(FrequencyDomain.ApplyDelayToSpectrum(new List<double>(), y, 1).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.ApplyDelayToSpectrum(null, y, 2).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.ApplyDelayToSpectrum(x, null, 2).ToReadOnlyList());
        }

        [TestMethod]
        public void TestCalculateGroupDelay()
        {
            double[] phase =
            {
                221.651357700633e-003,
                236.404025345054e-003,
                251.454348329399e-003,
                265.834661679128e-003,
                277.712938087876e-003,
                283.814953936594e-003,
                278.669273726763e-003,
                254.100812809699e-003,
                200.484690747697e-003,
                112.629294062348e-003,
                967.819685380718e-015,
                -111.610456260826e-003,
                -197.701817968959e-003,
                -250.729276954184e-003,
                -276.409200554505e-003,
                -283.872819775752e-003,
                -280.637306255616e-003,
                -271.705281042254e-003,
                -260.095771586513e-003,
                -247.532299129978e-003,
                -234.956640702564e-003
            };

            double[] frequencies =
            {
                900,
                910,
                920,
                930,
                940,
                950,
                960,
                970,
                980,
                990,
                1000,
                1010,
                1020,
                1030,
                1040,
                1050,
                1060,
                1070,
                1080,
                1090,
                1100
            };

            double[] target =
            {
                -229.465918533212e-006,
                -238.926661113766e-006,
                -237.668739549207e-006,
                -215.364054404027e-006,
                -154.259961127109e-006,
                -25.6450909504240e-006,
                211.328302694609e-006,
                597.714355880440e-006,
                1.12595107555368e-003,
                1.64674321357226e-003,
                1.86404323386552e-003,
                1.61855652428749e-003,
                1.10370485752431e-003,
                602.389383386658e-006,
                240.470529725761e-006,
                17.1697415512031e-006,
                -107.072059136385e-006,
                -169.426100432009e-006,
                -195.679681710948e-006,
                -201.814527319348e-006,
                -197.237014407641e-006
            };

            var result = FrequencyDomain.CalculateGroupDelay(frequencies, phase).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result, 1e-4);

            var p2 = new[] {4.0, 5, 6, 7};
            result = FrequencyDomain.CalculateGroupDelay(frequencies, p2).ToReadOnlyList();
            Assert.IsTrue(result.Count == p2.Length);

            Assert.IsTrue(FrequencyDomain.CalculateGroupDelay(new List<double>(), phase).ToReadOnlyList().Count == 0);
            Assert.IsTrue(FrequencyDomain.CalculateGroupDelay(frequencies, new List<double>()).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.CalculateGroupDelay(frequencies, null).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.CalculateGroupDelay(null, phase).ToReadOnlyList());
        }

        [TestMethod]
        public void TestDbToLinear()
        {
            var input = new[] {0, 20, 6.020599913279624};
            var target = new[] {1.0, 10, 2};

            var result = FrequencyDomain.DbToLinear(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.IsTrue(FrequencyDomain.DbToLinear(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.DbToLinear(null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestDegToRad()
        {
            var input = new[] {0.0, -90, 180, 270};
            var target = new[] {0, -Math.PI * 0.5, Math.PI, 1.5 * Math.PI};

            var result = FrequencyDomain.DegToRad(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.IsTrue(FrequencyDomain.DegToRad(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.DegToRad(null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestIirFrequencyResponse()
        {
            var a = new[]
            {
                1.005324035839730,
                -1.979734945559880,
                0.994675964160273
            };

            var b = new[]
            {
                1.009467623312100,
                -1.979734945559880,
                0.990532376687905
            };

            double[] frequencies =
            {
                20,
                500,
                1000,
                1500,
                20000
            };

            Complex[] target =
            {
                new Complex(1.00000174610533e+000, -1.16574212797254e-003),
                new Complex(1.00193212578060e+000, -38.7298412403863e-003),
                new Complex(1.77827941003874e+000, -1.72105381914270e-012),
                new Complex(1.00618373847305e+000, 69.0973060921694e-003),
                new Complex(1.00000048223708e+000, 612.629540369582e-006)
            };

            var result = FrequencyDomain.IirFrequencyResponse(a, b, frequencies, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(result, target);

            Assert.IsTrue(FrequencyDomain.IirFrequencyResponse(a, b, Enumerable.Empty<double>().ToReadOnlyList(), 44100).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.IirFrequencyResponse(null, b, frequencies, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.IirFrequencyResponse(a, null, frequencies, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.IirFrequencyResponse(a, b, null, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => FrequencyDomain.IirFrequencyResponse(a, b, frequencies, -1).ToReadOnlyList());

            ThrowsAssert.Throws<Exception>(
                () => FrequencyDomain.IirFrequencyResponse(Enumerable.Empty<double>().ToReadOnlyList(), b, frequencies, 44100).ToReadOnlyList());
            ThrowsAssert.Throws<Exception>(() => FrequencyDomain.IirFrequencyResponse(new[] {0.0}, b, frequencies, 44100).ToReadOnlyList());
        }

        [TestMethod]
        public void TestLinearToDb()
        {
            var input = new[] {1.0, 10, 2, 0};
            var target = new[] {0, 20, 6.020599913279624, -100};

            var result = FrequencyDomain.LinearToDb(input, -100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.IsTrue(FrequencyDomain.LinearToDb(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.LinearToDb(null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestPolarToComplex()
        {
            double[] amp = {1, 2, 3, 4, 5};
            double[] phase = {0, Math.PI * 0.5, Math.PI, Math.PI * 1.5, 2 * Math.PI};
            Complex[] target =
            {
                new Complex(1, 0),
                new Complex(0, 2),
                new Complex(-3, 0),
                new Complex(0, -4),
                new Complex(5, 0)
            };

            var result = FrequencyDomain.PolarToComplex(amp, phase).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(result, target);

            Assert.IsTrue(FrequencyDomain.PolarToComplex(Enumerable.Empty<double>(), phase).ToReadOnlyList().Count == 0);
            Assert.IsTrue(FrequencyDomain.PolarToComplex(amp, Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);

            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.PolarToComplex(null, phase));
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.PolarToComplex(amp, null));
        }

        [TestMethod]
        public void TestRadToDeg()
        {
            var target = new[] {0.0, -90, 180, 270};
            var input = new[] {0, -Math.PI * 0.5, Math.PI, 1.5 * Math.PI};

            var result = FrequencyDomain.RadToDeg(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            Assert.IsTrue(FrequencyDomain.RadToDeg(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.RadToDeg(null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestUnwrapPhase()
        {
            double[] input = {170, 179, -175, 0, 170, -90, 170, 10, -150, 150, 0};
            double[] target = {170, 179, 185, 360, 530, 630, 530, 370, 210, 150, 0};

            var input2 = FrequencyDomain.DegToRad(input).ToReadOnlyList();

            var result = FrequencyDomain.UnwrapPhase(input, true).ToReadOnlyList();
            var result2 = FrequencyDomain.UnwrapPhase(input2, false).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(result, target);
            FilterAssert.ListsAreReasonablyClose(FrequencyDomain.RadToDeg(result2).ToReadOnlyList(), target, 1e-13);

            Assert.IsTrue(FrequencyDomain.UnwrapPhase(new List<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.UnwrapPhase(null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestWrapPhase()
        {
            double[] input = {170, 179, 185, 360, 530, 630, 530, 370, 210, 150, 0};
            double[] target = {170, 179, -175, 0, 170, -90, 170, 10, -150, 150, 0};

            var input2 = FrequencyDomain.DegToRad(input).ToReadOnlyList();

            var result = FrequencyDomain.WrapPhase(input, true).ToReadOnlyList();
            var result2 = FrequencyDomain.WrapPhase(input2, false).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(result, target);
            FilterAssert.ListsAreReasonablyClose(FrequencyDomain.RadToDeg(result2).ToReadOnlyList(), target, 1e-13);

            Assert.IsTrue(FrequencyDomain.UnwrapPhase(new List<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => FrequencyDomain.UnwrapPhase(null).ToReadOnlyList());
        }
    }
}