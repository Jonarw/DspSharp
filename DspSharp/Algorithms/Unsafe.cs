// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Unsafe.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DspSharp.Algorithms
{
    public static unsafe class Unsafe
    {
        /// <summary>
        ///     Frees the specified memory.
        /// </summary>
        /// <param name="mem">The memory.</param>
        public static void Free(void* mem)
        {
            Marshal.FreeHGlobal((IntPtr)mem);
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in bytes).
        /// </summary>
        /// <param name="length">The length.</param>
        public static IntPtr Malloc(int length)
        {
            return Marshal.AllocHGlobal(length);
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in bytes).
        /// </summary>
        /// <param name="length">The length.</param>
        public static byte* MallocB(int length)
        {
            return (byte*)Marshal.AllocHGlobal(length * sizeof(byte));
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in Complex).
        /// </summary>
        /// <param name="length">The length.</param>
        public static Complex* MallocC(int length)
        {
            return (Complex*)Marshal.AllocHGlobal(length * 2 * sizeof(double));
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in doubles).
        /// </summary>
        /// <param name="length">The length.</param>
        public static double* MallocD(int length)
        {
            return (double*)Marshal.AllocHGlobal(length * sizeof(double));
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in ints).
        /// </summary>
        /// <param name="length">The length.</param>
        public static int* MallocI(int length)
        {
            return (int*)Marshal.AllocHGlobal(length * sizeof(int));
        }

        /// <summary>
        ///     Allocates a memory block of the specified length (in bytes).
        /// </summary>
        /// <param name="length">The length.</param>
        public static void* MallocV(int length)
        {
            return (void*)Marshal.AllocHGlobal(length);
        }

        /// <summary>
        ///     Copies one array to another.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in doubles). If not specified, the length of the smaller array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(double[] destination, double[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else
            {
                if ((count > destination.Length) || (count > source.Length))
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (double* pDest = destination)
            fixed (double* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(double));
            }
        }

        /// <summary>
        ///     Copies an array to the specified memory location.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in doubles). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(double* pDestination, double[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else
            {
                if (count > source.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (double* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * sizeof(double));
            }
        }

        /// <summary>
        ///     Copies data from one memory location to another.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in doubles).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(double* pDestination, double* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * sizeof(double));
        }

        /// <summary>
        ///     Copies data from one memory location to another.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in ints).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(int* pDestination, int* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * sizeof(int));
        }

        /// <summary>
        ///     Copies data from one memory location to another.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in bytes).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(byte* pDestination, byte* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count);
        }

        /// <summary>
        ///     Copies data from one memory location to another.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in Complex).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(Complex* pDestination, Complex* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * 2 * sizeof(double));
        }

        /// <summary>
        ///     Copies an array to the specified memory location.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in ints). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(int* pDestination, int[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else
            {
                if (count > source.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (int* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * sizeof(int));
            }
        }

        /// <summary>
        ///     Copies an array to the specified memory location.
        /// </summary>
        /// <param name="pDestination">The destination memory location.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in Complex). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(Complex* pDestination, Complex[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else
            {
                if (count > source.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (Complex* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * 2 * sizeof(double));
            }
        }

        /// <summary>
        ///     Copies data from a memory location to the specified array.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in doubles). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(double[] destination, double* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else
            {
                if (count > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (double* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(double));
            }
        }

        /// <summary>
        ///     Copies data from a memory location to the specified array.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in ints). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(int[] destination, int* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else
            {
                if (count > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (int* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(int));
            }
        }

        /// <summary>
        ///     Copies data from a memory location to the specified array.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="pSource">The source memory location.</param>
        /// <param name="count">The count (in Complex). If not specified, the length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(Complex[] destination, Complex* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else
            {
                if (count > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (Complex* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * 2 * sizeof(double));
            }
        }

        /// <summary>
        ///     Copies one array to another.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in ints). If not specified, the length of the smaller array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(int[] destination, int[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else
            {
                if ((count > destination.Length) || (count > source.Length))
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (int* pDest = destination)
            fixed (int* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(int));
            }
        }

        /// <summary>
        ///     Copies one array to another.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="source">The source array.</param>
        /// <param name="count">The count (in Complex). If not specified, the length of the smaller array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memcpy(Complex[] destination, Complex[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else
            {
                if ((count > destination.Length) || (count > source.Length))
                    throw new ArgumentOutOfRangeException(nameof(count));
            }

            fixed (Complex* pDest = destination)
            fixed (Complex* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * 2 * sizeof(double));
            }
        }

        /// <summary>
        ///     Sets the bytes of an array to the specified value.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in doubles). If not specified, the entire length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(double[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else
            {
                if (length > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }

            fixed (double* pDest = destination)
            {
                Interop.memset(pDest, value, length * sizeof(double));
            }
        }

        /// <summary>
        ///     Sets a memory location to the specified value.
        /// </summary>
        /// <param name="destination">The destination memory location.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in doubles).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(double* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * sizeof(double));
        }

        /// <summary>
        ///     Sets a memory location to the specified value.
        /// </summary>
        /// <param name="destination">The destination memory location.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in ints).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(int* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * sizeof(int));
        }

        /// <summary>
        ///     Sets a memory location to the specified value.
        /// </summary>
        /// <param name="destination">The destination memory location.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in Complex).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(Complex* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * 2 * sizeof(double));
        }

        /// <summary>
        ///     Sets a memory location to the specified value.
        /// </summary>
        /// <param name="destination">The destination memory location.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in bytes).</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(byte* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length);
        }

        /// <summary>
        ///     Sets the bytes of an array to the specified value.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in ints). If not specified, the entire length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(int[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else
            {
                if (length > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }

            fixed (int* pDest = destination)
            {
                Interop.memset(pDest, value, length * sizeof(int));
            }
        }

        /// <summary>
        ///     Sets the bytes of an array to the specified value.
        /// </summary>
        /// <param name="destination">The destination array.</param>
        /// <param name="value">The byte value.</param>
        /// <param name="length">The length (in Complex). If not specified, the entire length of the array is used.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Memset(Complex[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else
            {
                if (length > destination.Length)
                    throw new ArgumentOutOfRangeException(nameof(length));
            }

            fixed (Complex* pDest = destination)
            {
                Interop.memset(pDest, value, length * 2 * sizeof(double));
            }
        }

        /// <summary>
        ///     Reads bytes from an unmanaged memory location and converts them to a managed array.
        /// </summary>
        /// <param name="source">The source memory location.</param>
        /// <param name="length">The length (in ints).</param>
        /// <returns></returns>
        public static int[] ToManagedArray(int* source, int length)
        {
            var ret = new int[length];
            fixed (int* pRet = ret)
            {
                Interop.memcpy(pRet, source, length * sizeof(int));
            }

            return ret;
        }

        /// <summary>
        ///     Reads bytes from an unmanaged memory location and converts them to a managed array.
        /// </summary>
        /// <param name="source">The source memory location.</param>
        /// <param name="length">The length (in doubles).</param>
        /// <returns></returns>
        public static double[] ToManagedArray(double* source, int length)
        {
            var ret = new double[length];
            fixed (double* pRet = ret)
            {
                Interop.memcpy(pRet, source, length * sizeof(double));
            }

            return ret;
        }

        /// <summary>
        ///     Reads bytes from an unmanaged memory location and converts them to a managed array.
        /// </summary>
        /// <param name="source">The source memory location.</param>
        /// <param name="length">The length (in Complex).</param>
        /// <returns></returns>
        public static Complex[] ToManagedArray(Complex* source, int length)
        {
            var ret = new Complex[length];
            fixed (Complex* pRet = ret)
            {
                Interop.memcpy(pRet, source, length * 2 * sizeof(double));
            }

            return ret;
        }
    }
}