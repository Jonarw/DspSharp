using System;

namespace Filter_Win.FFTWSharp
{
    /// <summary>
    ///     Creates, stores, and destroys fftw plans
    /// </summary>
    public class FftwPlan
    {
        public FftwPlan(IntPtr handle)
        {
            this.Handle = handle;
        }

        public IntPtr Handle { get; }
        private static object FftwLock { get; } = new object();

        public static FftwPlan Dft(int rank, int[] n, FftwComplexarray input, FftwComplexarray output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft(rank, n, input.Handle, output.Handle, direction, flags));
            }
        }

        public static FftwPlan Dft1D(int n, FftwComplexarray input, FftwComplexarray output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_1d(n, input.Handle, output.Handle, direction, flags));
            }
        }

        public static FftwPlan Dft2D(int nx, int ny, FftwComplexarray input, FftwComplexarray output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_2d(nx, ny, input.Handle, output.Handle, direction, flags));
            }
        }

        public static FftwPlan Dft3D(
            int nx,
            int ny,
            int nz,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwDirection direction,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_3d(nx, ny, nz, input.Handle, output.Handle, direction, flags));
            }
        }

        public static FftwPlan DftComplexToReal(
            int rank,
            int[] n,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwDirection direction,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_c2r(rank, n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftComplexToReal1D(int n, FftwComplexarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_c2r_1d(n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftComplexToReal1D(int n, FftwComplexarray input, FftwRealarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_c2r_1d(n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftComplexToReal2D(
            int nx,
            int ny,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwDirection direction,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_c2r_2d(nx, ny, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftComplexToReal3D(
            int nx,
            int ny,
            int nz,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwDirection direction,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_c2r_3d(nx, ny, nz, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftRealToComplex(int rank, int[] n, FftwComplexarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_r2c(rank, n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftRealToComplex1D(int n, FftwComplexarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_r2c_1d(n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftRealToComplex1D(int n, FftwRealarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_r2c_1d(n, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftRealToComplex2D(int nx, int ny, FftwComplexarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_r2c_2d(nx, ny, input.Handle, output.Handle, flags));
            }
        }

        public static FftwPlan DftRealToComplex3D(int nx, int ny, int nz, FftwComplexarray input, FftwComplexarray output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.dft_r2c_3d(nx, ny, nz, input.Handle, output.Handle, flags));
            }
        }

        public void Execute()
        {
            FftwInterop.execute(this.Handle);
        }

        public static FftwPlan RealToReal(
            int rank,
            int[] n,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwRealToRealKind[] kind,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(
                    FftwInterop.r2r(
                        rank,
                        n,
                        input.Handle,
                        output.Handle,
                        kind,
                        flags));
            }
        }

        public static FftwPlan RealToReal1D(int n, FftwComplexarray input, FftwComplexarray output, FftwRealToRealKind kind, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.r2r_1d(n, input.Handle, output.Handle, kind, flags));
            }
        }

        public static FftwPlan RealToReal2D(
            int nx,
            int ny,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(FftwInterop.r2r_2d(nx, ny, input.Handle, output.Handle, kindx, kindy, flags));
            }
        }

        public static FftwPlan RealToReal3D(
            int nx,
            int ny,
            int nz,
            FftwComplexarray input,
            FftwComplexarray output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwRealToRealKind kindz,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return new FftwPlan(
                    FftwInterop.r2r_3d(
                        nx,
                        ny,
                        nz,
                        input.Handle,
                        output.Handle,
                        kindx,
                        kindy,
                        kindz,
                        flags));
            }
        }

        ~FftwPlan()
        {
            FftwInterop.destroy_plan(this.Handle);
        }
    }
}