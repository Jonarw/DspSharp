using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Algorithms;
using Filter.Extensions;
using MathNet.Numerics.IntegralTransforms;

namespace Filter_CrossPlatform
{
    public class MathNetFftProvider : IFftProvider
    {
        public IReadOnlyList<Complex> RealFft(IReadOnlyList<double> input, int n = -1)
        {
            IEnumerable<double> inp = input;

            if (n > 0)
            {
                inp = input.ZeroPad(n);
            }

            var values = inp.Select(d => new Complex(d, 0)).ToArray();

            if (n < 0)
            {
                n = values.Length;
            }

            Fourier.Forward(values, FourierOptions.AsymmetricScaling);
            return values.Take((n >> 1) + 1).ToReadOnlyList();
        }

        public IReadOnlyList<double> RealIfft(IReadOnlyList<Complex> input)
        {
            var inputlist = input.ToReadOnlyList();

            Complex[] values;
            if (Math.Abs(input[input.Count - 1].Imaginary) > 1e-13)
            {
                values = inputlist.Concat(inputlist.Skip(1).Reverse().ComplexConjugate()).ToArray();
            }
            else
            {
                values = inputlist.Concat(inputlist.Skip(1).Reverse().Skip(1).ComplexConjugate()).ToArray();
            }

            Fourier.Inverse(values, FourierOptions.AsymmetricScaling);
            return values.Select(c => c.Real).ToReadOnlyList();
        }
    }
}