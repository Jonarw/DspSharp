// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FftwKind.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DspSharpFftw
{
    /// <summary>
    ///     Kinds of real-to-real transforms
    /// </summary>
    public enum FftwRealToRealKind : uint
    {
        RealToHalfComplex = 0,
        HalfComplexToReal = 1,
        DiscreteHartleyTransform = 2,
        DiscreteCosineTransform1 = 3,
        DiscreteCosineTransform2 = 4,
        DiscreteCosineTransform3 = 5,
        DiscreteCosineTransform4 = 6,
        DiscreteSineTransform1 = 7,
        DiscreteSineTransform2 = 8,
        DiscreteSineTransform3 = 9,
        DiscreteSineTransform4 = 10
    }
}