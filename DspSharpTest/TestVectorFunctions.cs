﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestVectorFunctions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DspSharpTest
{
    [TestClass]
    public class TestVectorFunctions
    {
        private readonly double[] input = {5, 6, 1, 100, 2, 3};

        [TestMethod]
        public void TestMaxIndex()
        {
            Assert.AreEqual(this.input.MaxIndex(), 3);
        }

        [TestMethod]
        public void TestMinIndex()
        {
            Assert.AreEqual(this.input.MinIndex(), 2);
        }
    }
}