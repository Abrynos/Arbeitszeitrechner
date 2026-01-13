// Copyright (C) 2025-2026 Sebastian Göls
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
using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace Arbeitszeitrechner.ViewModels;

[JsonObject(MemberSerialization.OptIn)]
internal sealed partial class AppDatabaseViewModel : ViewModelBase
{
    [JsonIgnore]
    internal CultureInfo? Language { get; private init; }

    [JsonProperty]
    internal Dictionary<DayOfWeek, TimeSpan> TimeOverrides { get; private init; } = new()
    {
        { DayOfWeek.Monday, new TimeSpan(8, 15, 0) },
        { DayOfWeek.Tuesday, new TimeSpan(8, 15, 0) },
        { DayOfWeek.Wednesday, new TimeSpan(8, 15, 0) },
        { DayOfWeek.Thursday, new TimeSpan(8, 15, 0) },
        { DayOfWeek.Friday, new TimeSpan(5, 30, 0) },
    };

    [ObservableProperty]
    [JsonProperty(nameof(StartTime), Required = Required.DisallowNull)]
    internal partial TimeSpan StartTime { get; set; } = new(6, 45, 0);

    [JsonProperty(nameof(Language))]
    private string? _Language
    {
        get => Language?.ToString();
        init => Language = string.IsNullOrEmpty(value) ? null : CultureInfo.CreateSpecificCulture(value);
    }
}
