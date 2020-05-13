// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestStatistics.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public class TestStatistics
    {
        [TestMethod]
        public void TestStandardDeviation()
        {
            double[] x =
            {
                0.757740130578333,
                0.743132468124916,
                0.392227019534168,
                0.655477890177557,
                0.171186687811562,
                0.706046088019609,
                0.031832846377421,
                0.276922984960890,
                0.046171390631154,
                0.097131781235848,
            };

            Assert.AreEqual(0.302433511410605, x.StandardDeviation(Statistics.NormalisationMode.Sample), 1e-13);
            Assert.AreEqual(0.286913621046010, x.StandardDeviation(Statistics.NormalisationMode.Population), 1e-13);
        }

        [TestMethod]
        public void TestVariance()
        {
            double[] x =
            {
                0.757740130578333,
                0.743132468124916,
                0.392227019534168,
                0.655477890177557,
                0.171186687811562,
                0.706046088019609,
                0.031832846377421,
                0.276922984960890,
                0.046171390631154,
                0.097131781235848,
            };

            Assert.AreEqual(0.091466028824149, x.Variance(Statistics.NormalisationMode.Sample), 1e-13);
            Assert.AreEqual(0.082319425941734, x.Variance(Statistics.NormalisationMode.Population), 1e-13);
        }

        [TestMethod]
        public void TestMean()
        {
            double[] x =
            {
                0.757740130578333,
                0.743132468124916,
                0.392227019534168,
                0.655477890177557,
                0.171186687811562,
                0.706046088019609,
                0.031832846377421,
                0.276922984960890,
                0.046171390631154,
                0.097131781235848,
            };

            Assert.AreEqual(0.387786928745146, x.ArithmeticMean(), 1e-13);
        }
    }
}