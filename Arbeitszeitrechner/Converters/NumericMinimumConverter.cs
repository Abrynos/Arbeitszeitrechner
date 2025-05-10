// Copyright (C) 2025-2025 Sebastian Göls
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; version 2 of the License only
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using System;
using System.Globalization;
using System.Numerics;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Arbeitszeitrechner.Converters;

internal abstract class NumericMinimumConverter<T> : MarkupExtension, IValueConverter where T : INumber<T>
{
    /// <inheritdoc/>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not T val)
        {
            throw new ArgumentException($"{nameof(value)} is not {typeof(T).FullName}");
        }

        if (parameter is not T minimum)
        {
            minimum = T.Zero;
        }

        return val > minimum ? val : minimum;
    }

    /// <inheritdoc/>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
