using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Remoting;
using Filter.Algorithms;
using Filter.Extensions;
using Filter_Win;
using NUnit.Framework;

namespace FilterTests
{
    [TestFixture]
    public class AlgorithmTest
    {
        [Test]
        public void TestAdaptiveInterpolation()
        {
            var x = new[] {0.0, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 3.0, 20.0};
            var y = new[] {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0};
            var x2 = new[] {0.0, 1.0, 2.0, 3, 4, 5, 6, 7, 8, 9, 10};

            var y2 = Dsp.AdaptiveInterpolation(x, y, x2).ToReadOnlyList();
                
            Assert.That(y2.Count == x2.Length);
            Assert.That(y2[0] == y[0]);
            FilterAssert.ListIsMonotonouslyRising(y2);
            Assert.Less(y2[y2.Count - 1], 9.0);
            Assert.Greater(y2[y2.Count - 1], 8.0);

            Assert.That(Dsp.AdaptiveInterpolation(x, y, new List<double>()).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(x, y, null).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(x, (IReadOnlyList<double>)null, x2).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(null, y, x2).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.AdaptiveInterpolation(x2, y, x2).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.AdaptiveInterpolation(new List<double>(), new List<double>(), x2).ToReadOnlyList());
        }

        [Test]
        public void TestAdaptiveInterpolationComplex()
        {
            var x = new[] { 0.0, 1.0, 1.1, 1.2, 1.3, 1.4, 1.5, 3.0, 20.0 };
            var m = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0 };
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

            var x2 = new[] { 0.0, 1.0, 2.0, 3, 4, 5, 6, 7, 8, 9, 10 };

            var y = m.Zip(p, Complex.FromPolarCoordinates).ToReadOnlyList();

            var y2 = Dsp.AdaptiveInterpolation(x, y, x2).ToReadOnlyList();

            Assert.That(y2.Count == x2.Length);
            Assert.That(y2[0] == y[0]);
            FilterAssert.ListIsMonotonouslyRising(y2.Select(c => c.Magnitude));
            Assert.Less(y2[y2.Count - 1].Magnitude, 9.0);
            Assert.Greater(y2[y2.Count - 1].Magnitude, 8.0);

            Assert.That(Dsp.AdaptiveInterpolation(x, y, new List<double>()).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(x, y, null).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(x, (IReadOnlyList<Complex>)null, x2).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.AdaptiveInterpolation(null, y, x2).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.AdaptiveInterpolation(x2, y, x2).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.AdaptiveInterpolation(new List<double>(), new List<Complex>(), x2).ToReadOnlyList());
        }


        [Test]
        public void TestAlignedLogSweep()
        {
            var sweep = Dsp.AlignedLogSweep(20, 2000, .1, Dsp.SweepAlignments.NegativeOne, 44100).ToReadOnlyList();

            Assert.That(sweep.Count == 4410);
            Assert.That(sweep[0] == 0);
            FilterAssert.ListContainsPlausibleValues(sweep);

            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.AlignedLogSweep(-1, 2000, .1, Dsp.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.AlignedLogSweep(20, -1, .1, Dsp.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.AlignedLogSweep(20, 2000, -1, Dsp.SweepAlignments.NegativeOne, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.AlignedLogSweep(20, 2000, .1, Dsp.SweepAlignments.NegativeOne, -1).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.AlignedLogSweep(20, 20, .1, Dsp.SweepAlignments.NegativeOne, -1).ToReadOnlyList());
        }

        [Test]
        public void TestAmplitudeToComplex()
        {
            var input = new[] {1.0, 2, 3};
            var output = Dsp.AmplitudeToComplex(input).ToReadOnlyList();
            Complex[] target = {Complex.One, 2, 3};
            FilterAssert.ListsAreEqual(output, target);
            
            Assert.That(Dsp.AmplitudeToComplex(new List<double>()).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.AmplitudeToComplex(null));
        }

        [Test]
        public void TestApplyDelayToSpectrum()
        {
            var x = new[] { 250, 500.0, 1000, 2000 };
            var y = new[] { Complex.One, Complex.One, Complex.One, Complex.One };
            Complex[] target = {Complex.ImaginaryOne, -Complex.One, Complex.One, Complex.One};

            var output = Dsp.ApplyDelayToSpectrum(x, y, 0.001).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(target, output);

            var y2 = new[] { 4.0 * Complex.One, 5, 6, 7};
            output = Dsp.ApplyDelayToSpectrum(x, y2, 2).ToReadOnlyList();
            Assert.That(output.Count == x.Length);

            Assert.That(Dsp.ApplyDelayToSpectrum(new List<double>(), y, 1).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.ApplyDelayToSpectrum(null, y, 2).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.ApplyDelayToSpectrum(x, null, 2).ToReadOnlyList());
        }

        [Test]
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

            var result = Dsp.CalculateGroupDelay(frequencies, phase).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result, 1e-4);

            var p2 = new[] { 4.0, 5, 6, 7 };
            result = Dsp.CalculateGroupDelay(frequencies, p2).ToReadOnlyList();
            Assert.That(result.Count == p2.Length);

            Assert.That(Dsp.CalculateGroupDelay(new List<double>(), phase).ToReadOnlyList().Count == 0);
            Assert.That(Dsp.CalculateGroupDelay(frequencies, new List<double>()).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.CalculateGroupDelay(frequencies, null).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.CalculateGroupDelay(null, phase).ToReadOnlyList());
        }

