using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Filter.Extensions;

// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace Filter_Win.FFTWSharp
{
    /// <summary>
    ///     So FFTW can manage its own memory nicely
    /// </summary>
    public class FftwComplexarray
    {
        /// <summary>
        ///     Creates a new array of complex numbers
        /// </summary>
        /// <param name="length">Logical length of the array</param>
        public FftwComplexarray(int length)
        {
            this.Length = length;
            this.Handle = FftwInterop.malloc(this.Length * sizeof(double) * 2);
        }

        public IntPtr Handle { get; }
        public int Length { get; }

        /// <summary>
        ///     Get the complex data stored in the array
        /// </summary>
        public IReadOnlyList<Complex> GetData()
        {
            double[] tmp = new double[this.Length << 1];
            Marshal.Copy(this.Handle, tmp, 0, this.Length << 1);
            return tmp.UnInterleaveComplex().ToReadOnlyList();
        }

        /// <summary>
        ///     Set complex data to an array of complex numbers
        /// </summary>
        public void SetData(IEnumerable<Complex> data, int datalength = -1)
        {
            var datalist = data.ToReadOnlyList();

            if (datalength < 0)
            {
                datalength = datalist.Count;
            }

            if (datalength > this.Length)
            {
                throw new ArgumentException("Data longer than Array");
            }

            var ddata = new double[datalist.Count * 2];

            for (int i = 0; i < datalist.Count; i++)
            {
                ddata[2 * i] = datalist[i].Real;
                ddata[2 * i + 1] = datalist[i].Imaginary;
            }

            Marshal.Copy(ddata, 0, this.Handle, datalength << 1);

            if (datalength < this.Length)
            {
                double[] zeros = new double[(this.Length - datalength) << 1];
                Marshal.Copy(zeros, 0, this.Handle + sizeof(double) * (datalength << 1), zeros.Length);
            }
        }

        ~FftwComplexarray()
        {
            FftwInterop.free(this.Handle);
        }
    }
}