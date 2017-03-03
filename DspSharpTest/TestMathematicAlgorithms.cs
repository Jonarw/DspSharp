using System;
using System.Linq;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class TestMathematicAlgorithms
    {
        [TestMethod]
        public void TestFindRoot()
        {
            var func = new Func<double, double>(x => 5 * Math.Pow(x, 3) + 2 * Math.Pow(x, 2) + 4 * x + 1);

            var result = Mathematic.FindRoot(func, 0, 1);
            var target = -0.261840345691399;

            Assert.AreEqual(result, target, 1e-14);

            var func2 = new Func<double, double>(x => Math.Pow(x, 2) + 2);
            ThrowsAssert.Throws<Exception>(() => Mathematic.FindRoot(func2, 0, 1));

            ThrowsAssert.Throws<ArgumentNullException>(() => Mathematic.FindRoot(null, 0, 1));
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Mathematic.FindRoot(func, 0, 0));
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Mathematic.FindRoot(func, 0, 1, -1));
        }

        [TestMethod]
        public void TestLambertW()
        {
            double[] input = {-0.367879, -0.2, -0.1, 0, 0.5, 1, 2, 5, 10, 50, 10000, 1e50};
            double[] target =
            {
                -0.998452103780727259318294980306402,
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

            var output = input.Select(Mathematic.LambertW).ToReadOnlyList();

            FilterAssert.ListsAreReasonablyClose(output, target, 1e-13);
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Mathematic.LambertW(-1));
        }

        [TestMethod]
        public void TestMinimumDistance()
        {
            double[] input = {1, 2, 2.1, 3, 4, 10};
            Assert.AreEqual(Mathematic.MinimumDistance(input), .1, 1e-14);

            ThrowsAssert.Throws<ArgumentNullException>(() => Mathematic.MinimumDistance(null));
        }

        [TestMethod]
        public void TestMod()
        {
            Assert.AreEqual(Mathematic.Mod(21, 20), 1);
            Assert.AreEqual(Mathematic.Mod(-1, 20), 19);
            Assert.AreEqual(Mathematic.Mod(0, 20), 0);
            Assert.AreEqual(Mathematic.Mod(-60, 20), 0);

            Assert.AreEqual(Mathematic.Mod(1.5, 20), 1.5, 1e-14);
            Assert.AreEqual(Mathematic.Mod(-1.5, 20), 18.5, 1e-14);
            Assert.AreEqual(Mathematic.Mod(0.0, 20), 0.0, 1e-14);
            Assert.IsTrue(double.IsNaN(Mathematic.Mod(1.0, 0)));

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Mathematic.Mod(1, 0));
        }

        [TestMethod]
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

            var result = input.Select(Mathematic.ModBessel0).ToReadOnlyList();
            FilterAssert.ListsAreReasonablyClose(target, result, 1e-13);

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => Mathematic.ModBessel0(-1));
        }
    }
}