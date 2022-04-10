// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Observable.cs">
//   Copyright (c) 2017 Jonathan Arweck, see LICENSE.txt for license information
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DspSharp
{
    public class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Called when a property value changed. MUST be called from the changed property's setter!
        /// </summary>
        protected void OnPropertyChangedAuto([CallerMemberName] string propertyName = null)
        {
            this.OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Sets the field to the specified value and raises the PropertyChanged event. MUST be called from the changed property's setter!
        /// </summary>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return this.SetField(propertyName, ref field, value);
        }

        /// <summary>
        /// Sets the field to the specified value and raises the PropertyChanged event. MUST be called from the changed property's setter!
        /// </summary>
        protected bool SetField<T>(ref T field, T value, Action action, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            action();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the field to the specified value and raises the PropertyChanged event. MUST be called from the changed property's setter!
        /// </summary>
        protected bool SetField<T>(ref T field, T value, Action<T> action, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            var oldValue = field;
            field = value;
            action(oldValue);
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Sets the field to the specified value and raises the PropertyChanged event. MUST be called from the changed property's setter!
        /// </summary>
        protected bool SetField<T>(ref T field, T value, Func<T, bool> action, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            var oldValue = field;
            field = value;
            if (!action(oldValue))
            {
                field = oldValue;
                return false;
            }

            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected bool SetField<T>(string propertyName, ref T field, T value)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}