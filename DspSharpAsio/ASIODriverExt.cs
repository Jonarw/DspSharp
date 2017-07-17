// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ASIODriverExt.cs">
//   Copyright (c) 2017 NAudio contributors, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace DspSharpAsio
{
    /// <summary>
    ///     AsioDriverExt is a simplified version of the AsioDriver. It provides an easier
    ///     way to access the capabilities of the Driver and implement the callbacks necessary
    ///     for feeding the driver.
    ///     Implementation inspired from Rob Philpot's with a managed C++ ASIO wrapper BlueWave.Interop.Asio
    ///     http://www.codeproject.com/KB/mcpp/Asio.Net.aspx
    ///     Contributor: Alexandre Mutel - email: alexandre_mutel at yahoo.fr
    /// </summary>
    public class AsioDriverExt : IDisposable
    {
        private AsioBufferInfo[] bufferInfos;
        private AsioCallbacks callbacks;
        private IntPtr[] currentInputBuffers;
        private IntPtr[] currentOutputBuffers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsioDriverExt" /> class based on an already
        ///     instantiated AsioDriver instance.
        /// </summary>
        /// <param name="driver">A AsioDriver already instantiated.</param>
        public AsioDriverExt(AsioDriver driver)
        {
            this.Driver = driver;

            if (!driver.Init(IntPtr.Zero))
                throw new InvalidOperationException(driver.GetErrorMessage());

            this.callbacks = new AsioCallbacks();
            this.callbacks.pasioMessage = this.AsioMessageCallBack;
            this.callbacks.pbufferSwitch = this.BufferSwitchCallBack;
            this.callbacks.pbufferSwitchTimeInfo = this.BufferSwitchTimeInfoCallBack;
            this.callbacks.psampleRateDidChange = this.SampleRateDidChangeCallBack;

            this.BuildCapabilities();
        }

        /// <summary>
        ///     Gets the capabilities of the AsioDriver.
        /// </summary>
        /// <value>The capabilities.</value>
        public AsioDriverCapability Capabilities { get; private set; }

        /// <summary>
        ///     Gets the driver used.
        /// </summary>
        /// <value>The ASIOdriver.</value>
        public AsioDriver Driver { get; }

        /// <summary>
        ///     Gets or sets the fill buffer callback. Avoid allocating any managed resources in here (GC kills performance)!
        /// </summary>
        /// <value>The fill buffer callback.</value>
        public AsioFillBufferCallback FillBufferCallback { get; set; }

        private int BufferSize { get; set; }
        private bool CreatedBuffers { get; set; }
        private int InputChannelOffset { get; set; }
        private bool IsOutputReadySupported { get; set; }
        private bool IsStarted { get; set; }
        private int NumberOfInputChannels { get; set; }
        private int NumberOfOutputChannels { get; set; }
        private int OutputChannelOffset { get; set; }

        public void Dispose()
        {
            this.ReleaseDriver();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Creates the buffers for playing.
        /// </summary>
        /// <param name="numberOfOutputChannels">The number of outputs channels.</param>
        /// <param name="numberOfInputChannels">The number of input channel.</param>
        /// <param name="bufferSize">The buffer size. -1 (default value) -> preferred buffer size; -2 -> maximum buffer size.</param>
        public int CreateBuffers(int numberOfOutputChannels, int numberOfInputChannels, int bufferSize = -1)
        {
            if (this.CreatedBuffers)
                throw new Exception();

            if (numberOfOutputChannels < 0 || numberOfOutputChannels > this.Capabilities.NbOutputChannels)
            {
                throw new ArgumentException(
                    $"Invalid number of channels {numberOfOutputChannels}, must be in the range [0,{this.Capabilities.NbOutputChannels}]");
            }
            if (numberOfInputChannels < 0 || numberOfInputChannels > this.Capabilities.NbInputChannels)
            {
                throw new ArgumentException(
                    "numberOfInputChannels",
                    $"Invalid number of input channels {numberOfInputChannels}, must be in the range [0,{this.Capabilities.NbInputChannels}]");
            }

            if (bufferSize == -1)
                bufferSize = this.Capabilities.BufferPreferredSize;
            else if (bufferSize == -2)
                bufferSize = this.Capabilities.BufferMaxSize;

            if (bufferSize < this.Capabilities.BufferMinSize || bufferSize > this.Capabilities.BufferMaxSize)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            // each channel needs a buffer info
            this.NumberOfOutputChannels = numberOfOutputChannels;
            this.NumberOfInputChannels = numberOfInputChannels;
            // Ask for maximum of output channels even if we use only the nbOutputChannelsArg
            int nbTotalChannels = this.Capabilities.NbInputChannels + this.Capabilities.NbOutputChannels;
            this.bufferInfos = new AsioBufferInfo[nbTotalChannels];
            this.currentOutputBuffers = new IntPtr[numberOfOutputChannels];
            this.currentInputBuffers = new IntPtr[numberOfInputChannels];

            // and do the same for output channels
            // ONLY work on output channels (just put isInput = true for InputChannel)
            int totalIndex = 0;
            for (int index = 0; index < this.Capabilities.NbInputChannels; index++, totalIndex++)
            {
                this.bufferInfos[totalIndex].isInput = true;
                this.bufferInfos[totalIndex].channelNum = index;
                this.bufferInfos[totalIndex].pBuffer0 = IntPtr.Zero;
                this.bufferInfos[totalIndex].pBuffer1 = IntPtr.Zero;
            }

            for (int index = 0; index < this.Capabilities.NbOutputChannels; index++, totalIndex++)
            {
                this.bufferInfos[totalIndex].isInput = false;
                this.bufferInfos[totalIndex].channelNum = index;
                this.bufferInfos[totalIndex].pBuffer0 = IntPtr.Zero;
                this.bufferInfos[totalIndex].pBuffer1 = IntPtr.Zero;
            }

            this.BufferSize = bufferSize;

            unsafe
            {
                fixed (AsioBufferInfo* infos = &this.bufferInfos[0])
                {
                    IntPtr pOutputBufferInfos = new IntPtr(infos);

                    // Create the ASIO Buffers with the callbacks
                    this.Driver.CreateBuffers(pOutputBufferInfos, nbTotalChannels, this.BufferSize, ref this.callbacks);
                }
            }

            // Check if outputReady is supported
            this.IsOutputReadySupported = this.Driver.OutputReady() == AsioError.ASE_OK;

            this.CreatedBuffers = true;
            return this.BufferSize;
        }

        /// <summary>
        ///     Determines whether the specified sample rate is supported.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <returns>
        ///     <c>true</c> if [is sample rate supported]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSampleRateSupported(double sampleRate)
        {
            return this.Driver.CanSampleRate(sampleRate);
        }

        /// <summary>
        ///     Allows adjustment of which is the first output channel we write to
        /// </summary>
        /// <param name="outputChannelOffset">Output Channel offset</param>
        /// <param name="inputChannelOffset">Input Channel offset</param>
        public void SetChannelOffset(int outputChannelOffset, int inputChannelOffset)
        {
            if (outputChannelOffset + this.NumberOfOutputChannels <= this.Capabilities.NbOutputChannels)
                this.OutputChannelOffset = outputChannelOffset;
            else
                throw new ArgumentException("Invalid channel offset");
            if (inputChannelOffset + this.NumberOfInputChannels <= this.Capabilities.NbInputChannels)
                this.InputChannelOffset = inputChannelOffset;
            else
                throw new ArgumentException("Invalid channel offset");
        }

        /// <summary>
        ///     Sets the sample rate.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        public void SetSampleRate(double sampleRate)
        {
            this.Driver.SetSampleRate(sampleRate);
            // Update Capabilities
            this.BuildCapabilities();
        }

        /// <summary>
        ///     Shows the control panel.
        /// </summary>
        public void ShowControlPanel()
        {
            this.Driver.ControlPanel();
        }

        /// <summary>
        ///     Starts playing the buffers.
        /// </summary>
        public void Start()
        {
            if (!this.IsStarted)
            {
                this.Driver.Start();
                this.IsStarted = true;
            }
        }

        /// <summary>
        ///     Stops playing the buffers.
        /// </summary>
        public void Stop()
        {
            if (this.IsStarted)
            {
                this.Driver.Stop();
                this.IsStarted = false;
            }
        }

        /// <summary>
        ///     Asio message call back.
        /// </summary>
        /// <param name="selector">The selector.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="opt">The opt.</param>
        /// <returns></returns>
        private int AsioMessageCallBack(AsioMessageSelector selector, int value, IntPtr message, IntPtr opt)
        {
            // Check when this is called?
            switch (selector)
            {
            case AsioMessageSelector.kAsioSelectorSupported:
                AsioMessageSelector subValue = (AsioMessageSelector)Enum.ToObject(typeof(AsioMessageSelector), value);
                switch (subValue)
                {
                case AsioMessageSelector.kAsioEngineVersion:
                    return 1;
                case AsioMessageSelector.kAsioResetRequest:
                    return 0;
                case AsioMessageSelector.kAsioBufferSizeChange:
                    return 0;
                case AsioMessageSelector.kAsioResyncRequest:
                    return 0;
                case AsioMessageSelector.kAsioLatenciesChanged:
                    return 0;
                case AsioMessageSelector.kAsioSupportsTimeInfo:
//                            return 1; DON'T SUPPORT FOR NOW. NEED MORE TESTING.
                    return 0;
                case AsioMessageSelector.kAsioSupportsTimeCode:
//                            return 1; DON'T SUPPORT FOR NOW. NEED MORE TESTING.
                    return 0;
                }
                break;
            case AsioMessageSelector.kAsioEngineVersion:
                return 2;
            case AsioMessageSelector.kAsioResetRequest:
                return 1;
            case AsioMessageSelector.kAsioBufferSizeChange:
                return 0;
            case AsioMessageSelector.kAsioResyncRequest:
                return 0;
            case AsioMessageSelector.kAsioLatenciesChanged:
                return 0;
            case AsioMessageSelector.kAsioSupportsTimeInfo:
                return 0;
            case AsioMessageSelector.kAsioSupportsTimeCode:
                return 0;
            }
            return 0;
        }

        /// <summary>
        ///     Callback called by the AsioDriver on fill buffer demand. Redirect call to external callback.
        /// </summary>
        /// <param name="doubleBufferIndex">Index of the double buffer.</param>
        /// <param name="directProcess">if set to <c>true</c> [direct process].</param>
        private void BufferSwitchCallBack(int doubleBufferIndex, bool directProcess)
        {
            for (var i = 0; i < this.NumberOfInputChannels; i++)
                this.currentInputBuffers[i] = this.bufferInfos[i + this.InputChannelOffset].Buffer(doubleBufferIndex);

            for (var i = 0; i < this.NumberOfOutputChannels; i++)
            {
                this.currentOutputBuffers[i] =
                    this.bufferInfos[i + this.OutputChannelOffset + this.Capabilities.NbInputChannels].Buffer(doubleBufferIndex);
            }

            this.FillBufferCallback?.Invoke(this.currentInputBuffers, this.currentOutputBuffers);

            if (this.IsOutputReadySupported)
                this.Driver.OutputReady();
        }

        /// <summary>
        ///     Buffers switch time info call back.
        /// </summary>
        /// <param name="asioTimeParam">The asio time param.</param>
        /// <param name="doubleBufferIndex">Index of the double buffer.</param>
        /// <param name="directProcess">if set to <c>true</c> [direct process].</param>
        /// <returns></returns>
        private IntPtr BufferSwitchTimeInfoCallBack(IntPtr asioTimeParam, int doubleBufferIndex, bool directProcess)
        {
            // Check when this is called?
            return IntPtr.Zero;
        }

        /// <summary>
        ///     Builds the capabilities internally.
        /// </summary>
        private void BuildCapabilities()
        {
            this.Capabilities = new AsioDriverCapability();

            this.Capabilities.DriverName = this.Driver.GetDriverName();

            // Get nb Input/Output channels
            this.Driver.GetChannels(out this.Capabilities.NbInputChannels, out this.Capabilities.NbOutputChannels);

            this.Capabilities.InputChannelInfos = new AsioChannelInfo[this.Capabilities.NbInputChannels];
            this.Capabilities.OutputChannelInfos = new AsioChannelInfo[this.Capabilities.NbOutputChannels];

            // Get ChannelInfo for Inputs
            for (int i = 0; i < this.Capabilities.NbInputChannels; i++)
                this.Capabilities.InputChannelInfos[i] = this.Driver.GetChannelInfo(i, true);

            // Get ChannelInfo for Output
            for (int i = 0; i < this.Capabilities.NbOutputChannels; i++)
                this.Capabilities.OutputChannelInfos[i] = this.Driver.GetChannelInfo(i, false);

            // Get the current SampleRate
            this.Capabilities.SampleRate = this.Driver.GetSampleRate();

            var error = this.Driver.GetLatencies(out this.Capabilities.InputLatency, out this.Capabilities.OutputLatency);
            // focusrite scarlett 2i4 returns ASE_NotPresent here

            if (error != AsioError.ASE_OK && error != AsioError.ASE_NotPresent)
            {
                var ex = new AsioException("ASIOgetLatencies");
                ex.Error = error;
                throw ex;
            }

            // Get BufferSize
            this.Driver.GetBufferSize(
                out this.Capabilities.BufferMinSize,
                out this.Capabilities.BufferMaxSize,
                out this.Capabilities.BufferPreferredSize,
                out this.Capabilities.BufferGranularity);
        }

        /// <summary>
        ///     Releases this instance.
        /// </summary>
        private void ReleaseDriver()
        {
            if (this.CreatedBuffers)
                this.Driver.DisposeBuffers();

            this.Driver.ReleaseComAsioDriver();
        }

        /// <summary>
        ///     Callback called by the AsioDriver on event "Samples rate changed".
        /// </summary>
        /// <param name="sRate">The sample rate.</param>
        private void SampleRateDidChangeCallBack(double sRate)
        {
            // Check when this is called?
            this.Capabilities.SampleRate = sRate;
        }

        ~AsioDriverExt()
        {
            this.ReleaseDriver();
        }
    }
}