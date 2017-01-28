using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using Filter_Win;
using NUnit.Framework;
//using Filter_CrossPlatform;

namespace FilterTests
{
    [TestFixture]
    public class FftTest
    {
        private readonly Complex[] fftxComplex =
        {
            new Complex(6.238553055831749, +6.602111831687766),
            new Complex(1.354181005313841, +0.964221614229248),
            new Complex(-0.286456296497368, -0.178902625746594),
            new Complex(1.251129460280185, -1.037906571249956),
            new Complex(-1.333821911972046, -2.172769214747541),
            new Complex(-0.618403449404868, -0.344053609697519),
            new Complex(0.230469798736148, -0.381373412020015),
            new Complex(0.198032337434706, +0.068263596513588),
            new Complex(-0.287424189135541, -2.371569477649798),
            new Complex(1.400977053344984, +0.428108685456304)
        };

        private readonly Complex[] fftxEven =
        {
            new Complex(12.840664887519516, 0.000000000000000),
            new Complex(0.245748754832022, 0.701613594361947),
            new Complex(2.073744179172188, 0.291454488402043),
            new Complex(0.185565473751332, 0.434865843531484),
            new Complex(-1.562176294514651, 1.095849479632515),
            new Complex(1.627932083635650, 0.162896870938524),
            new Complex(0.239759411489261, -1.079633645304511),
            new Complex(1.783793726170643, -0.463993191833256),
            new Complex(-1.828747370001727, -0.113552046009665),
            new Complex(-0.557487014811493, -0.671908145463214),
            new Complex(-0.962457059102387, 0.000000000000000)
        };

        private readonly Complex[] fftxEvenLong =
        {
            new Complex(12.840664887519516, +0.000000000000000),
            new Complex(-2.336505049376223, -1.205094403303149),
            new Complex(0.239232171555840, -1.275062568087589),
            new Complex(1.706870670268013, -2.942555474257467),
            new Complex(1.634859281104858, -0.250037093715710),
            new Complex(-1.562176294514651, +1.095849479632515),
            new Complex(1.560897049040453, +0.076978685937275),
            new Complex(1.635076844404787, -1.643446692373355),
            new Complex(0.582051188757895, -0.349739471431378),
            new Complex(2.102296795149554, -1.497746622924533),
            new Complex(-1.828747370001727, -0.113552046009664),
            new Complex(0.051277965802424, -1.129719926316050),
            new Complex(-0.021419616036245, -0.585664635743748)
        };

        private readonly Complex[] fftxOdd =
        {
            new Complex(10.681233647363577, 0.000000000000000),
            new Complex(0.025333261449992, -0.524735214485467),
            new Complex(-0.303414542359190, 0.533439358464245),
            new Complex(0.613901189285935, -1.228039441010794),
            new Complex(0.620993893527226, -0.303782542462630),
            new Complex(-1.882324387531409, -0.106218494578395),
            new Complex(0.072296349602272, -1.335298787705063),
            new Complex(-1.085211650743170, +0.116305953433358),
            new Complex(-0.442519591037227, +0.045170557766656),
            new Complex(-0.464308672803711, -0.673585699065233),
            new Complex(1.511001126404082, -0.094521017906760)
        };

        private readonly Complex[] xComplex =
        {
            new Complex(0.814723686393179, +0.157613081677548),
            new Complex(0.905791937075619, +0.970592781760616),
            new Complex(0.126986816293506, +0.957166948242946),
            new Complex(0.913375856139019, +0.485375648722841),
            new Complex(0.632359246225410, +0.800280468888800),
            new Complex(0.097540404999410, +0.141886338627215),
            new Complex(0.278498218867048, +0.421761282626275),
            new Complex(0.546881519204984, +0.915735525189067),
            new Complex(0.957506835434298, +0.792207329559554),
            new Complex(0.964888535199277, +0.959492426392903)
        };

        private readonly double[] xEven =
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
            0.157613081677548,
            0.970592781760616,
            0.957166948242946,
            0.485375648722841,
            0.800280468888800,
            0.141886338627215,
            0.421761282626275,
            0.915735525189067,
            0.792207329559554,
            0.959492426392903
        };

        private readonly double[] xOdd =
        {
            0.381558457093008,
            0.765516788149002,
            0.795199901137063,
            0.186872604554379,
            0.489764395788231,
            0.445586200710899,
            0.646313010111265,
            0.709364830858073,
            0.754686681982361,
            0.276025076998578,
            0.679702676853675,
            0.655098003973841,
            0.162611735194631,
            0.118997681558377,
            0.498364051982143,
            0.959743958516081,
            0.340385726666133,
            0.585267750979777,
            0.223811939491137,
            0.751267059305653,
            0.255095115459269
        };

        [Test]
        public void TestFftw()
        {
            this.TestFftProviderReal(new FftwProvider());
            this.TestFftProviderComplex(new FftwProvider());
        }

        private void TestFftProviderComplex(IFftProvider provider)
        {
            var result = provider.ComplexFft(this.xComplex);
            var inverse = provider.ComplexIfft(result);

            FilterAssert.ListsAreReasonablyClose(result, this.fftxComplex);
            FilterAssert.ListsAreReasonablyClose(this.xComplex, inverse);

            var resultlong = provider.ComplexFft(this.xComplex, 25);
            var inverselong = provider.ComplexIfft(resultlong);

            FilterAssert.ListsAreReasonablyClose(this.xComplex.Concat(Enumerable.Repeat(Complex.Zero, 15)).ToReadOnlyList(), inverselong);

            Assert.That(provider.ComplexFft(new List<Complex>()).Count == 0);
            Assert.That(provider.ComplexIfft(new List<Complex>()).Count == 0);

            Assert.Catch<ArgumentNullException>(() => provider.ComplexFft(null));
            Assert.Catch<ArgumentNullException>(() => provider.ComplexIfft(null));
        }

        //[Test]
        //public void TestMathNet()
        //{
        //    this.TestFftProvider(new MathNetFftProvider());
        //}

        private void TestFftProviderReal(IFftProvider provider)
        {
            var result = provider.RealFft(this.xEven);
            var inverse = provider.RealIfft(result);

            FilterAssert.ListsAreReasonablyClose(result, this.fftxEven);
            FilterAssert.ListsAreReasonablyClose(this.xEven, inverse);

            var resultlong = provider.RealFft(this.xEven, 25);
            var inverselong = provider.RealIfft(resultlong);

            FilterAssert.ListsAreReasonablyClose(resultlong, this.fftxEvenLong);
            FilterAssert.ListsAreReasonablyClose(this.xEven.Concat(Enumerable.Repeat(0.0, 5)).ToReadOnlyList(), inverselong);

            var resultodd = provider.RealFft(this.xOdd);
            var inverseodd = provider.RealIfft(resultodd);
            FilterAssert.ListsAreReasonablyClose(resultodd, this.fftxOdd);
            FilterAssert.ListsAreReasonablyClose(this.xOdd, inverseodd);

            Assert.That(provider.RealFft(new List<double>()).Count == 0);
            Assert.That(provider.RealIfft(new List<Complex>()).Count == 0);

            Assert.Catch<ArgumentNullException>(() => provider.RealFft(null));
            Assert.Catch<ArgumentNullException>(() => provider.RealIfft(null));
        }
    }
}