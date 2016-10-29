using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Filter.Extensions;

namespace Filter.Algorithms
{
    /// <summary>
    ///     Static container class for acoustic algorithms.
    /// </summary>
    public sealed class Dsp
    {
        /// <summary>
        ///     Enumeration of all supported slope-generating methods.
        /// </summary>
        public enum SlopeModes
        {
            /// <summary>
            ///     The slope consists of a straight line in logarithmic scale.
            /// </summary>
            Straight,

            /// <summary>
            ///     The slope consists of a smooth raised-cosine line scale.
            /// </summary>
            Smooth
        }

        /// <summary>
        ///     Converts a real-valued array to a zero-phase complex-valued array.
        /// </summary>
        /// <param name="amplitude">The amplitude array.</param>
        /// <returns>A new complex array of the same length as <paramref name="amplitude" /> containing the result.</returns>
        public static IEnumerable<Complex> AmplitudeToComplex(IEnumerable<double> amplitude)
        {
            return amplitude.Select(d => new Complex(d, 0.0));
        }

        /// <summary>
        ///     Applies a time delay to a complex frequency spectrum by representing the constant group delay in the complex phase
        ///     information.
        /// </summary>
        /// <param name="frequencies">The frequencies of the complex spectrum.</param>
        /// <param name="amplitudes">
        ///     The complex amplitudes of the spectrum. Must be the same length as
        ///     <paramref name="frequencies" />.
        /// </param>
        /// <param name="delay">The delay to be applied to the spectrum. Can be negative.</param>
        /// <returns>
        ///     A new array of the same length as <paramref name="frequencies" /> and <paramref name="amplitudes" />
        ///     containing the result.
        /// </returns>
        public static IEnumerable<Complex> ApplyDelayToSpectrum(IEnumerable<double> frequencies, IEnumerable<Complex> amplitudes, double delay)
        {
            var factor = Complex.ImaginaryOne * 2 * Math.PI * delay;
            return frequencies.Zip(amplitudes, (f, a) => Complex.Exp(factor * f) * a);
        }

        public static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static IReadOnlyList<Complex> ApproximateSpectrumOfInfiniteSignal(IEnumerable<double> signal, double energyRatio = 0.00001, int initialLength = 1024, int maximumLength = 524288)
        {
            var currentLength = initialLength / 2;

            // ReSharper disable PossibleMultipleEnumeration - unavoidable with infinite signal
            while (signal.Skip(currentLength).Take(currentLength).CalculateEnergy() / signal.Take(currentLength).CalculateEnergy() > energyRatio)
            {
                currentLength *= 2;
                if (currentLength > maximumLength)
                {
                    break;
                }
            }

            return Fft.RealFft(signal.Take(currentLength));
            // ReSharper restore PossibleMultipleEnumeration
        } 

        /// <summary>
        ///     Calculates the group delay of a system for a given phase response.
        /// </summary>
        /// <param name="phase">The phase values.</param>
        /// <param name="frequencies">
        ///     The frequencies the phase values correspond to. Must be the same length as
        ///     <paramref name="phase" />.
        /// </param>
        /// <returns>
        ///     An array of the same length as <paramref name="phase" /> and <paramref name="frequencies" /> containing the
        ///     result (in seconds).
        /// </returns>
        public static IEnumerable<double> CalculateGroupDelay(IEnumerable<double> phase, IEnumerable<double> frequencies)
        {
            var phaselist = phase.ToReadOnlyList();
            var frequencylist = frequencies.ToReadOnlyList();

            if (phaselist.Count != frequencylist.Count)
            {
                throw new ArgumentException();
            }

            yield return (phaselist[0] - phaselist[1]) / (2 * Math.PI * (frequencylist[1] - frequencylist[0]));
            for (var c = 1; c <= phaselist.Count - 2; c++)
            {
                yield return (phaselist[c - 1] - phaselist[c + 1]) / (2 * Math.PI * (frequencylist[c + 1] - frequencylist[c - 1]));
            }

            yield return (phaselist[phaselist.Count - 2] - phaselist[phaselist.Count - 1]) /
                         (2 * Math.PI * (frequencylist[frequencylist.Count - 1] - frequencylist[frequencylist.Count - 2]));
        }

