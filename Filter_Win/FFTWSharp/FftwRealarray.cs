using System;
using System.Runtime.InteropServices;

namespace Filter_Win.FFTWSharp
{
    public class FftwRealarray
    {
        /// <summary>
        ///     Creates a new array of real numbers
        /// </summary>
        /// <param name="length">Logical length of the array</param>
        public FftwRealarray(int length)
        {
            this.Length = length;
            this.Handle = FftwInterop.malloc(this.Length * 8);
        }

        public IntPtr Handle { get; }
        public int Length { get; }

        /// <summary>
        ///     Get the data stored in the array
        /// </summary>
        public double[] GetData()
        {
            double[] ret = new double[this.Length];
            Marshal.Copy(this.Handle, ret, 0, this.Length);
            return ret;
        }

        /// <summary>
        ///     Set the data to an array of real numbers
        /// </summary>
        public void SetData(double[] data, int datalength = -1)
        {
            if (datalength < 0)
            {
                datalength = data.Length;
            }

            if (datalength > this.Length)
            {
                throw new ArgumentException("Input longer than array!");
            }

            Marshal.Copy(data, 0, this.Handle, datalength);

            if (datalength < this.Length)
            {
                double[] zeros = new double[this.Length - datalength];
                Marshal.Copy(zeros, 0, this.Handle + datalength * sizeof(double), zeros.Length);
            }
        }

        ~FftwRealarray()
        {
            FftwInterop.free(this.Handle);
        }
    }
}