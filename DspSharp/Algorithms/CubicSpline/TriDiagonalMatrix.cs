//
// Author: Ryan Seghers
//
// Copyright (C) 2013-2014 Ryan Seghers
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the irrevocable, perpetual, worldwide, and royalty-free
// rights to use, copy, modify, merge, publish, distribute, sublicense, 
// display, perform, create derivative works from and/or sell copies of 
// the Software, both in source and object code form, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace DspSharp.Algorithms.CubicSpline
{
    /// <summary>
    ///     A tri-diagonal matrix has non-zero entries only on the main diagonal, the diagonal above the main (super), and the
    ///     diagonal below the main (sub).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is based on the wikipedia article: http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
    ///     </para>
    ///     <para>
    ///         The entries in the matrix on a particular row are A[i], B[i], and C[i] where i is the row index.
    ///         B is the main diagonal, and so for an NxN matrix B is length N and all elements are used.
    ///         So for row 0, the first two values are B[0] and C[0].
    ///         And for row N-1, the last two values are A[N-1] and B[N-1].
    ///         That means that A[0] is not actually on the matrix and is therefore never used, and same with C[N-1].
    ///     </para>
    /// </remarks>
    public class TriDiagonalMatrixF
    {
        /// <summary>
        ///     The values for the sub-diagonal. A[0] is never used.
        /// </summary>
        public double[] A;

        /// <summary>
        ///     The values for the main diagonal.
        /// </summary>
        public double[] B;

        /// <summary>
        ///     The values for the super-diagonal. C[C.Length-1] is never used.
        /// </summary>
        public double[] C;

        /// <summary>
        ///     Construct an NxN matrix.
        /// </summary>
        public TriDiagonalMatrixF(int n)
        {
            this.A = new double[n];
            this.B = new double[n];
            this.C = new double[n];
        }

        /// <summary>
        ///     Indexer. Setter throws an exception if you try to set any not on the super, main, or sub diagonals.
        /// </summary>
        public double this[int row, int col]
        {
            get
            {
                var di = row - col;

                if (di == 0)
                    return this.B[row];
                if (di == -1)
                    return this.C[row];
                if (di == 1)
                    return this.A[row];
                return 0;
            }
            set
            {
                var di = row - col;

                if (di == 0)
                    this.B[row] = value;
                else
                {
                    if (di == -1)
                        this.C[row] = value;
                    else
                    {
                        if (di == 1)
                            this.A[row] = value;
                        else
                            throw new ArgumentException("Only the main, super, and sub diagonals can be set.");
                    }
                }
            }
        }

        /// <summary>
        ///     The width and height of this matrix.
        /// </summary>
        public int N
        {
            get { return this.A?.Length ?? 0; }
        }

        /// <summary>
        ///     Solve the system of equations this*x=d given the specified d.
        /// </summary>
        /// <remarks>
        ///     Uses the Thomas algorithm described in the wikipedia article:
        ///     http://en.wikipedia.org/wiki/Tridiagonal_matrix_algorithm
        ///     Not optimized. Not destructive.
        /// </remarks>
        /// <param name="d">Right side of the equation.</param>
        public double[] Solve(double[] d)
        {
            var n = this.N;

            if (d.Length != n)
                throw new ArgumentException("The input d is not the same size as this matrix.");

            // cPrime
            var cPrime = new double[n];
            cPrime[0] = this.C[0] / this.B[0];

            for (var i = 1; i < n; i++)
            {
                cPrime[i] = this.C[i] / (this.B[i] - cPrime[i - 1] * this.A[i]);
            }

            // dPrime
            var dPrime = new double[n];
            dPrime[0] = d[0] / this.B[0];

            for (var i = 1; i < n; i++)
            {
                dPrime[i] = (d[i] - dPrime[i - 1] * this.A[i]) / (this.B[i] - cPrime[i - 1] * this.A[i]);
            }

            // Back substitution
            var x = new double[n];
            x[n - 1] = dPrime[n - 1];

            for (var i = n - 2; i >= 0; i--)
            {
                x[i] = dPrime[i] - cPrime[i] * x[i + 1];
            }

            return x;
        }
    }
}