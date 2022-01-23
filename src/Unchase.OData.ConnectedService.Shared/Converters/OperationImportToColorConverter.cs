using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.OData.Edm;

namespace Unchase.OData.ConnectedService.Converters
{
    public class OperationImportToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new InvalidOperationException("Приемник должен быть типа Brush.");

            if (value == null)
                return Brushes.Coral;

            if (value is IEdmOperationImport operationImport)
                switch (operationImport)
                {
                    case IEdmFunctionImport _:
                        return Brushes.Coral;
                    case IEdmActionImport _:
                        return Brushes.DarkKhaki;
                }

            return Brushes.Coral;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
