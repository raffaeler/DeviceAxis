using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using IAmRaf.MVVM.MarkupExtensions;

namespace WitUI.Converters
{
    public class NotConverter: ConverterMarkupBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal) return !boolVal;
            return null;
        }

    }
}