        [Test]
        public void TestGenerateSlope()
        {
            var x = new[] { 1.0, 2, 3, 4, 5, 6, 7, 8 };

            var output = Dsp.GenerateSlope(x, 2, 7, -10, 20).ToReadOnlyList();

            Assert.That(output.Count == x.Length);
            Assert.That(output[0] == -10);
            Assert.That(output[output.Count - 1] == 20);
            FilterAssert.ListIsMonotonouslyRising(output);

            var output2 = Dsp.GenerateSlope(x, 7, 2, 20, -10).ToReadOnlyList();
            FilterAssert.ListsAreEqual(output, output2);

            Assert.That(Dsp.GenerateSlope(new List<double>(), 2, 7, 10, 20).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.GenerateSlope(null, 2, 7, 10, 20).ToReadOnlyList());
        }

        [Test]
        public void TestCircularShift()
        {
            var x = new[] { 1.0, 2, 3, 4, 5, 6, 7, 8 };

            var output = Dsp.CircularShift(x, 0).ToReadOnlyList();
            FilterAssert.ListsAreEqual(x, output);

            output = Dsp.CircularShift(x, -2).ToReadOnlyList();
            FilterAssert.ListsAreEqual(new[] { 7, 8, 1.0, 2, 3, 4, 5, 6 }, output);

            output = Dsp.CircularShift(x, 2).ToReadOnlyList();
            FilterAssert.ListsAreEqual(new[] { 3, 4, 5, 6, 7, 8, 1.0, 2 }, output);

            Assert.That(Dsp.CircularShift(new List<double>(), 2).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.CircularShift<double>(null, 2).ToReadOnlyList());
        }

