// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftwInterop.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace DspSharpFftw
{
    /// <summary>
    ///     Contains the Basic Interface FFTW functions for double-precision (double) operations
    /// </summary>
    public static unsafe class FftwInterop
    {
        private const string Fftw32LibraryName = "libfftw3-3-32";
        private const string Fftw64LibraryName = "libfftw3-3-64";
        private static readonly bool Is64Bit = sizeof(IntPtr) == 8;
        private static readonly object FftwLock = new object();
        public static string WisdomPath { get; } = "fftwisdom";

        static FftwInterop()
        {
            try
            {
                ImportWisdomFromFilename(WisdomPath);
            }
            catch (Exception)
            {
                // wisdom file could not be read...
            }
        }

        /// <summary>
        ///     Exports the accumulated wisdom to a file for later use.
        /// </summary>
        public static void ExportWisdom()
        {
            //var fi = new FileInfo(WisdomPath);
            //fi.Directory?.Create();
            ExportWisdomToFilename(WisdomPath);
        }

        /// <summary>
        ///     Clears all memory used by FFTW, resets it to initial state. Does not replace destroy_plan and free
        /// </summary>
        /// <remarks>
        ///     After calling fftw_cleanup, all existing plans become undefined, and you should not
        ///     attempt to execute them nor to destroy them. You can however create and execute/destroy new plans,
        ///     in which case FFTW starts accumulating wisdom information again.
        ///     fftw_cleanup does not deallocate your plans; you should still call fftw_destroy_plan for this purpose.
        /// </remarks>
        public static void Cleanup()
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_cleanup64();
                else
                    fftw_cleanup32();
            }
        }

        /// <summary>
        ///     Deallocates an FFTW plan and all associated resources
        /// </summary>
        /// <param name="plan">Pointer to the plan to release</param>
        public static void DestroyPlan(void* plan)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_destroy_plan64(plan);
                else
                    fftw_destroy_plan32(plan);
            }
        }

        /// <summary>
        ///     Executes an FFTW plan, provided that the input and output arrays still exist
        /// </summary>
        /// <param name="plan">Pointer to the plan to execute</param>
        public static void Execute(void* plan)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_execute64(plan);
                else
                    fftw_execute32(plan);
            }
        }

        /// <summary>
        ///     Executes an FFTW plan using the specified input and output locations.
        /// </summary>
        /// <param name="plan">Pointer to the plan to execute</param>
        /// <param name="input">Pointer to the input array.</param>
        /// <param name="output">Pointer to the output array.</param>
        /// <remarks>execute (and equivalents) is the only function in FFTW guaranteed to be thread-safe.</remarks>
        public static void ExecuteDft(void* plan, void* input, void* output)
        {
            if (Is64Bit)
                fftw_execute_dft64(plan, input, output);
            else
                fftw_execute_dft32(plan, input, output);
        }

        /// <summary>
        ///     Executes an FFTW plan using the specified input and output locations.
        /// </summary>
        /// <param name="plan">Pointer to the plan to execute</param>
        /// <param name="input">Pointer to the input array.</param>
        /// <param name="output">Pointer to the output array.</param>
        /// <remarks>execute (and equivalents) is the only function in FFTW guaranteed to be thread-safe.</remarks>
        public static void ExecuteDftC2R(void* plan, void* input, void* output)
        {
            if (Is64Bit)
                fftw_execute_dft_c2r64(plan, input, output);
            else
                fftw_execute_dft_c2r32(plan, input, output);
        }

        /// <summary>
        ///     Executes an FFTW plan using the specified input and output locations.
        /// </summary>
        /// <param name="plan">Pointer to the plan to execute</param>
        /// <param name="input">Pointer to the input array.</param>
        /// <param name="output">Pointer to the output array.</param>
        /// <remarks>execute (and equivalents) is the only function in FFTW guaranteed to be thread-safe.</remarks>
        public static void ExecuteDftR2C(void* plan, void* input, void* output)
        {
            if (Is64Bit)
                fftw_execute_dft_r2c64(plan, input, output);
            else
                fftw_execute_dft_r2c32(plan, input, output);
        }

        /// <summary>
        ///     Exports the accumulated Wisdom to the provided filename
        /// </summary>
        /// <param name="filename">The target filename</param>
        public static void ExportWisdomToFilename(string filename)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_export_wisdom_to_filename64(filename);
                else
                    fftw_export_wisdom_to_filename32(filename);
            }
        }

        /// <summary>
        ///     Returns (approximately) the number of flops used by a certain plan
        /// </summary>
        /// <param name="plan">The plan to measure</param>
        /// <param name="add">Reference to double to hold number of adds</param>
        /// <param name="mul">Reference to double to hold number of muls</param>
        /// <param name="fma">Reference to double to hold number of fmas (fused multiply-add)</param>
        /// <remarks>Total flops ~= add+mul+2*fma or add+mul+fma if fma is supported</remarks>
        public static void Flops(void* plan, ref double add, ref double mul, ref double fma)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_flops64(plan, ref add, ref mul, ref fma);
                else
                    fftw_flops32(plan, ref add, ref mul, ref fma);
            }
        }

        /// <summary>
        ///     Deallocates memory allocated by FFTW malloc
        /// </summary>
        /// <param name="mem">Pointer to memory to release</param>
        public static void Free(void* mem)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_free64(mem);
                else
                    fftw_free32(mem);
            }
        }

        /// <summary>
        ///     Imports Wisdom from provided filename
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        public static void ImportWisdomFromFilename(string filename)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_import_wisdom_from_filename64(filename);
                else
                    fftw_import_wisdom_from_filename32(filename);
            }
        }

        /// <summary>
        ///     Allocates FFTW-optimized unmanaged memory
        /// </summary>
        /// <param name="length">Amount to allocate, in bytes</param>
        /// <returns>Pointer to allocated memory</returns>
        public static void* Malloc(int length)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_malloc64(length) : fftw_malloc32(length);
            }
        }

        /// <summary>
        ///     Creates a plan for an n-dimensional complex-to-complex DFT
        /// </summary>
        /// <param name="rank">Number of dimensions</param>
        /// <param name="n">Array containing the logical size along each dimension</param>
        /// <param name="direction">Specifies the direction of the transform</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDft(int rank, int[] n, void* input, void* output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft64(rank, n, input, output, direction, flags) : fftw_plan_dft32(rank, n, input, output, direction, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 1-dimensional complex-to-complex DFT
        /// </summary>
        /// <param name="n">The logical size of the transform</param>
        /// <param name="direction">Specifies the direction of the transform</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDft1D(int n, void* input, void* output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_1d64(n, input, output, direction, flags) : fftw_plan_dft_1d32(n, input, output, direction, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 2-dimensional complex-to-complex DFT
        /// </summary>
        /// <param name="nx">The logical size of the transform along the first dimension</param>
        /// <param name="ny">The logical size of the transform along the second dimension</param>
        /// <param name="direction">Specifies the direction of the transform</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDft2D(int nx, int ny, void* input, void* output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit
                    ? fftw_plan_dft_2d64(nx, ny, input, output, direction, flags)
                    : fftw_plan_dft_2d32(nx, ny, input, output, direction, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 3-dimensional complex-to-complex DFT
        /// </summary>
        /// <param name="nx">The logical size of the transform along the first dimension</param>
        /// <param name="ny">The logical size of the transform along the second dimension</param>
        /// <param name="nz">The logical size of the transform along the third dimension</param>
        /// <param name="direction">Specifies the direction of the transform</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDft3D(int nx, int ny, int nz, void* input, void* output, FftwDirection direction, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit
                    ? fftw_plan_dft_3d64(nx, ny, nz, input, output, direction, flags)
                    : fftw_plan_dft_3d32(nx, ny, nz, input, output, direction, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for an n-dimensional complex-to-real DFT
        /// </summary>
        /// <param name="rank">Number of dimensions</param>
        /// <param name="n">Array containing the number of REAL (output) elements along each dimension</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftC2R(int rank, int[] n, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_c2r64(rank, n, input, output, flags) : fftw_plan_dft_c2r32(rank, n, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 1-dimensional complex-to-real DFT
        /// </summary>
        /// <param name="n">Number of REAL (output) elements in the transform</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftC2R1D(int n, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_c2r_1d64(n, input, output, flags) : fftw_plan_dft_c2r_1d32(n, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 2-dimensional complex-to-real DFT
        /// </summary>
        /// <param name="nx">Number of REAL (output) elements in the transform along the first dimension</param>
        /// <param name="ny">Number of REAL (output) elements in the transform along the second dimension</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftC2R2D(int nx, int ny, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_c2r_2d64(nx, ny, input, output, flags) : fftw_plan_dft_c2r_2d32(nx, ny, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 3-dimensional complex-to-real DFT
        /// </summary>
        /// <param name="nx">Number of REAL (output) elements in the transform along the first dimension</param>
        /// <param name="ny">Number of REAL (output) elements in the transform along the second dimension</param>
        /// <param name="nz">Number of REAL (output) elements in the transform along the third dimension</param>
        /// <param name="input">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftC2R3D(int nx, int ny, int nz, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_c2r_3d64(nx, ny, nz, input, output, flags) : fftw_plan_dft_c2r_3d32(nx, ny, nz, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for an n-dimensional real-to-complex DFT
        /// </summary>
        /// <param name="rank">Number of dimensions</param>
        /// <param name="n">Array containing the number of REAL (input) elements along each dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftR2C(int rank, int[] n, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_r2c64(rank, n, input, output, flags) : fftw_plan_dft_r2c32(rank, n, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 1-dimensional real-to-complex DFT
        /// </summary>
        /// <param name="n">Number of REAL (input) elements in the transform</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftR2C1D(int n, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_r2c_1d64(n, input, output, flags) : fftw_plan_dft_r2c_1d32(n, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 2-dimensional real-to-complex DFT
        /// </summary>
        /// <param name="nx">Number of REAL (input) elements in the transform along the first dimension</param>
        /// <param name="ny">Number of REAL (input) elements in the transform along the second dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftR2C2D(int nx, int ny, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_r2c_2d64(nx, ny, input, output, flags) : fftw_plan_dft_r2c_2d32(nx, ny, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 3-dimensional real-to-complex DFT
        /// </summary>
        /// <param name="nx">Number of REAL (input) elements in the transform along the first dimension</param>
        /// <param name="ny">Number of REAL (input) elements in the transform along the second dimension</param>
        /// <param name="nz">Number of REAL (input) elements in the transform along the third dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 16-byte complex numbers</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanDftR2C3D(int nx, int ny, int nz, void* input, void* output, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_dft_r2c_3d64(nx, ny, nz, input, output, flags) : fftw_plan_dft_r2c_3d32(nx, ny, nz, input, output, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for an n-dimensional real-to-real DFT
        /// </summary>
        /// <param name="rank">Number of dimensions</param>
        /// <param name="n">Array containing the number of elements in the transform along each dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="kind">An array containing the kind of real-to-real transform to compute along each dimension</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanR2R(int rank, int[] n, void* input, void* output, FftwRealToRealKind[] kind, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_r2r64(rank, n, input, output, kind, flags) : fftw_plan_r2r32(rank, n, input, output, kind, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 1-dimensional real-to-real DFT
        /// </summary>
        /// <param name="n">Number of elements in the transform</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="kind">The kind of real-to-real transform to compute</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanR2R1D(int n, void* input, void* output, FftwRealToRealKind kind, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit ? fftw_plan_r2r_1d64(n, input, output, kind, flags) : fftw_plan_r2r_1d32(n, input, output, kind, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 2-dimensional real-to-real DFT
        /// </summary>
        /// <param name="nx">Number of elements in the transform along the first dimension</param>
        /// <param name="ny">Number of elements in the transform along the second dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="kindx">The kind of real-to-real transform to compute along the first dimension</param>
        /// <param name="kindy">The kind of real-to-real transform to compute along the second dimension</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanR2R2D(int nx, int ny, void* input, void* output, FftwRealToRealKind kindx, FftwRealToRealKind kindy, FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit
                    ? fftw_plan_r2r_2d64(nx, ny, input, output, kindx, kindy, flags)
                    : fftw_plan_r2r_2d32(nx, ny, input, output, kindx, kindy, flags);
            }
        }

        /// <summary>
        ///     Creates a plan for a 3-dimensional real-to-real DFT
        /// </summary>
        /// <param name="nx">Number of elements in the transform along the first dimension</param>
        /// <param name="ny">Number of elements in the transform along the second dimension</param>
        /// <param name="nz">Number of elements in the transform along the third dimension</param>
        /// <param name="input">Pointer to an array of 8-byte real numbers</param>
        /// <param name="output">Pointer to an array of 8-byte real numbers</param>
        /// <param name="kindx">The kind of real-to-real transform to compute along the first dimension</param>
        /// <param name="kindy">The kind of real-to-real transform to compute along the second dimension</param>
        /// <param name="kindz">The kind of real-to-real transform to compute along the third dimension</param>
        /// <param name="flags">Flags that specify the behavior of the planner</param>
        public static void* PlanR2R3D(
            int nx,
            int ny,
            int nz,
            void* input,
            void* output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwRealToRealKind kindz,
            FftwFlags flags)
        {
            lock (FftwLock)
            {
                return Is64Bit
                    ? fftw_plan_r2r_3d64(nx, ny, nz, input, output, kindx, kindy, kindz, flags)
                    : fftw_plan_r2r_3d32(nx, ny, nz, input, output, kindx, kindy, kindz, flags);
            }
        }

        /// <summary>
        ///     Outputs a "nerd-readable" version of the specified plan to stdout
        /// </summary>
        /// <param name="plan">The plan to output</param>
        public static void PrintPlan(void* plan)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_print_plan64(plan);
                else
                    fftw_print_plan32(plan);
            }
        }

        /// <summary>
        ///     Sets the maximum time that can be used by the planner.
        /// </summary>
        /// <param name="seconds">Maximum time, in seconds.</param>
        /// <remarks>
        ///     This function instructs FFTW to spend at most seconds seconds (approximately) in the planner.
        ///     If seconds == -1.0 (the default value), then planning time is unbounded.
        ///     Otherwise, FFTW plans with a progressively wider range of algorithms until the the given time limit is
        ///     reached or the given range of algorithms is explored, returning the best available plan. For example,
        ///     specifying fftw_flags.Patient first plans in Estimate mode, then in Measure mode, then finally (time
        ///     permitting) in Patient. If fftw_flags.Exhaustive is specified instead, the planner will further progress to
        ///     Exhaustive mode.
        /// </remarks>
        public static void SetTimelimit(double seconds)
        {
            lock (FftwLock)
            {
                if (Is64Bit)
                    fftw_set_timelimit64(seconds);
                else
                    fftw_set_timelimit32(seconds);
            }
        }

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_cleanup", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_cleanup32();

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_cleanup", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_cleanup64();

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_destroy_plan", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_destroy_plan32(void* plan);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_destroy_plan", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_destroy_plan64(void* plan);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_execute_dft_c2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft_c2r32(void* plan, void* input, void* output);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_execute_dft_c2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft_c2r64(void* plan, void* input, void* output);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_execute_dft_r2c", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft_r2c32(void* plan, void* input, void* output);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_execute_dft_r2c", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft_r2c64(void* plan, void* input, void* output);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_execute_dft", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft32(void* plan, void* input, void* output);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_execute_dft", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute_dft64(void* plan, void* input, void* output);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_execute", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute32(void* plan);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_execute", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_execute64(void* plan);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_export_wisdom_to_filename", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl
            )]
        private static extern void fftw_export_wisdom_to_filename32(string filename);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_export_wisdom_to_filename", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl
            )]
        private static extern void fftw_export_wisdom_to_filename64(string filename);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_flops", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_flops32(void* plan, ref double add, ref double mul, ref double fma);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_flops", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_flops64(void* plan, ref double add, ref double mul, ref double fma);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_free", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_free32(void* mem);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_free", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_free64(void* mem);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_import_wisdom_from_filename", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_import_wisdom_from_filename32(string filename);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_import_wisdom_from_filename", ExactSpelling = true,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_import_wisdom_from_filename64(string filename);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_malloc", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_malloc32(int length);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_malloc", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_malloc64(int length);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_1d32(int n, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_1d64(int n, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_2d32(int nx, int ny, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_2d64(int nx, int ny, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_3d32(int nx, int ny, int nz, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_3d64(int nx, int ny, int nz, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_c2r_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_1d32(int n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_c2r_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_1d64(int n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_c2r_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_2d32(int nx, int ny, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_c2r_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_2d64(int nx, int ny, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_c2r_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_3d32(int nx, int ny, int nz, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_c2r_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r_3d64(int nx, int ny, int nz, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_c2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r32(int rank, int[] n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_c2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_c2r64(int rank, int[] n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_r2c_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_1d32(int n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_r2c_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_1d64(int n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_r2c_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_2d32(int nx, int ny, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_r2c_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_2d64(int nx, int ny, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_r2c_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_3d32(int nx, int ny, int nz, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_r2c_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c_3d64(int nx, int ny, int nz, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft_r2c", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c32(int rank, int[] n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft_r2c", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft_r2c64(int rank, int[] n, void* input, void* output, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_dft", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft32(int rank, int[] n, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_dft", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_dft64(int rank, int[] n, void* input, void* output, FftwDirection direction, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_r2r_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_1d32(int n, void* input, void* output, FftwRealToRealKind kind, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_r2r_1d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_1d64(int n, void* input, void* output, FftwRealToRealKind kind, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_r2r_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_2d32(
            int nx,
            int ny,
            void* input,
            void* output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_r2r_2d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_2d64(
            int nx,
            int ny,
            void* input,
            void* output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_r2r_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_3d32(
            int nx,
            int ny,
            int nz,
            void* input,
            void* output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwRealToRealKind kindz,
            FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_r2r_3d", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r_3d64(
            int nx,
            int ny,
            int nz,
            void* input,
            void* output,
            FftwRealToRealKind kindx,
            FftwRealToRealKind kindy,
            FftwRealToRealKind kindz,
            FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_plan_r2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r32(int rank, int[] n, void* input, void* output, FftwRealToRealKind[] kind, FftwFlags flags);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_plan_r2r", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void* fftw_plan_r2r64(int rank, int[] n, void* input, void* output, FftwRealToRealKind[] kind, FftwFlags flags);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_print_plan", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_print_plan32(void* plan);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_print_plan", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_print_plan64(void* plan);

        [DllImport(Fftw32LibraryName, EntryPoint = "fftw_set_timelimit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_set_timelimit32(double seconds);

        [DllImport(Fftw64LibraryName, EntryPoint = "fftw_set_timelimit", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void fftw_set_timelimit64(double seconds);
    }
}