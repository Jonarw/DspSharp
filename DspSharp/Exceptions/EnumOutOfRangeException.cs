// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SamplerateMismatchException.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace UTilities
{
    public static class EnumOutOfRangeException
    {
        public static EnumOutOfRangeException<T> Create<T>(T value) where T : Enum
        {
            return new EnumOutOfRangeException<T>(value);
        }
    }

    public class EnumOutOfRangeException<T> : Exception where T : Enum
    {
        public EnumOutOfRangeException(T value) : base($"The value {(long)(object)value} is not defined for {typeof(T).Name}.")
        {
        }
    }
}