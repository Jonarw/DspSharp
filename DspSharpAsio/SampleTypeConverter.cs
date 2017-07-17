using System;
using System.Collections.Generic;

namespace DspSharpAsio
{
    public static class AsioSampleTypeExtensions
    {
        public static int GetSize(this AsioSampleType sampleType)
        {
            return SampleTypeConverter.SizeOf(sampleType);
        }
    }

    public static unsafe class SampleTypeConverter
    {
        public static IReadOnlyList<AsioSampleType> SupportedSampleFormats { get; } =
            new List<AsioSampleType> {AsioSampleType.Int32LSB, AsioSampleType.Float32LSB}.AsReadOnly();

        public static int SizeOf(AsioSampleType sampleType)
        {
            if (sampleType == AsioSampleType.Int32LSB)
                return sizeof(int);
            if (sampleType == AsioSampleType.Float32LSB)
                return sizeof(float);

            throw new NotSupportedException();
        }

        public static void ConvertFrom(AsioSampleType sampleType, void* input, double* output, int length)
        {
            if (sampleType == AsioSampleType.Int32LSB)
            {
                var intinput = (int*)input;

                for (int i = 0; i < length; i++)
                {
                    *(output + i) = *(intinput + i) / 2147483648d;
                }
            }
            else if (sampleType == AsioSampleType.Float32LSB)
            {
                var floatinput = (float*)input;

                for (int i = 0; i < length; i++)
                {
                    output[i] = floatinput[i];
                }
            }
            else
                throw new NotSupportedException();

        }

        public static void ConvertTo(AsioSampleType sampleType, double* input, void* output, int length)
        {
            if (sampleType == AsioSampleType.Int32LSB)
            {
                var intoutput = (int*)output;

                for (int i = 0; i < length; i++)
                {
                    *(intoutput + i) = (int)(Math.Max(Math.Min(*(input + i), 8388607.0d / 8388608.0d), -1) * 2147483648d);
                }
            }
            else if (sampleType == AsioSampleType.Float32LSB)
            {
                var floatoutput = (float*)output;

                for (int i = 0; i < length; i++)
                {
                    floatoutput[i] = (float)input[i];
                }
            }
            else
                throw new NotSupportedException();

        }
    }
}