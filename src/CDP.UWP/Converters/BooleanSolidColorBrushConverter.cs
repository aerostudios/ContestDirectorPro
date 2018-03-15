//---------------------------------------------------------------
// Date: 2/7/2018
// Rights: 
// FileName: BooleanSolidColorBrushConverter.cs
//---------------------------------------------------------------

namespace CDP.UWP.Converters
{
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Coverts a boolean value to a solid color brush
    /// </summary>
    /// <seealso cref="CDP.UWP.Converters.BooleanConverter{Windows.UI.Xaml.Media.SolidColorBrush}" />
    public class BooleanSolidColorBrushConverter : BooleanConverter<SolidColorBrush>
    {
        public BooleanSolidColorBrushConverter():base(new SolidColorBrush(), new SolidColorBrush()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanSolidColorBrushConverter"/> class.
        /// </summary>
        /// <param name="trueValue">The true value.</param>
        /// <param name="falseValue">The false value.</param>
        public BooleanSolidColorBrushConverter(SolidColorBrush trueValue, SolidColorBrush falseValue) : base(trueValue, falseValue)
        {
        }
    }
}
