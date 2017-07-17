using System;
using System.Numerics;
using DspSharp.Algorithms;

namespace DspSharpFftw.Pointers
{
    public unsafe struct ManagedDoublePointer
    {
        public ManagedDoublePointer(double* pointer, int length)
        {
            this.Pointer = pointer;
            this.Length = length;
        }

        public double* Pointer { get; }
        public int Length { get; }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                return this.Pointer[index];
            }
            set
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                this.Pointer[index] = value;
            }
        }
    }

    public unsafe struct ManagedComplexPointer
    {
        public ManagedComplexPointer(Complex* pointer, int length)
        {
            this.Pointer = pointer;
            this.Length = length;
        }

        public Complex* Pointer { get; }
        public int Length { get; }

        public Complex this[int index]
        {
            get
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                return this.Pointer[index];
            }
            set
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                this.Pointer[index] = value;
            }
        }
    }

    public unsafe class ManagedPointerD : IDisposable
    {
        public ManagedPointerD(int length)
        {
            this.Pointer = (double*)FftwInterop.Malloc(length * sizeof(double));
            this.Length = length;
            this.OwnMemory = true;
        }

        public ManagedPointerD(double[] array)
        {
            this.Pointer = (double*)FftwInterop.Malloc(array.Length * sizeof(double));
            this.Length = array.Length;
            Unsafe.Memcpy(this.Pointer, array);
            this.OwnMemory = true;
        }

        public ManagedPointerD(double* pointer, int length)
        {
            this.OwnMemory = false;
            this.Pointer = pointer;
            this.Length = length;
        }

        public double this[int index]
        {
            get
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                return this.Pointer[index];
            }
            set
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                this.Pointer[index] = value;
            }
        }

        public int Length { get; }
        public double* Pointer { get; }
        private bool OwnMemory { get; }

        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public double[] ToManagedArray()
        {
            return Unsafe.ToManagedArray(this.Pointer, this.Length);
        }

        private void ReleaseUnmanagedResources()
        {
            if (this.OwnMemory)
                FftwInterop.Free(this.Pointer);
        }

        ~ManagedPointerD()
        {
            this.ReleaseUnmanagedResources();
        }
    }

    public unsafe class ManagedPointerC
    {
        public ManagedPointerC(int length)
        {
            this.Pointer = (Complex*)FftwInterop.Malloc(length * 2 * sizeof(double));
            this.Length = length;
            this.OwnMemory = true;
        }

        public ManagedPointerC(Complex[] array)
        {
            this.Pointer = (Complex*)FftwInterop.Malloc(array.Length * 2 * sizeof(double));
            this.Length = array.Length;
            Unsafe.Memcpy(this.Pointer, array);
            this.OwnMemory = true;
        }

        public ManagedPointerC(Complex* pointer, int length)
        {
            this.OwnMemory = false;
            this.Pointer = pointer;
            this.Length = length;
        }

        public Complex this[int index]
        {
            get
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                return this.Pointer[index];
            }
            set
            {
                if (index < 0 || index > this.Length)
                    throw new ArgumentOutOfRangeException();

                this.Pointer[index] = value;
            }
        }

        public int Length { get; }
        public Complex* Pointer { get; }
        private bool OwnMemory { get; }

        public void Dispose()
        {
            this.ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        public Complex[] ToManagedArray()
        {
            return Unsafe.ToManagedArray(this.Pointer, this.Length);
        }

        private void ReleaseUnmanagedResources()
        {
            if (this.OwnMemory)
                FftwInterop.Free(this.Pointer);
        }

        ~ManagedPointerC()
        {
            this.ReleaseUnmanagedResources();
        }
    }
}