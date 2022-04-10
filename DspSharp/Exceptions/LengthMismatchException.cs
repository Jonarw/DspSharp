// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplerateMismatchException.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharp.Exceptions
{
    public class LengthMismatchException : Exception
    {
        public LengthMismatchException(string parameter1, string parameter2) : base($"{parameter1} and {parameter2} must be the same length.")
        {
        }

        public LengthMismatchException() : base("The length of the two sequences does not match.")
        {
        }
    }
}