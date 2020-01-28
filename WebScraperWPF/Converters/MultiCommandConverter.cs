using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using WebScraperWPF.Commands;

namespace WebScraperWPF.Converters
{
    class MultiCommandConverter : IMultiValueConverter
    {
        private List<GenericActionCommand> _values = new List<GenericActionCommand>();
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            _values.AddRange(values.Cast<GenericActionCommand>());
            return new GenericActionCommand(new Action(() =>
            {
                foreach (ICommand command in _values)
                {
                    command.Execute(null); // TODO: Pretty faulty
                }
            }));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


    }
}
