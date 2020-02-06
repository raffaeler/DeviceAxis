using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Media;
using IAmRaf.MVVM.Converters;
using IAmRaf.MVVM.MarkupExtensions;

namespace WitUI.Converters
{
    public class BoolImageConverter : ConverterMarkupBase
    {
        public string ImageOn { get; set; }
        public string ImageOff { get; set; }

        public BoolImageConverter()
        {

        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal && boolVal)
            {
                return ConverterHelper.GetImageFromPack(ImageOn);
                //return ImageOn;
            }

            return ConverterHelper.GetImageFromPack(ImageOff);
        }
    }
}
