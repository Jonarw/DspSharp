// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileImpulseResponseFactory.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DspSharp.Signal;
using UTilities.Observable.DataAnnotations;

namespace DspSharpDemo.ViewModel.Signal.SignalFactories
{
    public class FileImpulseResponseFactory : SignalFactory
    {
        private string _FileName;

        public FileImpulseResponseFactory(double sampleRate) : base(sampleRate)
        {
        }

        [NotNull]
        public string FileName
        {
            get { return this._FileName; }
            set { this.SetField(ref this._FileName, value); }
        }

        public override ISignal CreateItem()
        {
            try
            {
                var fi = new FileInfo(this.FileName);
                if (!fi.Exists)
                    return null;
            }
            catch (Exception)
            {
                return null;
            }

            var file = File.ReadLines(this.FileName);

            var signal = new List<double>();

            foreach (var line in file)
            {
                var fields = line.Split(',');

                double ret;
                if (!double.TryParse(fields[0], NumberStyles.Any, CultureInfo.InvariantCulture, out ret))
                    continue;

                if (fields.Length == 1)
                    signal.Add(ret);
                else
                {
                    if (fields.Length == 2)
                    {
                        if (!double.TryParse(fields[1], NumberStyles.Any, CultureInfo.InvariantCulture, out ret))
                            continue;

                        signal.Add(ret);
                    }
                }
            }

            return new FiniteSignal(signal, this.SampleRate, this.TimeOffset);
        }
    }
}