        /// <summary>
        ///     Generates a slope.
        /// </summary>
        /// <param name="frequencies">The frequencies at which the slope is evaluated.</param>
        /// <param name="from">The start frequency.</param>
        /// <param name="to">The stop frequency.</param>
        /// <param name="gain">The gain.</param>
        /// <param name="mode">The slope mode.</param>
        /// <param name="logarithmicFrequencies">If set to <c>true</c> the generation is done on a logarithmic frequency scale.</param>
        /// <param name="logarithmicAmplitudes">if set to <c>true</c> the generation is done on a logarithmic amplitude scale.</param>
        /// <returns>The result.</returns>
        public static IEnumerable<double> CalculateSlope(
            IEnumerable<double> frequencies,
            double from,
            double to,
            double gain,
            SlopeModes mode = SlopeModes.Smooth,
            bool logarithmicFrequencies = true,
            bool logarithmicAmplitudes = true)
        {
            IReadOnlyList<double> actualFrequencies;
            double actualTo, actualFrom;

            double actualToGain, actualFromGain;
            Func<double, double> smoothSlope = x => -0.5 * (Math.Cos(Math.PI * x) + 1);

            if (logarithmicFrequencies)
            {
                actualFrequencies = frequencies.Log(10).ToReadOnlyList();
                actualTo = Math.Log10(to);
                actualFrom = Math.Log10(from);
            }
            else
            {
                actualFrequencies = frequencies.ToReadOnlyList();
                actualTo = to;
                actualFrom = from;
            }

            if (logarithmicAmplitudes)
            {
                actualFromGain = 0;
                actualToGain = Math.Log10(gain);
            }
            else
            {
                actualFromGain = 1;
                actualToGain = gain;
            }

            var delta = actualTo - actualFrom;
            var deltaGain = actualToGain - actualFromGain;

            for (var c = 0; c <= actualFrequencies.Count - 1; c++)
            {
                double actualGain;
                if (actualFrequencies[c] < actualFrom)
                {
                    actualGain = actualFromGain;
                }
                else if (actualFrequencies[c] > actualTo)
                {
                    actualGain = actualToGain;
                }
                else
                {
                    var tmpgain = (actualFrequencies[c] - actualFrom) / delta;
                    if (mode == SlopeModes.Smooth)
                    {
                        tmpgain = smoothSlope(tmpgain);
                    }
                    tmpgain = deltaGain * tmpgain + actualFromGain;
                    actualGain = tmpgain;
                }

                yield return logarithmicAmplitudes ? Math.Pow(10, actualGain) : actualGain;
            }
        }

        /// <summary>
        ///     Performs a circular shift on an array.
        /// </summary>
        /// <param name="input">The array to be circularly shifted.</param>
        /// <param name="offset">
        ///     The amount of samples the array should be shifted. Positive offsets are used for right-shifts while negative
        ///     offsets are
        ///     used for left-shifts.
        /// </param>
        /// <returns>An array of the same length as <paramref name="input" /> containing the result.</returns>
        public static IEnumerable<double> CircularShift(IReadOnlyList<double> input, int offset)
        {
            return input.Skip(offset).Concat(input.Take(offset));
        }

        /// <summary>
        ///     Generates a complex series consisting only of ones.
        /// </summary>
        /// <param name="length">The length of the series.</param>
        /// <returns>A new complex array of length <paramref name="length" /> containing the result.</returns>
        public static IReadOnlyList<Complex> ComplexOnes(int length)
        {
            var ret = new Complex[length];
            for (int i = 0; i < length; i++)
            {
                ret[i] = Complex.One;
            }
            return ret.ToReadOnlyList();
        }

        public static int ConvertTimeToSampleDelay(double delay, double sampleRate, out bool integer)
        {
            var mod = Math.Abs(delay % (1 / sampleRate));

            if ((mod > 1e-10) && (mod < 1 / sampleRate - 1e-10))
            {
                integer = false;
            }
            else
            {
                integer = true;
            }

            return Convert.ToInt32(delay * sampleRate);
        }

