// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2024
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ******************************************************************************************************************************
using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace FileRenamer.Converters
{
	/// <summary>
	/// Converter to convert an enum value to bool by comparing to the given parameter.
	/// Both value and parameter must be of the same enum type.
	/// </summary>
	/// <remarks>
	/// This converter is useful to enable binding of radio buttons with a selected enum value.
	/// </remarks>
	public class EnumToBoolConverter : IValueConverter
	{
		/// <inheritdoc/>
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value == null && parameter == null)
				return true;
			else if (value == null || parameter == null)
				return false;
			else
				return value!.Equals(parameter);
		}

		/// <inheritdoc/>
		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is bool boolValue && boolValue == true)
				return parameter;

			return BindingOperations.DoNothing;
		}
	}
}