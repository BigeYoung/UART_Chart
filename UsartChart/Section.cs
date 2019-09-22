using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UsartChart
{
    public enum SectionType
    {
        UNKNOWN,
        INT8,
        INT16,
        INT32,
        UINT8,
        UINT16,
        UINT32,
        Float,
        Double
    }
    public class Section
    {
        public Section()
        {
            SubscriptCMD = new RelayCommand(() => UART.Subscript(Addr, Size));
            UnsubscriptCMD = new RelayCommand(() => UART.Unsubscript(Addr));
        }
        public RelayCommand SubscriptCMD { get; set; }
        public RelayCommand UnsubscriptCMD { get; set; }

        public SectionType Type { get; set; }
        public string Name { get; set; }
        public uint Addr { get; set; }
        public byte Size { get; set; }

        public static SectionType TypeParse(string s)
        {
            if (s.StartsWith("volatile "))
            {
                s.Remove(0, "volatile ".Length);
            }
            switch (s)
            {
                case "uint8_t":
                    return SectionType.UINT8;
                case "uint16_t":
                    return SectionType.UINT16;
                case "uint32_t":
                    return SectionType.UINT32;
                case "int8_t":
                    return SectionType.INT8;
                case "int16_t":
                    return SectionType.INT16;
                case "int32_t":
                    return SectionType.INT32;
                case "unsigned char":
                    return SectionType.UINT8;
                case "short unsigned int":
                    return SectionType.UINT16;
                case "unsigned int":
                    return SectionType.UINT32;
                case "char":
                    return SectionType.INT8;
                case "short int":
                    return SectionType.INT16;
                case "int":
                    return SectionType.INT32;
                case "float":
                    return SectionType.Float;
                case "double":
                    return SectionType.Double;
                default:
                    return SectionType.UNKNOWN;
            }
        }
    }
    public class SectionData
    {
        public string Name;
        public uint Addr;
        public double Value;
    }

    [ValueConversion(typeof(SectionType), typeof(SolidColorBrush))]
    public class TypeHighlight : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SectionType type = (SectionType)value;
            switch (type)
            {
                case SectionType.UNKNOWN:
                    return Brushes.Red;
                case SectionType.INT8:
                case SectionType.INT16:
                case SectionType.INT32:
                    return Brushes.DarkSlateBlue;
                case SectionType.UINT8:
                case SectionType.UINT16:
                case SectionType.UINT32:
                    return Brushes.DarkOliveGreen;
                case SectionType.Float:
                case SectionType.Double:
                    return Brushes.DarkGoldenrod;
                default:
                    return Brushes.DarkGray;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(SectionType), typeof(string))]
    public class TypeName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SectionType type = (SectionType)value;
            switch (type)
            {
                case SectionType.UNKNOWN:
                    return "???";
                case SectionType.INT8:
                    return "int8_t";
                case SectionType.INT16:
                    return "int16_t";
                case SectionType.INT32:
                    return "int32_t";
                case SectionType.UINT8:
                    return "uint8_t";
                case SectionType.UINT16:
                    return "uint16_t";
                case SectionType.UINT32:
                    return "uint32_t";
                case SectionType.Float:
                    return "float";
                case SectionType.Double:
                    return "double";
                default:
                    return "???";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