        /// <summary>
        /// Convolves the specified signals. 
        /// </summary>
        /// <param name="signal1">The first signal. Can be of infinite length.</param>
        /// <param name="signal2">The second signal.</param>
        /// <returns>The convolution of the two signals.</returns>
        public static IEnumerable<double> Convolve(IEnumerable<double> signal1, IReadOnlyList<double> signal2)
        {
            var e1 = signal1.GetEnumerator();

            var n = 2 * Fft.NextPowerOfTwo(signal2.Count);
            var blockSize = n - signal2.Count + 1;
            var sig2Fft = Fft.RealFft(signal2, n);
            IReadOnlyList<double> buffer = null;
            var sig1Buffer = new List<double>(blockSize);

            while (true)
            {
                var c = 0;
                while ((c < blockSize) && e1.MoveNext())
                {
                    sig1Buffer.Add(e1.Current);
                    c++;
                }

                if (c == 0)
                {
                    break;
                }

                var sig1Fft = Fft.RealFft(sig1Buffer, n);
                sig1Buffer.Clear();
                var spec = sig1Fft.Multiply(sig2Fft);
                var blockconv = Fft.RealIfft(spec);

                IEnumerable<double> ret = blockconv;

                if (buffer != null)
                {
                    ret = ret.AddFull(buffer);
                }

                if (c < blockSize)
                {
                    foreach (var d in ret.Take(c + signal2.Count - 1))
                    {
                        yield return d;
                    }

                    yield break;
                }

                foreach (var d in ret.Take(blockSize))
                {
                    yield return d;
                }

                buffer = blockconv.GetRangeOptimized(blockSize, signal2.Count - 1).ToReadOnlyList();
            }
        }

        /// <summary>
        ///     Converts a single value from dB to linear scale.
        /// </summary>
        /// <param name="dB">The value in dB.</param>
        /// <returns>The value in linear scale.</returns>
        public static double DbToLinear(double dB)
        {
            return Math.Pow(10, dB / 20);
        }

        /// <summary>
        ///     Converts an array from dB to linear scale.
        /// </summary>
        /// <param name="dB">The array in dB scale.</param>
        /// <returns>A new array of the same length as <paramref name="dB" /> containing the result.</returns>
        public static IEnumerable<double> DbToLinear(IEnumerable<double> dB)
        {
            return dB.Select(DbToLinear);
        }

        /// <summary>
        ///     Converts a singe value from degree to rad.
        /// </summary>
        /// <param name="deg">The value in degree.</param>
        /// <returns>The value in rad.</returns>
        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180;
        }

        /// <summary>
        ///     Converts an array from degree to rad.
        /// </summary>
        /// <param name="deg">The array in degree.</param>
        /// <returns>A new array of the same length as <paramref name="deg" /> containing the result.</returns>
        public static IEnumerable<double> DegToRad(IEnumerable<double> deg)
        {
            return deg.Select(DegToRad);
        }

        /// <summary>
        ///     Generates a dirac pulse (1 followed by zeros) of a specified length.
        /// </summary>
        /// <param name="length">The length of the pulse.</param>
        /// <returns>A new array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> Dirac(int length)
        {
            return 1.0.ToEnumerable().Concat(new double[length - 1]);
        }

