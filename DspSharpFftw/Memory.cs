using System.Collections.Generic;
using System.Numerics;

namespace DspSharpFftw
{
    public static unsafe class Memory
    {
        public static void Copy(IReadOnlyList<Complex> source, Complex* destination)
        {
            var c = 0;
            for (var i = 0; i < source.Count; i++)
            {
                destination[c++] = source[i];
            }
        }

        public static void Copy(IReadOnlyList<double> source, double* destination)
        {
            for (var i = 0; i < source.Count; i++)
            {
                destination[i] = source[i];
            }
        }

        public static void Copy(Complex* source, IList<Complex> destination)
        {
            for (var i = 0; i < destination.Count; i++)
            {
                destination[i] = source[i];
            }
        }

        public static void Copy(double* source, IList<double> destination)
        {
            for (var i = 0; i < destination.Count; i++)
            {
                destination[i] = source[i];
            }
        }

        public static void Clear(double* target, int count)
        {
            for (var i = 0; i < count; i++)
            {
                target[i] = 0;
            }
        }

        public static void Clear(Complex* target, int count)
        {
            for (var i = 0; i < count; i++)
            {
                target[i] = 0;
            }
        }
    }
}