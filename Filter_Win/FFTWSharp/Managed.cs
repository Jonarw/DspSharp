using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Filter.Extensions;

// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace Filter.Algorithms.FFTWSharp
{
    /// <summary>
    ///     So FFTW can manage its own memory nicely
    /// </summary>
    public class fftw_complexarray
    {
        private IntPtr handle;
        private int length;

        /// <summary>
        ///     Creates a new array of complex numbers
        /// </summary>
        /// <param name="length">Logical length of the array</param>
        public fftw_complexarray(int length)
        {
            this.length = length;
            this.handle = fftw.malloc(this.length * 16);
        }

        public IntPtr Handle
        {
            get { return this.handle; }
        }

        public int Length
        {
            get { return this.length; }
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

            if (datalength > this.length)
            {
                throw new ArgumentException("Data longer than Array");
            }

            double[] ddata = datalist.InterleaveComplex().ToArray();

            Marshal.Copy(ddata, 0, this.handle, datalength << 1);

            if (datalength < this.length)
            {
                double[] zeros = new double[(this.length - datalength) << 1];
                Marshal.Copy(zeros, 0, this.handle + sizeof(double) * (datalength << 1), zeros.Length);
            }
        }

        /// <summary>
        ///     Get the complex data stored in the array
        /// </summary>
        public IReadOnlyList<Complex> GetData()
        {
            double[] tmp = new double[this.length << 1];
            Marshal.Copy(this.handle, tmp, 0, this.length << 1);
            return tmp.UnInterleaveComplex().ToReadOnlyList();
        }

        ~fftw_complexarray()
        {
            fftw.free(this.handle);
        }
    }

    public class fftw_realarray
    {
        private IntPtr handle;

        private int length;

        /// <summary>
        ///     Creates a new array of real numbers
        /// </summary>
        /// <param name="length">Logical length of the array</param>
        public fftw_realarray(int length)
        {
            this.length = length;
            this.handle = fftw.malloc(this.length * 8);
        }

        public IntPtr Handle
        {
            get { return this.handle; }
        }

        public int Length
        {
            get { return this.length; }
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

            if (datalength > this.length)
            {
                throw new ArgumentException("Input longer than array!");
            }

            Marshal.Copy(data, 0, this.handle, datalength);

            if (datalength < this.length)
            {
                double[] zeros = new double[this.length - datalength];
                Marshal.Copy(zeros, 0, this.handle + datalength * sizeof(double), zeros.Length);
            }
        }

        /// <summary>
        ///     Get the data stored in the array
        /// </summary>
        public double[] GetData()
        {
            double[] ret = new double[this.length];
            Marshal.Copy(this.handle, ret, 0, this.length);
            return ret;
        }

        ~fftw_realarray()
        {
            fftw.free(this.handle);
        }
    }

    /// <summary>
    ///     Creates, stores, and destroys fftw plans
    /// </summary>
    public class fftw_plan
    {
        protected IntPtr handle;

        public IntPtr Handle
        {
            get { return this.handle; }
        }

        public void Execute()
        {
            fftw.execute(this.handle);
        }

        ~fftw_plan()
        {
            fftw.destroy_plan(this.handle);
        }

        #region Plan Creation

        //Complex<->Complex transforms
        public static fftw_plan dft_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_1d(n, input.Handle, output.Handle, direction, flags);
            return p;
        }

        public static fftw_plan dft_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags);
            return p;
        }

        public static fftw_plan dft_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags);
            return p;
        }

        public static fftw_plan dft(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft(rank, n, input.Handle, output.Handle, direction, flags);
            return p;
        }

        //Real->Complex transforms
        public static fftw_plan dft_r2c_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_r2c_1d(n, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_r2c_1d(int n, fftw_realarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_r2c_1d(n, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_r2c_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_r2c_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_r2c(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_r2c(rank, n, input.Handle, output.Handle, flags);
            return p;
        }

        //Complex->Real
        public static fftw_plan dft_c2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_c2r_1d(n, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_c2r_1d(int n, fftw_complexarray input, fftw_realarray output, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_c2r_1d(n, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_c2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_c2r_3d(int nx, int ny, int nz, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags);
            return p;
        }

        public static fftw_plan dft_c2r(int rank, int[] n, fftw_complexarray input, fftw_complexarray output, fftw_direction direction, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.dft_c2r(rank, n, input.Handle, output.Handle, flags);
            return p;
        }

        //Real<->Real
        public static fftw_plan r2r_1d(int n, fftw_complexarray input, fftw_complexarray output, fftw_kind kind, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.r2r_1d(n, input.Handle, output.Handle, kind, flags);
            return p;
        }

        public static fftw_plan r2r_2d(int nx, int ny, fftw_complexarray input, fftw_complexarray output, fftw_kind kindx, fftw_kind kindy, fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags);
            return p;
        }

        public static fftw_plan r2r_3d(
            int nx,
            int ny,
            int nz,
            fftw_complexarray input,
            fftw_complexarray output,
            fftw_kind kindx,
            fftw_kind kindy,
            fftw_kind kindz,
            fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.r2r_3d(
                nx,
                ny,
                nz,
                input.Handle,
                output.Handle,
                kindx,
                kindy,
                kindz,
                flags);
            return p;
        }

        public static fftw_plan r2r(
            int rank,
            int[] n,
            fftw_complexarray input,
            fftw_complexarray output,
            fftw_kind[] kind,
            fftw_flags flags)
        {
            fftw_plan p = new fftw_plan();
            p.handle = fftw.r2r(
                rank,
                n,
                input.Handle,
                output.Handle,
                kind,
                flags);
            return p;
        }

        #endregion
    }
}