        /// <summary>
        /// Calculates an infinite white noise sequence.
        /// </summary>  
        /// <returns></returns>
        /// <remarks>http://dspguru.com/dsp/howtos/how-to-generate-white-gaussian-noise</remarks>
        public static IEnumerable<double> WhiteNoise(int seed)
        {
            var rnd = new Random(seed);

            while (true)
            {
                double v1, v2, s;
                do
                {
                    v1 = 2 * rnd.NextDouble() - 1;
                    v2 = 2 * rnd.NextDouble() - 1;
                    s = v1 * v1 + v2 * v2;
                }
                while (s >= 1);

                yield return Math.Sqrt(-2 * Math.Log(s) / s) * v1;
                yield return Math.Sqrt(-2 * Math.Log(s) / s) * v2;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Applies an IIR filter to the provided input signal.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <param name="a">The denominator coefficients of the filter.</param>
        /// <param name="b">The numerator coefficients of the filter.</param>
        /// <param name="inputbuffer">The inputbuffer. If not provided, an empty buffer is created.</param>
        /// <param name="outputbuffer">The outputbuffer. If not provided, an empty buffer is created.</param>        
        /// <param name="clip">If set to true, the output signal is clipped to the length of the input signal. Otherwise, the output signal will be infinitely long.</param>
        /// <returns>The filter output.</returns>
        public static IEnumerable<double> IirFilter(
            IEnumerable<double> input,
            IReadOnlyList<double> a,
            IReadOnlyList<double> b,
            CircularBuffer<double> inputbuffer = null,
            CircularBuffer<double> outputbuffer = null,
            bool clip = false)
        {
            if (a.Count != b.Count)
            {
                throw new ArgumentException();
            }

            var n = a.Count - 1;

            if ((inputbuffer == null) || (outputbuffer == null))
            {
                inputbuffer = new CircularBuffer<double>(n);
                outputbuffer = new CircularBuffer<double>(n);
            }
            else if ((inputbuffer.Length != n) || (outputbuffer.Length != n))
            {
                throw new ArgumentException();
            }

            var an = a.Multiply(1 / a[0]).ToReadOnlyList();
            var bn = b.Multiply(1 / a[0]).ToReadOnlyList();
            var e = input.GetEnumerator();

            while (e.MoveNext())
            {
                double currentY = e.Current * bn[0];

                for (int i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(i) * bn[i];
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                inputbuffer.Store(e.Current);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            if (clip)
            {
                yield break;
            }

            for (int i2 = 0; i2 < inputbuffer.Length; i2++)
            {
                double currentY = 0.0;

                for (int i = 1; i <= n; i++)
                {
                    currentY += inputbuffer.Peek(i) * bn[i];
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                inputbuffer.Store(0.0);
                outputbuffer.Store(currentY);

                yield return currentY;
            }

            while (true)
            {
                double currentY = 0.0;
                for (int i = 1; i <= n; i++)
                {
                    currentY -= outputbuffer.Peek(i) * an[i];
                }

                outputbuffer.Store(currentY);

                yield return currentY;
            }
        }

        /// <summary>
        ///     Calculates the frequency response of an IIR filter.
        /// </summary>
        /// <param name="a">The denominator coefficients.</param>
        /// <param name="b">The numerator coefficents.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <param name="samplerate">The samplerate.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<Complex> IirFrequencyResponse(IReadOnlyList<double> a, IReadOnlyList<double> b, IReadOnlyList<double> frequencies, double samplerate)
        {
            var len = a.Count;
            if (b.Count != len)
            {
                throw new NotSupportedException();
            }

            double factor = 2 * Math.PI / samplerate;

            foreach (double d in frequencies)
            {
                var w = d * factor;
                Complex nom = 0;
                Complex den = 0;
                for (var c1 = 0; c1 < len; c1++)
                {
                    nom += b[c1] * Complex.Exp((len - c1) * Complex.ImaginaryOne * w);
                    den += a[c1] * Complex.Exp((len - c1) * Complex.ImaginaryOne * w);
                }

                yield return nom / den;
            }
        }

        /// <summary>
        ///     Interpolates a data series with x and y values to a new series with the specified x values.
        ///     Depending on the local point density of the original and new x values either spline interpolation, linear
        ///     interpolation or moving averaging is used to calculate the new y values.
        /// </summary>
        /// <param name="x">The original x values.</param>
        /// <param name="y">The original y values; must be the same length as <paramref name="x" />.</param>
        /// <param name="xtarget">The new x values.</param>
        /// <param name="logX">Determines whether the calculation should be performed with logarithmic scaling along the x axis.</param>
        /// <returns>The interpolated y values.</returns>
        public static IEnumerable<double> AdaptiveInterpolation(IReadOnlyList<double> x, IReadOnlyList<double> y, IReadOnlyList<double> xtarget, bool logX = true)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException();
            }

            IReadOnlyList<double> actualX;
            IReadOnlyList<double> actualTargetX;
            IReadOnlyList<double> spline = null;

            if (logX)
            {
                actualX = x.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
                actualTargetX = xtarget.Log(10).Select(d => Math.Max(d, -1e9)).ToReadOnlyList();
            }
            else
            {
                actualX = x;
                actualTargetX = xtarget;
            }

            var xCurrent = 0;
            while (actualX[xCurrent + 1] < actualTargetX[0])
            {
                xCurrent += 1;
            }

            var xMax = actualX.Count - 1;
            while (actualX[xMax - 1] > actualTargetX.Last())
            {
                xMax -= 1;
            }

            for (var c = 0; c < actualTargetX.Count; c++)
            {
                if (actualTargetX[c] < actualX.First())
                {
                    yield return double.NaN;
                    continue;
                }

                if (actualTargetX[c] > actualX.Last())
                {
                    yield return double.NaN;
                    continue;
                }

                double xlim;
                if (c == actualTargetX.Count - 1)
                {
                    xlim = actualTargetX[c];
                }
                else
                {
                    xlim = (actualTargetX[c + 1] + actualTargetX[c]) / 2;
                }

                var pointCounter = 0;
                while ((xCurrent < xMax) && (actualX[xCurrent] < xlim))
                {
                    pointCounter += 1;
                    xCurrent += 1;
                }

                if (pointCounter < 2) // spline
                {
                    if (spline == null)
                    {
                        spline = CubicSpline.Compute(actualX.ToArray(), y.ToArray(), actualTargetX.ToArray());
                    }

                    yield return spline[c];
                }
                else if (pointCounter < 3) // linear interpolation
                {
                    var tmp = (actualTargetX[c] - actualX[xCurrent - 1]) * y[xCurrent];
                    tmp += (actualX[xCurrent] - actualTargetX[c]) * y[xCurrent - 1];
                    tmp /= actualX[xCurrent] - actualX[xCurrent - 1];
                    yield return tmp;
                }
                else // average
                {
                    double tmp = 0;
                    for (var c2 = 1; c2 <= pointCounter; c2++)
                    {
                        tmp += y[xCurrent - c2];
                    }

                    tmp /= pointCounter;
                    yield return tmp;
                }
            }
        }

        /// <summary>
        ///     Interpolates a complex-valued series.
        /// </summary>
        /// <param name="x">The x-values of the original series.</param>
        /// <param name="y">The complex y-values of the original series. Must be the same length as <paramref name="x" />.</param>
        /// <param name="targetX">The desired x-values for the new series.</param>
        /// <param name="logX">
        ///     If <c>true</c> (default), the target x-values are assumed to be an a logarithmic scale and the
        ///     interpolation is done on a logarithmic scale as well.
        /// </param>
        /// <returns>A new array of the same length as <paramref name="targetX" /> containing the result.</returns>
        public static IEnumerable<Complex> Interpolate(IReadOnlyList<double> x, IReadOnlyList<Complex> y, IReadOnlyList<double> targetX, bool logX = true)
        {
            if (x.Count != y.Count)
            {
                throw new Exception();
            }

            IReadOnlyList<double> actualX;
            IReadOnlyList<double> actualTargetX;

            if (logX)
            {
                actualX = x.Log().ToReadOnlyList();
                actualTargetX = targetX.Log().ToReadOnlyList();
            }
            else
            {
                actualX = x.ToReadOnlyList();
                actualTargetX = targetX.ToReadOnlyList();
            }

            var magnitude = y.Magitude();
            var phase = y.Phase();
            phase = UnwrapPhase(phase);

            var mspline = CubicSpline.Compute(actualX.ToArray(), magnitude.ToArray(), actualTargetX.ToArray());
            var pspline = CubicSpline.Compute(actualX.ToArray(), phase.ToArray(), actualTargetX.ToArray());

            return PolarToComplex(mspline, pspline);
        }

        /// <summary>
        ///     Converts a single value from linear scale to dB.
        /// </summary>
        /// <param name="linear">The value in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>The value in dB.</returns>
        public static double LinearToDb(double linear, double minValue = double.NegativeInfinity)
        {
            if (linear <= 0)
            {
                return minValue;
            }

            return Math.Max(20 * Math.Log10(linear), minValue);
        }

        /// <summary>
        ///     Converts an array from linear scale to dB.
        /// </summary>
        /// <param name="linear">The array in linear scale.</param>
        /// <param name="minValue">The minimum return value.</param>
        /// <returns>A new array of the same length as <paramref name="linear" /> containing the result.</returns>
        public static IEnumerable<double> LinearToDb(IEnumerable<double> linear, double minValue = double.NegativeInfinity)
        {
            return linear.Select(d => LinearToDb(d, minValue));
        }

        /// <summary>
        ///     Generates a linear series of values between two points with a specified number of steps.
        /// </summary>
        /// <param name="from">The starting point of the series.</param>
        /// <param name="to">The stopping point of the series.</param>
        /// <param name="length">The number of steps (including starting and stopping points).</param>
        /// <returns>An array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> LinSeries(double from, double to, int length)
        {
            if (length < 1)
            {
                throw new ArgumentException();
            }
            var d = (to - from) / (length - 1);

            return Enumerable.Range(0, length).Select(i => i * d + from);
        }

        /// <summary>
        ///     Generates a logarithmic value series between a start and a stop value with a specified number of steps.
        /// </summary>
        /// <param name="from">The start value.</param>
        /// <param name="to">The stop value.</param>
        /// <param name="steps">The number of steps (including start and stop values).</param>
        /// <returns>A new array of length <paramref name="steps" /> containing the result.</returns>
        public static IEnumerable<double> LogSeries(double from, double to, int steps)
        {
            var startValueLog = Math.Log(from);
            var stopValueLog = Math.Log(to);
            var stepSizeLog = (stopValueLog - startValueLog) / (steps - 1);

            return Enumerable.Range(0, steps).Select(i => Math.Exp(startValueLog + i * stepSizeLog));
        }

        /// <summary>
        ///     Computes a logarithmic sweep.
        /// </summary>
        /// <param name="from">The start frequency of the sweep in Hz.</param>
        /// <param name="to">The stop frequency of the sweep in Hz.</param>
        /// <param name="length">The length oft the sweep in seconds.</param>
        /// <param name="samplerate">The samplerate of the sweep.</param>
        /// <param name="oversampling">
        ///     The oversampling used to calculate the sweep. Increases the accuracy of the phase
        ///     calculation, especially at higher frequencies.
        /// </param>
        /// <returns>An new array containing the result.</returns>
        public static IEnumerable<double> LogSweep(double from, double to, double length, double samplerate = 44100, int oversampling = 10)
        {
            var logAngularFrom = Math.Log(from * 2 * Math.PI / (samplerate * oversampling));
            var logAngularTo = Math.Log(to * 2 * Math.PI / (samplerate * oversampling));

            var steps = (int)(length * samplerate);
            var oversampledSteps = steps * oversampling;
            var logStep = (logAngularTo - logAngularFrom) / oversampledSteps;

            var logCurrentFrequency = logAngularFrom;
            var currentPhase = 0.0;

            for (var c = 0; c < oversampledSteps; c++)
            {
                if (c % oversampling == 0)
                {
                    yield return Math.Sin(currentPhase);
                }

                logCurrentFrequency += logStep;
                var currentFrequency = Math.Pow(Math.E, logCurrentFrequency);
                currentPhase += currentFrequency;
            }
        }

        /// <summary>
        ///     Finds the minimum distance between two neighbouring points of an array.
        /// </summary>
        /// <param name="input">The array.</param>
        /// <returns>The result.</returns>
        public static double MinimumDistance(IEnumerable<double> input)
        {
            var inputlist = input.ToReadOnlyList();

            var ret = double.PositiveInfinity;
            for (var c = 1; c < inputlist.Count; c++)
            {
                ret = Math.Min(ret, inputlist[c] - inputlist[c - 1]);
            }

            return ret;
        }

        /// <summary>
        ///     Calculates the modified bessel function of the first kind for a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        public static double ModBessel0(double x)
        {
            if (x < 0)
            {
                throw new Exception("Value out of range");
            }

            if (x < 4.9)
            {
                return 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 + Math.Pow(x, 10) / 14745600;
            }

            if (x > 5.1)
            {
                return Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                       (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                        893025 / (3932160 * x));
            }

            var t1 = 1 + Math.Pow(x, 2) / 4 + Math.Pow(x, 4) / 64 + Math.Pow(x, 6) / 2304 + Math.Pow(x, 8) / 147456 + Math.Pow(x, 10) / 14745600;
            var t2 = Math.Pow(Math.E, x) / Math.Sqrt(2 * Math.PI * x) *
                     (1 + 1 / (8 * x) + 9 / (128 * Math.Pow(x, 2)) + 225 / (3072 * Math.Pow(x, 3)) + 11025 / (98304 * Math.Pow(x, 4)) +
                      893025 / (3932160 * x));
            return t1 * (5.1 - x) / 0.2 + t2 * (x - 4.9) / 0.2;
        }

        /// <summary>
        ///     Generates a series of ones.
        /// </summary>
        /// <param name="length">The length of the series.</param>
        /// <returns>A new array of length <paramref name="length" /> containing the result.</returns>
        public static IReadOnlyList<double> Ones(int length)
        {
            var ret = new double[length];
            for (int i = 0; i < length; i++)
            {
                ret[i] = 1.0;
            }
            return ret.ToReadOnlyList();
        }

        /// <summary>
        ///     Converts two individual arrays containing magnitude and phase information to one complex array.
        /// </summary>
        /// <param name="amplitude">The amplitude data.</param>
        /// <param name="phase">The phase data. Has to be the same length as <paramref name="amplitude" />.</param>
        /// <returns>
        ///     A new complex array of the same length as <paramref name="amplitude" /> and <paramref name="phase" />
        ///     containing the result.
        /// </returns>
        public static IEnumerable<Complex> PolarToComplex(IEnumerable<double> amplitude, IEnumerable<double> phase)
        {
            return amplitude.Zip(phase, Complex.FromPolarCoordinates);
        }

        /// <summary>
        ///     Converts a single value from rad to degree.
        /// </summary>
        /// <param name="rad">The value in rad.</param>
        /// <returns>The value in degree.</returns>
        public static double RadToDeg(double rad)
        {
            return rad * 180 / Math.PI;
        }

        /// <summary>
        ///     Converts an array from rad to deg.
        /// </summary>
        /// <param name="rad">The array in rad.</param>
        /// <returns>A new array of the same length as <paramref name="rad" /> containing the result.</returns>
        public static IEnumerable<double> RadToDeg(IEnumerable<double> rad)
        {
            return rad.Select(RadToDeg);
        }

        /// <summary>
        ///     Resamples a frequency domain signal to a specified series of frequencies. Depending on the local point density of
        ///     the original
        ///     and new frequency values, either spline interpolation, linear interpolation or moving averaging is used to
        ///     calculate the new spectral amplitudes.
        /// </summary>
        /// <param name="x">The original frequencies.</param>
        /// <param name="y">The original spectral amplitudes. Must be the same length as <paramref name="x" />.</param>
        /// <param name="targetX">The target frequencies.</param>
        /// <param name="logX">Determines whether the calculation shall be performed in a logarithmic x space.</param>
        /// <returns>The resampled spectral amplitudes.</returns>
        public static IEnumerable<Complex> ResampleFrequencyResponse(IReadOnlyList<double> x, IReadOnlyList<Complex> y, IReadOnlyList<double> targetX, bool logX = true)
        {
            if (x.Count != y.Count)
            {
                throw new ArgumentException();
            }

            var magnitude = y.Magitude().ToReadOnlyList();
            var phase = y.Phase().ToReadOnlyList();
            phase = UnwrapPhase(phase).ToReadOnlyList();

            var smagnitude = AdaptiveInterpolation(x, magnitude, targetX, logX);
            var sphase = AdaptiveInterpolation(x, phase, targetX, logX);

            return PolarToComplex(smagnitude, sphase);
        }

        /// <summary>
        ///     Performs a linear right-shift on an array, filling the beginning with zeros.
        /// </summary>
        /// <param name="input">The array to be shifted.</param>
        /// <param name="offset">The amount of samples the array should be shifted. Must be positive.</param>
        /// <returns>An array containing the result.</returns>
        public static IEnumerable<double> RightShift(IEnumerable<double> input, int offset)
        {
            if (offset < 0)
            {
                throw new ArgumentException();
            }

            return Zeros(offset).Concat(input);
        }

        /// <summary>
        ///     Calculates the sinc = sin(pi * x) / (pi * x) of a single value.
        /// </summary>
        /// <param name="x">The value.</param>
        /// <returns>The result.</returns>
        public static double Sinc(double x)
        {
            if (x == 0)
            {
                return 1;
            }

            return Math.Sin(Math.PI * x) / (Math.PI * x);
        }

        /// <summary>
        ///     Generates a sinc pulse, multiplied by a symmetrical rectangle window to make its length finite.
        /// </summary>
        /// <param name="frequency">The frequency of the sinc pulse.</param>
        /// <param name="samplerate">The samplerate at which the sinc pulse should be generated.</param>
        /// <param name="length">The length of the resulting sinc pulse.</param>
        /// <param name="start">The start time (in samples).</param>
        /// <returns>An array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> WindowedSinc(double frequency, double samplerate, int length, int start = 0)
        {
            double factor = 2 * Math.PI * frequency / samplerate;
            return Enumerable.Range(start, length).Select(
                i =>
                {
                    if (i == 0)
                    {
                        return 1;
                    }

                    var omega = i * factor;
                    return Math.Sin(omega) / omega;
                });
        }

        
        public static IEnumerable<double> HalfSinc(double frequency, double samplerate)
        {
            yield return 1;

            double factor = 2 * Math.PI * frequency / samplerate;
            int c = 1;  
            while (true)
            {
                var omega = c * factor;
                yield return Math.Sin(omega) / omega;
                c++;
            }
            // ReSharper disable once FunctionNeverReturns
            // Output is meant to be infinte
        }

        /// <summary>
        ///     Smooths the y-values of a set of xy-related data with a moving average filter.
        /// </summary>
        /// <param name="xValues">The x-values.</param>
        /// <param name="yValues">The y-values. Must be the same length as <paramref name="xValues" />.</param>
        /// <param name="resolution">The smoothing resultion in points per octave.</param>
        /// <param name="logX">If <c>true</c> (default), the x-values are assumed to be on a logarithmic scale.</param>
        /// <returns>
        ///     An array of the same length as <paramref name="xValues" /> and <paramref name="yValues" /> containing the
        ///     result.
        /// </returns>
        public static IEnumerable<double> Smooth(IReadOnlyList<double> xValues, IReadOnlyList<double> yValues, int resolution, bool logX = true)
        {
            if (xValues.Count != yValues.Count)
            {
                throw new ArgumentException();
            }

            if (resolution == 0)
            {
                foreach (var d in yValues)
                {
                    yield return d;
                }
            }

            double bandwidth;
            IReadOnlyList<double> actualX;

            if (logX)
            {
                actualX = xValues.Log(10).ToReadOnlyList();
                bandwidth = Math.Log(Math.Pow(2.0, 1.0 / resolution));
            }
            else
            {
                actualX = xValues;
                bandwidth = Math.Pow(2.0, 1.0 / resolution);
            }

            for (var fc = 0; fc < yValues.Count; fc++)
            {
                double factorSum = 0;
                double sum = 0;
                var fc2 = fc;
                double factor;
                while (fc2 >= 0)
                {
                    if (!(actualX[fc2] > actualX[fc] - bandwidth))
                    {
                        break;
                    }

                    factor = SmoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * yValues[fc2];
                    fc2 -= 1;
                }

                fc2 = fc + 1;
                while (fc2 < actualX.Count)
                {
                    if (!(actualX[fc2] < actualX[fc] + bandwidth))
                    {
                        break;
                    }

                    factor = SmoothWindow(actualX[fc2], actualX[fc], bandwidth);
                    factorSum += factor;
                    sum += factor * yValues[fc2];
                    fc2 += 1;
                }

                yield return sum / factorSum;
            }
        }

        /// <summary>
        ///     Unwraps phase information.
        /// </summary>
        /// <param name="phase">The phase array.</param>
        /// <param name="useDeg">If true, the phase unit is assumed to be degree, otherwise rad (default).</param>
        /// <returns>A new array the same length as <paramref name="phase" /> containing the result.</returns>
        public static IEnumerable<double> UnwrapPhase(IEnumerable<double> phase, bool useDeg = false)
        {
            double fullPeriod;
            double halfPeriod;
            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            double offset = 0;

            var ephase = phase.GetEnumerator();
            if (!ephase.MoveNext())
            {
                yield break;
            }
            
            var previousPhase = ephase.Current;

            yield return previousPhase;
            while (ephase.MoveNext())
            {
                if (previousPhase - ephase.Current > halfPeriod)
                {
                    offset += fullPeriod;
                }
                else if (previousPhase - ephase.Current < -halfPeriod)
                {
                    offset -= fullPeriod;
                }
                previousPhase = ephase.Current + offset;
                yield return previousPhase;
            }
        }

        /// <summary>
        ///     Wraps phase data from an array, so that all resulting values are in the range -180° to +180° (or -pi to +pi).
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="useDeg">If true, the angular unit is assumed to be degree, otherwise radians (default).</param>
        /// <returns></returns>
        public static IEnumerable<double> WrapPhase(IEnumerable<double> input, bool useDeg = false)
        {
            double fullPeriod;
            double halfPeriod;

            if (useDeg)
            {
                fullPeriod = 360;
                halfPeriod = 180;
            }
            else
            {
                fullPeriod = 2 * Math.PI;
                halfPeriod = Math.PI;
            }

            return input.Select(
                d =>
                {
                    var tmp = d % fullPeriod;
                    if (tmp > halfPeriod)
                    {
                        tmp -= fullPeriod;
                    }
                    else if (tmp < -halfPeriod)
                    {
                        tmp += fullPeriod;
                    }
                    return tmp;
                });
        }

        /// <summary>
        ///     Zero-Pads or truncates an array of arbitraty length to the desired length.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <param name="length">The desired length.</param>
        /// <returns>A new array of length <paramref name="length" /> containing the result.</returns>
        public static IEnumerable<double> ZeroPad(IEnumerable<double> input, int length)
        {
            var e = input.GetEnumerator();
            int c = 0;

            while (e.MoveNext() && (c < length))
            {
                yield return e.Current;
                c++;
            }

            while (c < length)
            {
                yield return 0.0;
                c++;
            }
        }

        /// <summary>
        ///     Generates a series of zeros.
        /// </summary>
        /// <param name="length">The length of the series.</param>
        /// <returns>A new array of length <paramref name="length" /> containing the result.</returns>
        public static IReadOnlyList<double> Zeros(int length)
        {
            return new double[length].ToReadOnlyList();
        }

        private static double SmoothWindow(double logF, double logF0, double bandwidth)
        {
            var argument = (logF - logF0) / bandwidth * Math.PI;
            if (Math.Abs(argument) >= Math.PI)
            {
                return 0;
            }
            return 0.5 * (1.0 + Math.Cos(argument));
        }
    }
}