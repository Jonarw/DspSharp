﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace DspSharp.Extensions
{
    public static class Utilities
    {
        /// <summary>
        ///     Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;

            var collection = enumerable as ICollection<T>;
            if (collection != null)
                return collection.Count < 1;

            return !enumerable.Any();
        }
    }
}