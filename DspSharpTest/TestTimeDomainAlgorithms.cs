// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestTimeDomainAlgorithms.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using DspSharp.Algorithms;
using DspSharp.Extensions;
using DspSharp.Signal.Windows;
using DspSharpFftw;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class TestTimeDomainAlgorithms
    {
        [TestMethod]
        public void TestCircularConvolve()
        {
            Fft.FftProvider = new FftwProvider();

            double[] x =
            {
                0.814723686393179,
                0.905791937075619,
                0.126986816293506,
                0.913375856139019,
                0.632359246225410,
                0.097540404999410,
                0.278498218867048,
                0.546881519204984,
                0.957506835434298,
                0.964888535199277,
            };

            double[] y =
            {
                0.157613081677548,
                0.970592781760616,
                0.957166948242946,
                0.485375648722841,
                0.800280468888800,
                0.141886338627215,
                0.421761282626275,
                0.915735525189067,
                0.792207329559554,
                0.959492426392903,
            };

            double[] target =
            {
                4.556393918484424,
                4.264651724078043,
                4.575316744471095,
                3.607392496105358,
                3.555148746578472,
                4.389748340514129,
                3.927789539974260,
                4.316984545134884,
                4.085545491259646,
                3.908653395918341,
            };

            double[] y2 =
            {
                0.655740699156587,
                0.035711678574190,
                0.849129305868777,
                0.933993247757551,
                0.678735154857773,
            };

            double[] target2 =
            {
                2.081562561471973,
                2.707867552261903,
                2.358516459454473,
                2.788457304892876,
                1.954095266955865,
                1.595515689887880,
                1.662338014872647,
                1.661942094317720,
                1.404193788584711,
                1.457603541640849,
            };

            var result = TimeDomain.CircularConvolve(x, y);
            DspAssert.ListsAreReasonablyClose(result, target);

            var result2 = TimeDomain.CircularConvolve(x, y2);
            DspAssert.ListsAreReasonablyClose(result2, target2);
        }

        [TestMethod]
        public void TestConvertTimeToSamples()
        {
            bool integer;

            var result = TimeDomain.ConvertTimeToSamples(1, 44100, out integer);
            Assert.IsTrue(integer);
            Assert.IsTrue(result == 44100);

            result = TimeDomain.ConvertTimeToSamples(44100.6 / 44100, 44100, out integer);
            Assert.IsTrue(!integer);
            Assert.IsTrue(result == 44101);

            result = TimeDomain.ConvertTimeToSamples(-1, 44100, out integer);
            Assert.IsTrue(integer);
            Assert.IsTrue(result == -44100);

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => TimeDomain.ConvertTimeToSamples(1, -1, out integer));
        }

        [TestMethod]
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

            var result = TimeDomain.Convolve(x, y);

            DspAssert.ListsAreReasonablyClose(target, result);

            Assert.IsTrue(TimeDomain.Convolve(new List<double>(), y).Count == 0);
            Assert.IsTrue(TimeDomain.Convolve(x, new List<double>()).Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.Convolve(null, y));
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.Convolve(x, null));
        }

        [TestMethod]
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

            var result = TimeDomain.Convolve(y, x).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target, result);

            result = TimeDomain.Convolve(y, x2).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target2, result);

            result = TimeDomain.Convolve((IEnumerable<double>)x, y.ToReadOnlyList()).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target, result);

            result = TimeDomain.Convolve((IEnumerable<double>)x2, y.ToReadOnlyList()).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target2, result);

            Assert.IsTrue(TimeDomain.Convolve(Enumerable.Empty<double>(), x).ToReadOnlyList().Count == 0);
            Assert.IsTrue(TimeDomain.Convolve(y, new List<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.Convolve(null, x).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.Convolve(y, null).ToReadOnlyList());
        }

        [TestMethod]
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

            var result = TimeDomain.CrossCorrelate(x, y);

            DspAssert.ListsAreReasonablyClose(target, result);

            Assert.IsTrue(TimeDomain.CrossCorrelate(new List<double>(), y).Count == 0);
            Assert.IsTrue(TimeDomain.CrossCorrelate(x, new List<double>()).Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.CrossCorrelate(null, y));
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.CrossCorrelate(x, null));
        }

        [TestMethod]
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

            var result = TimeDomain.CrossCorrelate((IEnumerable<double>)x, y).ToReadOnlyList();

            DspAssert.ListsAreReasonablyClose(target, result);

            Assert.IsTrue(TimeDomain.CrossCorrelate(Enumerable.Empty<double>(), y).ToReadOnlyList().Count == 0);
            Assert.IsTrue(TimeDomain.CrossCorrelate((IEnumerable<double>)x, new List<double>()).ToReadOnlyList().Count == 0);
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.CrossCorrelate((IEnumerable<double>)null, y).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.CrossCorrelate((IEnumerable<double>)x, null).ToReadOnlyList());
        }

        [TestMethod]
        public void TestFrequencyWindowedBandpass()
        {
            var sig = SignalGenerators.GetDirac(1000).Multiply(2).ToReadOnlyList();
            var inv = TimeDomain.FrequencyWindowedBandpass(sig, 44100, 100, 1000, 10, 10, WindowType.BlackmanHarris).ToSignal(44100);
            var spec = inv.Spectrum;

            Assert.AreEqual(spec.GetValue(50).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(1050).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(950).Magnitude, 2, 1e-3);
            Assert.AreEqual(spec.GetValue(150).Magnitude, 2, 1e-3);
            Assert.AreEqual(spec.GetValue(500).Magnitude, 2, 1e-3);
            Assert.AreEqual(spec.GetValue(10).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(10000).Magnitude, 0d, 1e-3);

            Assert.AreEqual(spec.GetValue(950).Phase, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(150).Phase, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(500).Phase, 0d, 1e-3);
        }

        [TestMethod]
        public void TestFrequencyWindowedInversion()
        {
            var sig = SignalGenerators.GetDirac(1000).Multiply(2).ToReadOnlyList();
            var inv = TimeDomain.FrequencyWindowedInversion(sig, 44100, 100, 1000, 10, 10, WindowType.BlackmanHarris).ToSignal(44100);
            var spec = inv.Spectrum;

            Assert.AreEqual(spec.GetValue(50).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(1050).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(950).Magnitude, .5, 1e-3);
            Assert.AreEqual(spec.GetValue(150).Magnitude, .5, 1e-3);
            Assert.AreEqual(spec.GetValue(500).Magnitude, .5, 1e-3);
            Assert.AreEqual(spec.GetValue(10).Magnitude, 0d, 1e-3);
            Assert.AreEqual(spec.GetValue(10000).Magnitude, 0d, 1e-3);

            Assert.AreEqual(spec.GetValue(950).Phase, 0, 1e-3);
            Assert.AreEqual(spec.GetValue(150).Phase, 0, 1e-3);
            Assert.AreEqual(spec.GetValue(500).Phase, 0, 1e-3);
        }

        [TestMethod]
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

            var result = TimeDomain.IirFilter(input, a, b).Take(20).ToReadOnlyList();
            DspAssert.ListsAreReasonablyClose(target, result);

            DspAssert.ListContainsPlausibleValues(TimeDomain.IirFilter(input, new[] {1.0}, b).Take(20).ToReadOnlyList());

            DspAssert.ListContainsOnlyZeroes(TimeDomain.IirFilter(Enumerable.Empty<double>(), a, b).Take(10).ToReadOnlyList());
            DspAssert.ListContainsOnlyZeroes(TimeDomain.IirFilter(input, a, Enumerable.Empty<double>().ToReadOnlyList()).Take(10).ToReadOnlyList());

            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.IirFilter(null, a, b).Take(20).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.IirFilter(input, null, b).Take(20).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentNullException>(() => TimeDomain.IirFilter(input, a, null).Take(20).ToReadOnlyList());

            ThrowsAssert.Throws<Exception>(
                () => TimeDomain.IirFilter(input, Enumerable.Empty<double>().ToReadOnlyList(), b).Take(20).ToReadOnlyList());
            ThrowsAssert.Throws<Exception>(() => TimeDomain.IirFilter(input, new[] {0.0}, b).Take(20).ToReadOnlyList());
        }
    }
}