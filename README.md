# DspSharp
API for digital signal processing in C#.

## Binaries
DspSharp on nuget: https://www.nuget.org/packages/DspSharp/

DspSharpFftw on nuget: https://www.nuget.org/packages/DspSharpFftw/

DspSharpAsio on nuget: https://www.nuget.org/packages/DspSharpAsio/

## Features
### DspSharp
#### Algorithms
- standard vector operations (addition, multiplication...)
- special vector operations (shift...)
- window synthesis (Hann, Blackman...)
- some mathematical functions (root finding, modified bessel function)
- fast fourier transform, using fftw as backend
- fast convolution, cross correlation
- evaluation of coefficient-based zero/pole filters
- synthesis of standard signals
- signal interpolation
- signal analysis (group delay...)

#### signal / filter model
Provides an object-oriented programming model for signals and filters.

### DspSharpFftw
C# Wrapper for the fftw library.

### DspSharpAsio
ASIO interface.
