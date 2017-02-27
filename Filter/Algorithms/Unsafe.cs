using System;
using System.Numerics;
using Filter.Algorithms.FftwProvider;

namespace Filter.Algorithms
{
    public static unsafe class Unsafe
    {
        public static void Free(void* mem)
        {
            FftwInterop.free(mem);
        }

        public static void* Malloc(int length)
        {
            return FftwInterop.malloc(length);
        }

        public static double* MallocD(int length)
        {
            return (double*)FftwInterop.malloc(length * sizeof(double));
        }

        public static int* MallocI(int length)
        {
            return (int*)FftwInterop.malloc(length * sizeof(int));
        }

        public static Complex* MallocC(int length)
        {
            return (Complex*)FftwInterop.malloc(length * 2 * sizeof(double));
        }

        public static void Memcpy(double[] destination, double[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else if (count > destination.Length || count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (double* pDest = destination)
            fixed (double* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(double));
            }
        }

        public static void Memcpy(double* pDestination, double[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else if (count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (double* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * sizeof(double));
            }
        }

        public static void Memcpy(double* pDestination, double* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * sizeof(double));
        }

        public static void Memcpy(int* pDestination, int* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * sizeof(int));
        }

        public static void Memcpy(byte* pDestination, byte* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count);
        }

        public static void Memcpy(Complex* pDestination, Complex* pSource, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Interop.memcpy(pDestination, pSource, count * 2 * sizeof(double));
        }

        public static void Memcpy(int* pDestination, int[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else if (count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (int* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * sizeof(int));
            }
        }

        public static void Memcpy(Complex* pDestination, Complex[] source, int count = -1)
        {
            if (count < 0)
                count = source.Length;
            else if (count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (Complex* pSource = source)
            {
                Interop.memcpy(pDestination, pSource, count * 2 * sizeof(double));
            }
        }

        public static void Memcpy(double[] destination, double* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else if (count > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (double* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(double));
            }
        }

        public static void Memcpy(int[] destination, int* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else if (count > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (int* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(int));
            }
        }

        public static void Memcpy(Complex[] destination, Complex* pSource, int count = -1)
        {
            if (count < 0)
                count = destination.Length;
            else if (count > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (Complex* pDest = destination)
            {
                Interop.memcpy(pDest, pSource, count * 2 * sizeof(double));
            }
        }

        public static void Memcpy(int[] destination, int[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else if (count > destination.Length || count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (int* pDest = destination)
            fixed (int* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * sizeof(int));
            }
        }

        public static void Memcpy(Complex[] destination, Complex[] source, int count = -1)
        {
            if (count < 0)
                count = Math.Min(destination.Length, source.Length);
            else if (count > destination.Length || count > source.Length)
                throw new ArgumentOutOfRangeException(nameof(count));

            fixed (Complex* pDest = destination)
            fixed (Complex* pSource = source)
            {
                Interop.memcpy(pDest, pSource, count * 2 * sizeof(double));
            }
        }

        public static void Memset(double[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else if (length > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            fixed (double* pDest = destination)
            {
                Interop.memset(pDest, value, length * sizeof(double));
            }
        }

        public static void Memset(double* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * sizeof(double));
        }

        public static void Memset(int* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * sizeof(int));
        }

        public static void Memset(Complex* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length * 2 * sizeof(double));
        }

        public static void Memset(byte* destination, byte value, int length)
        {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            Interop.memset(destination, value, length);
        }

        public static void Memset(int[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else if (length > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            fixed (int* pDest = destination)
            {
                Interop.memset(pDest, value, length * sizeof(int));
            }
        }

        public static void Memset(Complex[] destination, byte value = 0, int length = -1)
        {
            if (length < 0)
                length = destination.Length;
            else if (length > destination.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            fixed (Complex* pDest = destination)
            {
                Interop.memset(pDest, value, length * 2 * sizeof(double));
            }
        }

        public static int[] ToManagedArray(int* source, int length)
        {
            var ret = new int[length];
            fixed (int* pRet = ret)
            {
                Interop.memcpy(pRet, source, length * sizeof(int));
            }

            return ret;
        }

        public static double[] ToManagedArray(double* source, int length)
        {
            var ret = new double[length];
            fixed (double* pRet = ret)
            {
                Interop.memcpy(pRet, source, length * sizeof(double));
            }

            return ret;
        }

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