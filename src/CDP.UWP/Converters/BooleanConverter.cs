//---------------------------------------------------------------
// Date: 2/7/2018
// Rights: 
// FileName: BooleanConverter.cs
//---------------------------------------------------------------

namespace CDP.UWP.Converters
{
    using System;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converts a bool for UI purposes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanConverter{T}"/> class.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        /// <summary>
        /// Gets or sets the true.
        /// </summary>
        /// <value>
        /// The true.
        /// </value>
        public T True { get; set; }

        /// <summary>
        /// Gets or sets the false.
        /// </summary>
        /// <value>
        /// The false.
        /// </value>
        public T False { get; set; }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public virtual object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool && ((bool)value) ? True : False;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        public virtual object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }
}
