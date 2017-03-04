// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ASIOStructures.cs">
//   Copyright (c) 2017 NAudio contributors, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;

namespace DspSharpAsio
{
    // -------------------------------------------------------------------------------
    // Structures used by the AsioDriver and ASIODriverExt
    // -------------------------------------------------------------------------------

    // -------------------------------------------------------------------------------
    // Error and Exceptions
    // -------------------------------------------------------------------------------

    /// <summary>
    ///     ASIO common Exception.
    /// </summary>
    internal class AsioException : Exception
    {
        private AsioError error;

        public AsioException()
        {
        }

        public AsioException(string message)
            : base(message)
        {
        }

        public AsioException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AsioError Error
        {
            get { return this.error; }
            set
            {
                this.error = value;
                this.Data["ASIOError"] = this.error;
            }
        }

        /// <summary>
        ///     Gets the name of the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>the name of the error</returns>
        public static string getErrorName(AsioError error)
        {
            return Enum.GetName(typeof(AsioError), error);
        }
    }

    // -------------------------------------------------------------------------------
    // Channel Info, Buffer Info
    // -------------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct AsioBufferInfo
    {
        public bool isInput; // on input:  ASIOTrue: input, else output
        public int channelNum; // on input:  channel index
        public IntPtr pBuffer0; // on output: double buffer addresses
        public IntPtr pBuffer1; // on output: double buffer addresses

        public IntPtr Buffer(int bufferIndex)
        {
            return bufferIndex == 0 ? this.pBuffer0 : this.pBuffer1;
        }
    }

    // -------------------------------------------------------------------------------
    // Time structures
    // -------------------------------------------------------------------------------

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct AsioTimeCode
    {
        public double speed; // speed relation (fraction of nominal speed)
        // ASIOSamples     timeCodeSamples;        // time in samples
        public Asio64Bit timeCodeSamples; // time in samples
        public AsioTimeCodeFlags flags; // some information flags (see below)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string future;
    }

    [Flags]
    internal enum AsioTimeCodeFlags
    {
        kTcValid = 1,
        kTcRunning = 1 << 1,
        kTcReverse = 1 << 2,
        kTcOnspeed = 1 << 3,
        kTcStill = 1 << 4,
        kTcSpeedValid = 1 << 8
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct AsioTimeInfo
    {
        public double speed; // absolute speed (1. = nominal)
        public Asio64Bit systemTime; // system time related to samplePosition, in nanoseconds
        public Asio64Bit samplePosition;
        public double sampleRate; // current rate
        public AsioTimeInfoFlags flags; // (see below)
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)] public string reserved;
    }

    [Flags]
    internal enum AsioTimeInfoFlags
    {
        kSystemTimeValid = 1, // must always be valid
        kSamplePositionValid = 1 << 1, // must always be valid
        kSampleRateValid = 1 << 2,
        kSpeedValid = 1 << 3,
        kSampleRateChanged = 1 << 4,
        kClockSourceChanged = 1 << 5
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    internal struct AsioTime
    {
        // both input/output
        public int reserved1;
        public int reserved2;
        public int reserved3;
        public int reserved4;
        public AsioTimeInfo timeInfo; // required
        public AsioTimeCode timeCode; // optional, evaluated if (timeCode.flags & kTcValid)
    }
}