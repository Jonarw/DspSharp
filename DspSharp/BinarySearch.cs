// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySearchExtensions.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DspSharp
{
    public static class BinarySearchExtensions
    {
        public static int BinarySearch<T>(this IReadOnlyList<T> sortedList, T value, IComparer<T> comparer = null)
        {
            comparer ??= Comparer<T>.Default;

            var high = sortedList.Count - 1;
            var low = 0;

            while (low <= high)
            {
                var mid = (high + low) >> 1;
                var comparison = comparer.Compare(value, sortedList[mid]);
                if (comparison == 0)
                    return mid;

                if (comparison < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }

            return ~low;
        }
    }
}