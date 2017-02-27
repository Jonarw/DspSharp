using System.Numerics;

namespace Filter.Extensions
{
    public static unsafe class UnsafeConversions
    {
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