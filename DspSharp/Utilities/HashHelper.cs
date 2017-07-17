// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashHelper.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp.Utilities
{
    /// <summary>
    ///     Provides a quick way to generate hash codes for implementing the .GetHashCode() method.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        ///     Alternative way to get a hashcode is to use a fluent
        ///     interface like this:<br />
        ///     return 0.CombineHashCode(field1).CombineHashCode(field2).
        ///     CombineHashCode(field3);
        /// </summary>
        public static int CombineHashCode<T>(this int hashCode, T arg)
        {
            unchecked
            {
                return 31 * hashCode + arg.GetHashCode();
            }
        }

        /// <summary>
        ///     Gets the combined hash code of two parameters.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <param name="arg1">The first parameter.</param>
        /// <param name="arg2">The second parameter.</param>
        /// <returns>The combined hash code of the parameters.</returns>
        public static int GetHashCode<T1, T2>(T1 arg1, T2 arg2)
        {
            unchecked
            {
                return 31 * arg1.GetHashCode() + arg2.GetHashCode();
            }
        }

        /// <summary>
        ///     Gets the combined hash code of three parameters.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <param name="arg1">The first parameter.</param>
        /// <param name="arg2">The second parameter.</param>
        /// <param name="arg3">The third parameter.</param>
        /// <returns>The combined hash code of the parameters.</returns>
        public static int GetHashCode<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3)
        {
            unchecked
            {
                var hash = arg1.GetHashCode();
                hash = 31 * hash + arg2.GetHashCode();
                return 31 * hash + arg3.GetHashCode();
            }
        }

        /// <summary>
        ///     Gets the combined hash code of three parameters.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter.</typeparam>
        /// <param name="arg1">The first parameter.</param>
        /// <param name="arg2">The second parameter.</param>
        /// <param name="arg3">The third parameter.</param>
        /// <param name="arg4">The fourth parameter.</param>
        /// <returns>The combined hash code of the parameters.</returns>
        public static int GetHashCode<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            unchecked
            {
                var hash = arg1.GetHashCode();
                hash = 31 * hash + arg2.GetHashCode();
                hash = 31 * hash + arg3.GetHashCode();
                return 31 * hash + arg4.GetHashCode();
            }
        }

        /// <summary>
        ///     Gets the combined hash code of a list of objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <returns>The combined hash code of all objects in the list.</returns>
        public static int GetHashCode<T>(T[] list)
        {
            unchecked
            {
                var hash = 0;
                foreach (var item in list)
                {
                    hash = 31 * hash + item.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        ///     Gets the combined hash code of an IEnumberable of objects.
        /// </summary>
        /// <typeparam name="T">The type of the objects.</typeparam>
        /// <param name="list">The list of objects.</param>
        /// <returns>The combined hash code of all objects in the list.</returns>
        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                var hash = 0;
                foreach (var item in list)
                {
                    hash = 31 * hash + item.GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        ///     Gets a hashcode for a collection for that the order of items
        ///     does not matter.
        ///     So {1, 2, 3} and {3, 2, 1} will get same hash code.
        /// </summary>
        public static int GetHashCodeForOrderNoMatterCollection<T>(IEnumerable<T> list)
        {
            unchecked
            {
                var hash = 0;
                var count = 0;
                foreach (var item in list)
                {
                    hash += item.GetHashCode();
                    count++;
                }
                return 31 * hash + count.GetHashCode();
            }
        }
    }
}