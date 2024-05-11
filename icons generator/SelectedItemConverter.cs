using System;
using icons_generator.Resources.Options;
using Microsoft.Maui.Controls;

namespace YourNamespace
{
    public class SelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Microsoft.Maui.Graphics.Color.FromRgba(255,0,0,1); // Цвет для невыбранного элемента
            else
                return Microsoft.Maui.Graphics.Color.FromRgba(0, 0, 0, 1); // Цвет для выбранного элемента
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
