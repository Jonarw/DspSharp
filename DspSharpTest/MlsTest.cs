﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MlsTest.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using DspSharp.Algorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace DspSharpTest
{
    [TestClass]
    public class MlsTest
    {
        [TestMethod]
        public void TestMlsLowOrders()
        {
            for (int i = 2; i < 21; i++)
            {
                var sequence = SignalGenerators.Mls(i);

                Assert.IsTrue(sequence.Count() == Math.Pow(2, i) - 1);
            }

            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.Mls(1).ToReadOnlyList());
            ThrowsAssert.Throws<ArgumentOutOfRangeException>(() => SignalGenerators.Mls(SignalGenerators.MlsFeedbackTaps.Count).ToReadOnlyList());
        }

        //[TestMethod]
        public void TestMlsHighOrders()
        {
            for (int i = 21; i < SignalGenerators.MlsFeedbackTaps.Count; i++)
            {
                var sequence = SignalGenerators.Mls(i);

                Assert.IsTrue(sequence.Count() == Math.Pow(2, i) - 1);
            }
        }
    }
}