        [Test]
        public void TestConvertTimeToSamples()
        {
            bool integer;

            var result = Dsp.ConvertTimeToSamples(1, 44100, out integer);
            Assert.That(integer);
            Assert.That(result == 44100);

            result = Dsp.ConvertTimeToSamples(44100.6 / 44100, 44100, out integer);
            Assert.That(!integer);
            Assert.That(result == 44101);

            result = Dsp.ConvertTimeToSamples(-1, 44100, out integer);
            Assert.That(integer);
            Assert.That(result == -44100);

            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.ConvertTimeToSamples(1, -1, out integer));
        }

        [Test]
        public void TestConvolve()
        {
            Fft.FftProvider = new FftwProvider();

            var x = new[]
            {
                0.706046088019609,
                0.031832846377421,
                0.276922984960890,
                0.046171390631154,
                0.097131781235848,
                0.823457828327293,
                0.694828622975817,
                0.317099480060861,
                0.950222048838355,
                0.034446080502909
            };

            var y = new[]
            {
                0.438744359656398,
                0.381558457093008,
                0.765516788149002,
                0.795199901137063,
                0.186872604554379,
                0.489764395788231,
                0.445586200710899,
                0.646313010111265,
                0.709364830858073,
                0.754686681982361
            };

            var target = new[]
            {
                0.309773738776068,
                0.283364337781218,
                0.674134623045390,
                0.711736101800065,
                0.429476447439455,
                1.005647957710862,
                1.112065240300617,
                1.726619556339416,
                2.410188791663434,
                2.129381251142283,
                1.819441363049064,
                1.852986487123710,
                1.305830381386788,
                1.719629972594990,
                1.759561460666554,
                1.378806697858031,
                0.935627807393663,
                0.741554763253968,
                0.025995998202038
            };

            var result = Dsp.Convolve(x, y);

            FilterAssert.ListsAreReasonablyClose(target, result);

            Assert.That(Dsp.Convolve(new List<double>(), y).Count == 0);
            Assert.That(Dsp.Convolve(x, new List<double>()).Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.Convolve(null, y));
            Assert.Throws<ArgumentNullException>(() => Dsp.Convolve(x, null));
        }

        [Test]
        public void TestConvolve2()
        {
            Fft.FftProvider = new FftwProvider();

            var x = new[]
            {
                0.276025076998578,
                0.679702676853675,
                0.655098003973841,
                0.162611735194631
            };

            var x2 = new[]
            {
                0.257508254123736,
                0.840717255983663,
                0.254282178971531,
                0.814284826068816,
                0.243524968724989,
                0.929263623187228,
                0.349983765984809,
                0.196595250431208,
                0.251083857976031
            };

            IEnumerable<double> y = new[]
            {
                0.118997681558377,
                0.498364051982143,
                0.959743958516081,
                0.340385726666133,
                0.585267750979777,
                0.223811939491137,
                0.751267059305653,
                0.255095115459269,
                0.505957051665142,
                0.699076722656686,
                0.890903252535799,
                0.959291425205444,
                0.547215529963803,
                0.138624442828679,
                0.149294005559057
            };

            var target = new[]
            {

                0.032846344214803,
                0.218444018516304,
                0.681607923894635,
                1.092123249297922,
                1.102675860408013,
                0.838637405405360,
                0.798252571381053,
                0.822841039495085,
                0.841593665814167,
                0.826140209658119,
                1.094008872602152,
                1.410576134814834,
                1.500385179763377,
                1.183508904704664,
                0.649904138880630,
                0.281271797868616,
                0.120344166235758,
                0.024276957298115
            };

            var target2 = new[]
            {
                0.030642885222870,
                0.228376261252129,
                0.696384239185831,
                1.118148544942429,
                1.115714407941796,
                1.649681392896622,
                1.546094578511470,
                2.403339593306050,
                1.640616065530008,
                2.313309936596889,
                2.057177973907335,
                2.624883046543862,
                2.557340468529460,
                2.398710561054729,
                2.357714037548000,
                2.076032996517954,
                1.751812342971872,
                1.350244496751099,
                0.768975585726849,
                0.535692359201659,
                0.216900371766017,
                0.064156852325964,
                0.037485314888463
            };
            
            var result = Dsp.Convolve(y, x).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);     
                   
            result = Dsp.Convolve(y, x2).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target2, result);

            result = Dsp.Convolve((IEnumerable<double>)x, y.ToReadOnlyList()).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            result = Dsp.Convolve((IEnumerable<double>)x2, y.ToReadOnlyList()).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target2, result);

            Assert.That(Dsp.Convolve(Enumerable.Empty<double>(), x).ToReadOnlyList().Count == 0);
            Assert.That(Dsp.Convolve(y, new List<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.Convolve(null, x).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.Convolve(y, null).ToReadOnlyList());
        }

        [Test]
        public void TestCrossCorrelate()
        {
            Fft.FftProvider = new FftwProvider();

            var x = new[]
            {
                0.616044676146639,
                0.473288848902729,
                0.351659507062997,
                0.830828627896291,
                0.585264091152724,
                0.549723608291140,
                0.917193663829810,
                0.285839018820374,
                0.757200229110721,
                0.753729094278495
            };

            var y = new[]
            {
                0.380445846975357,
                0.567821640725221,
                0.075854289563064,
                0.053950118666607,
                0.530797553008973,
                0.779167230102011,
                0.934010684229183,
                0.129906208473730,
                0.568823660872193,
                0.469390641058206
            };

            var target = new[]
            {
                0.289165605456966,
                0.572578144138708,
                0.514311605265986,
                1.226890899803151,
                1.715053887382157,
                1.723096881636733,
                2.153707981735077,
                2.180225109104805,
                2.452308536614765,
                2.991904576873709,
                2.274857481454638,
                2.194310061297180,
                2.185291802257769,
                1.609001416200661,
                1.192553191608914,
                0.609348760441016,
                0.595874529034391,
                0.716057373469512,
                0.286753103662751
            };

            var result = Dsp.CrossCorrelate(x, y);

            FilterAssert.ListsAreReasonablyClose(target, result);

            Assert.That(Dsp.CrossCorrelate(new List<double>(), y).Count == 0);
            Assert.That(Dsp.CrossCorrelate(x, new List<double>()).Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.CrossCorrelate(null, y));
            Assert.Throws<ArgumentNullException>(() => Dsp.CrossCorrelate(x, null));
        }

        [Test]
        public void TestCrossCorrelate2()
        {
            Fft.FftProvider = new FftwProvider();

            var x = new[]
            {
                0.616044676146639,
                0.473288848902729,
                0.351659507062997,
                0.830828627896291,
                0.585264091152724,
                0.549723608291140,
                0.917193663829810,
                0.285839018820374,
                0.757200229110721,
                0.753729094278495
            };

            var y = new[]
            {
                0.380445846975357,
                0.567821640725221,
                0.075854289563064,
                0.053950118666607,
                0.530797553008973,
                0.779167230102011,
                0.934010684229183,
                0.129906208473730,
                0.568823660872193,
                0.469390641058206
            };

            var target = new[]
            {
                0.289165605456966,
                0.572578144138708,
                0.514311605265986,
                1.226890899803151,
                1.715053887382157,
                1.723096881636733,
                2.153707981735077,
                2.180225109104805,
                2.452308536614765,
                2.991904576873709,
                2.274857481454638,
                2.194310061297180,
                2.185291802257769,
                1.609001416200661,
                1.192553191608914,
                0.609348760441016,
                0.595874529034391,
                0.716057373469512,
                0.286753103662751
            };

            var result = Dsp.CrossCorrelate((IEnumerable<double>)x, y).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(target, result);

            Assert.That(Dsp.CrossCorrelate(Enumerable.Empty<double>(), y).ToReadOnlyList().Count == 0);
            Assert.That(Dsp.CrossCorrelate((IEnumerable<double>)x, new List<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.CrossCorrelate((IEnumerable<double>)null, y).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.CrossCorrelate((IEnumerable<double>)x, null).ToReadOnlyList());
        }

        [Test]
        public void TestDbToLinear()
        {
            var input = new[] {0, 20, 6.020599913279624};
            var target = new[] {1.0, 10, 2};

            var result = Dsp.DbToLinear(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.That(Dsp.DbToLinear(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.DbToLinear(null).ToReadOnlyList());
        }

        [Test]
        public void TestDegToRad()
        {
            var input = new[] {0.0, -90, 180, 270};
            var target = new[] {0, -Math.PI * 0.5, Math.PI, 1.5 * Math.PI};

            var result = Dsp.DegToRad(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.That(Dsp.DegToRad(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.DegToRad(null).ToReadOnlyList());
        }

        [Test]
        public void TestFindRoot()
        {
            var func = new Func<double, double>(x => 5 * Math.Pow(x, 3) + 2 * Math.Pow(x, 2) + 4 * x + 1);

            var result = Dsp.FindRoot(func, 0, 1);
            var target = -0.261840345691399;

            Assert.AreEqual(result, target, 1e-14);

            var func2 = new Func<double, double>(x => Math.Pow(x, 2) + 2);
            Assert.Throws<Exception>(() => Dsp.FindRoot(func2, 0, 1));

            Assert.Throws<ArgumentNullException>(() => Dsp.FindRoot(null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.FindRoot(func, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.FindRoot(func, 0, 1, -1));
        }

        [Test]
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

            var result = Dsp.HalfSinc(1000, 44100);
            FilterAssert.ListsAreReasonablyClose(target, result.Take(11).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.HalfSinc(-1, 44100).Take(1).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.HalfSinc(1000, -1).Take(1).ToReadOnlyList());
        }

        [Test]
        public void TestIirFilter()
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

            var input = new[]
            {
                0.011902069501241,
                0.337122644398882,
                0.162182308193243,
                0.794284540683907,
                0.311215042044805,
                0.528533135506213,
                0.165648729499781,
                0.601981941401637,
                0.262971284540144,
                0.654079098476782
            };

            var target = new[]
            {
                0.011951125590942,
                0.338608747553482,
                0.165679682794458,
                0.801585643285872,
                0.322746670917094,
                0.542922698153360,
                0.181975922148892,
                0.620199851895649,
                0.283093784100937,
                0.675895116230680,
                0.021967963324124,
                0.018979594184005,
                0.015640291287202,
                0.012021084391531,
                0.008197893160596,
                0.004249945003087,
                0.000258142993172,
                -0.003696582700258,
                -0.007534906469386,
                -0.011180689296962
            };

            var result = Dsp.IirFilter(input, a, b).Take(20).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            FilterAssert.ListContainsPlausibleValues(Dsp.IirFilter(input, new[]{1.0}, b).Take(20).ToReadOnlyList());

            FilterAssert.ListContainsOnlyZeroes(Dsp.IirFilter(Enumerable.Empty<double>(), a, b).Take(10).ToReadOnlyList());
            FilterAssert.ListContainsOnlyZeroes(Dsp.IirFilter(input, a, Enumerable.Empty<double>().ToReadOnlyList()).Take(10).ToReadOnlyList());

            Assert.Throws<ArgumentNullException>(() => Dsp.IirFilter(null, a, b).Take(20).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.IirFilter(input, null, b).Take(20).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.IirFilter(input, a, null).Take(20).ToReadOnlyList());

            Assert.Throws<Exception>(() => Dsp.IirFilter(input, Enumerable.Empty<double>().ToReadOnlyList(), b).Take(20).ToReadOnlyList());
            Assert.Throws<Exception>(() => Dsp.IirFilter(input, new[] { 0.0 }, b).Take(20).ToReadOnlyList());
        }

        [Test]
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

            var result = Dsp.IirFrequencyResponse(a, b, frequencies, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(result, target);

            Assert.That(Dsp.IirFrequencyResponse(a, b, Enumerable.Empty<double>().ToReadOnlyList(), 44100).ToReadOnlyList().Count == 0);

            Assert.Throws<ArgumentNullException>(() => Dsp.IirFrequencyResponse(null, b, frequencies, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.IirFrequencyResponse(a, null, frequencies, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.IirFrequencyResponse(a, b, null, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.IirFrequencyResponse(a, b, frequencies, -1).ToReadOnlyList());

            Assert.Throws<Exception>(() => Dsp.IirFrequencyResponse(Enumerable.Empty<double>().ToReadOnlyList(), b, frequencies, 44100).ToReadOnlyList());
            Assert.Throws<Exception>(() => Dsp.IirFrequencyResponse(new [] {0.0}, b, frequencies, 44100).ToReadOnlyList());
        }

        [Test]
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

            var result = Dsp.InterpolateComplex(x, y, x2).ToReadOnlyList();

            Assert.That(result.Count == x2.Length);
            FilterAssert.ListIsMonotonouslyRising(result.Select(c => c.Magnitude));
            Assert.GreaterOrEqual(result[0].Magnitude, 0);
            Assert.GreaterOrEqual(result[result.Count - 1].Magnitude, 4);
            Assert.LessOrEqual(result[0].Magnitude, 1);
            Assert.LessOrEqual(result[result.Count - 1].Magnitude, 5);

            Assert.Throws<ArgumentNullException>(() => Dsp.InterpolateComplex(null, y, x2).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.InterpolateComplex(x, null, x2).ToReadOnlyList());
            Assert.Throws<ArgumentNullException>(() => Dsp.InterpolateComplex(x, y, null).ToReadOnlyList());
            Assert.Catch<Exception>(() => Dsp.InterpolateComplex(x2, y, x2).ToReadOnlyList());
        }

        [Test]
        public void TestLambertW()
        {
            double[] input = {-0.367879, -0.2, -0.1, 0, 0.5, 1, 2, 5, 10, 50, 10000, 1e50};
            double[] target =
            {   -0.998452103780727259318294980306402,
                -0.259171101819073745,
                -0.11183255915896296483356945682026584227264536229126586,
                0,
                0.351733711249195826024909300929951065171464215,
                0.5671432904097838729999686622103555497538,
                0.85260550201372549134647241469531746689,
                1.326724665242200223635099297758079660128793554638,
                1.74552800274069938307430126487538991153528812908,
                2.860890177982210866757626984338803536992633740,
                7.23184603809337270647561850014125388396765914,
                110.424918827313345741844606222084501335079416897000087
            };

            var output = input.Select(Dsp.LambertW).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(output, target, 1e-13);
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.LambertW(-1));
        }

        [Test]
        public void TestLinearToDb()
        {
            var input = new[] { 1.0, 10, 2, 0};
            var target = new[] { 0, 20, 6.020599913279624, -100 };

            var result = Dsp.LinearToDb(input, -100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.That(Dsp.LinearToDb(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.LinearToDb(null).ToReadOnlyList());
        }

        [Test]
        public void TestLinSeries()
        {
            double[] target = {1, 2, 3, 4, 5};
            var result = Dsp.LinSeries(1, 5, 5).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.LinSeries(1, 5, 1));
        }

        [Test]
        public void TestLogSeries()
        {
            double[] target = { 1, 2, 4, 8, 16 };   
            var result = Dsp.LogSeries(1, 16, 5).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.LogSeries(1, 16, 1));
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.LogSeries(-1, 16, 5));
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.LogSeries(1, -1, 5));
        }

        [Test]
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

            var result = Dsp.LogSweep(5000, 10000, 21.0 / 44100, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            result = Dsp.LogSweep(10000, 5000, 21.0 / 44100, 44100).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target.Reverse().ToReadOnlyList(), result);

            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.LogSweep(-1, 10000, .1, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.LogSweep(5000, -1, .1, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.LogSweep(5000, 10000, -1, 44100).ToReadOnlyList());
            Assert.Throws<ArgumentOutOfRangeException>(() => Dsp.LogSweep(5000, 10000, .1, -1).ToReadOnlyList());
            Assert.Throws<ArgumentException>(() => Dsp.LogSweep(20, 20, .1, 44100).ToReadOnlyList());
        }

        [Test]
        public void TestMinimumDistance()
        {
            double[] input = {1, 2, 2.1, 3, 4, 10};
            Assert.AreEqual(Dsp.MinimumDistance(input), .1, 1e-14);

            Assert.Throws<ArgumentNullException>(() => Dsp.MinimumDistance(null));
        }

        [Test]
        public void TestMod()
        {
            Assert.AreEqual(Dsp.Mod(21, 20), 1);
            Assert.AreEqual(Dsp.Mod(-1, 20), 19);
            Assert.AreEqual(Dsp.Mod(0, 20), 0);
            Assert.AreEqual(Dsp.Mod(-60, 20), 0);

            Assert.AreEqual(Dsp.Mod(1.5, 20), 1.5, 1e-14);
            Assert.AreEqual(Dsp.Mod(-1.5, 20), 18.5, 1e-14);
            Assert.AreEqual(Dsp.Mod(0.0, 20), 0.0, 1e-14);
            Assert.That(double.IsNaN(Dsp.Mod(1.0, 0)));

            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.Mod(1, 0));
        }

        [Test]
        public void TestModBessel0()
        {
            double[] input = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};

            double[] target =
            {
                1,
                1.26606587775201e+000,
                2.27958530233607e+000,
                4.88079258586502e+000,
                11.3019219521363e+000,
                27.2398718236044e+000,
                67.2344069764780e+000,
                168.593908510290e+000,
                427.564115721805e+000,
                1.09358835451137e+003,
                2.81571662846625e+003
            };

            var result = input.Select(Dsp.ModBessel0).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result, 1e-13);

            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.ModBessel0(-1));
        }

        [Test]
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
                new Complex(5, 0),
            };

            var result = Dsp.PolarToComplex(amp, phase).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(result, target);

            Assert.That(Dsp.PolarToComplex(Enumerable.Empty<double>(), phase).ToReadOnlyList().Count == 0);
            Assert.That(Dsp.PolarToComplex(amp, Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);

            Assert.Catch<ArgumentNullException>(() => Dsp.PolarToComplex(null, phase));
            Assert.Catch<ArgumentNullException>(() => Dsp.PolarToComplex(amp, null));
        }


        [Test]
        public void TestRadToDeg()
        {
            var target = new[] { 0.0, -90, 180, 270 };
            var input = new[] { 0, -Math.PI * 0.5, Math.PI, 1.5 * Math.PI };

            var result = Dsp.RadToDeg(input).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result);

            Assert.That(Dsp.RadToDeg(Enumerable.Empty<double>()).ToReadOnlyList().Count == 0);
            Assert.Throws<ArgumentNullException>(() => Dsp.RadToDeg(null).ToReadOnlyList());
        }

        [Test]
        public void TestRightShift()
        {
            double[] input = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            double[] target1 = {0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            double[] target2 = {3, 4, 5, 6, 7, 8, 9};

            var result1 = Dsp.RightShift(input, 2).ToReadOnlyList();
            var result2 = Dsp.RightShift(input, -2).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target1, result1);
            FilterAssert.ListsAreReasonablyClose(target2, result2);

            Assert.Throws<ArgumentNullException>(() => Dsp.RightShift(null, 2).ToReadOnlyList());
        }

        [Test]
        public void TestSmooth()
        {
            double[] x = {1, 2, 3, 4, 5, 6, 7, 8, 9};
            double[] y = {1, 2, 3, 4, 5, 6, 7, 8, 9};

            var result = Dsp.Smooth(x, y, 1).ToReadOnlyList();

            Assert.That(result.Count == x.Length);
            Assert.GreaterOrEqual(result[0], 1);
            Assert.LessOrEqual(result[result.Count - 1], 9);
            FilterAssert.ListIsMonotonouslyRising(result);

            FilterAssert.ListsAreReasonablyClose(Dsp.Smooth(x, y, 0).ToReadOnlyList(), y);

            Assert.Catch<ArgumentNullException>(() => Dsp.Smooth(null, y, 1).ToReadOnlyList());
            Assert.Catch<ArgumentNullException>(() => Dsp.Smooth(x, null, 1).ToReadOnlyList());
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.Smooth(x, y, -1).ToReadOnlyList());
            Assert.Catch<ArgumentException>(() => Dsp.Smooth(new List<double> {1}, y, -1).ToReadOnlyList());
            Assert.Catch<ArgumentException>(() => Dsp.Smooth(new List<double>(), new List<double>(), -1).ToReadOnlyList());
        }

        [Test]
        public void TestUnwrapPhase()
        {
            double[] input = {170, 179, -175, 0, 170, -90, 170, 10, -150, 150, 0};
            double[] target = {170, 179, 185, 360, 530, 630, 530, 370, 210, 150, 0};

            var input2 = Dsp.DegToRad(input).ToReadOnlyList();

            var result = Dsp.UnwrapPhase(input, true).ToReadOnlyList();
            var result2 = Dsp.UnwrapPhase(input2, false).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(result, target);
            FilterAssert.ListsAreReasonablyClose(Dsp.RadToDeg(result2).ToReadOnlyList(), target, 1e-13);

            Assert.That(Dsp.UnwrapPhase(new List<double>()).ToReadOnlyList().Count == 0);
            Assert.Catch<ArgumentNullException>(() => Dsp.UnwrapPhase(null).ToReadOnlyList());
        }

        [Test]
        public void TestWhiteNoise()
        {
            var noise = Dsp.WhiteNoise().Take(1000000).ToReadOnlyList();
            var noise1 = Dsp.WhiteNoise().Take(1).ToReadOnlyList();

            Assert.That(noise1[0] != noise[0]);
            Assert.AreEqual(noise.Average(), 0, 1e-2);
            Assert.AreEqual(noise.Variance(), 1, 1e-2);
        }

        [Test]
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

            var result = Dsp.WindowedSinc(5000, 44100, 21, -10);
            FilterAssert.ListsAreReasonablyClose(result.ToReadOnlyList(), target);

            Assert.That(Dsp.WindowedSinc(5000, 44100, 0, -10).ToReadOnlyList().Count == 0);

            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.WindowedSinc(-1, 44100, 21, -10).ToReadOnlyList());
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.WindowedSinc(5000, -1, 21, -10).ToReadOnlyList());
            Assert.Catch<ArgumentOutOfRangeException>(() => Dsp.WindowedSinc(5000, 44100, -1, -10).ToReadOnlyList());
        }

        [Test]
        public void TestWrapPhase()
        {
            double[] input = { 170, 179, 185, 360, 530, 630, 530, 370, 210, 150, 0 };
            double[] target = { 170, 179, -175, 0, 170, -90, 170, 10, -150, 150, 0 };

            var input2 = Dsp.DegToRad(input).ToReadOnlyList();

            var result = Dsp.WrapPhase(input, true).ToReadOnlyList();
            var result2 = Dsp.WrapPhase(input2, false).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(result, target);
            FilterAssert.ListsAreReasonablyClose(Dsp.RadToDeg(result2).ToReadOnlyList(), target, 1e-13);

            Assert.That(Dsp.UnwrapPhase(new List<double>()).ToReadOnlyList().Count == 0);
            Assert.Catch<ArgumentNullException>(() => Dsp.UnwrapPhase(null).ToReadOnlyList());
        }
    }
}
