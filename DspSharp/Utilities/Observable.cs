// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Observable.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DspSharp.Utilities
{
    /// <summary>
    ///     Base class to simplify the implementation of the INotifyPropertyChanged interface.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class Observable : INotifyPropertyChanged
    {
        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises the PropertyChanged event for the provided property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises the PropertyChanged event for the calling property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected void RaisePropertyChangedAuto([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Sets the specified field to the provided value and raised the PropertyChanged event for the calling property if the
        ///     value was changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return this.SetField(propertyName, ref field, value);
        }

        /// <summary>
        ///     Sets the specified field to the provided value and raised the PropertyChanged event for the provided property if
        ///     the value was changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        protected bool SetField<T>(string propertyName, ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}