using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace KanjiStrokes {
	[ValueConversion(typeof(IEnumerable<string>), typeof(string))]
	class StringJoinConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(!(value is IEnumerable<string> strings)) return "";
			return string.Join(parameter as string ?? "", strings);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
