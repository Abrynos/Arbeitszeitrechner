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
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Arbeitszeitrechner.ViewModels;

internal sealed partial class MainWindowViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    private const string DbFilePath = "./app.db";

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Formatting = Formatting.Indented,
        Culture = CultureInfo.InvariantCulture,
    };

    private static readonly TimeSpan MaxTimeBeforeBreak = TimeSpan.FromHours(6);
    private static readonly TimeSpan MinBreak = TimeSpan.FromMinutes(30);

    public TimeSpan EndTime => Database.StartTime
                               + TargetWorkHours
                               + (TargetWorkHours > MaxTimeBeforeBreak
                                   ? MinBreak
                                   : TimeSpan.Zero);

    public TimeSpan IsTime
    {
        get
        {
            TimeSpan diff = DateTime.Now.TimeOfDay - Database.StartTime;
            if (diff > MaxTimeBeforeBreak)
            {
                diff -= MinBreak;
            }

            return diff;
        }
    }

    private readonly AppDatabaseViewModel Database;

    private readonly SemaphoreSlim DatabaseFileSemaphore = new(1, 1);

    private readonly Timer IsTimeTimer;

    private TimeSpan TargetWorkHours => Database.TimeOverrides?.TryGetValue(DateTime.Today.DayOfWeek, out TimeSpan @override) ?? false
        ? @override
        : DateTime.Today.DayOfWeek
            switch
            {
                DayOfWeek.Friday => new TimeSpan(5, 30, 0),
                _ => new TimeSpan(8, 15, 0),
            };

    public int StartHours
    {
        get => Database.StartTime.Hours;
        set
        {
            Database.StartTime = new TimeSpan(value, Database.StartTime.Minutes, 0);
            OnStartTimeChanged();
        }
    }

    public int StartMinutes
    {
        get => Database.StartTime.Minutes;
        set
        {
            Database.StartTime = new TimeSpan(Database.StartTime.Hours, value, 0);
            OnStartTimeChanged();
        }
    }

    public MainWindowViewModel()
    {
        Database = LoadOrCreateDatabase();
        if (Database.Language != null)
        {
            CultureInfo.CurrentCulture = Database.Language;
            CultureInfo.CurrentUICulture = Database.Language;
        }

        Database.PropertyChanged += OnDatabaseChanged;
        IsTimeTimer = new Timer(OnIsTimeChanged, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        DatabaseFileSemaphore.Dispose();
        IsTimeTimer.Dispose();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        DatabaseFileSemaphore.Dispose();
        return IsTimeTimer.DisposeAsync();
    }

    private static AppDatabaseViewModel LoadOrCreateDatabase()
    {
        if (!File.Exists(DbFilePath))
        {
            return new AppDatabaseViewModel();
        }

        AppDatabaseViewModel? db = null;
        try
        {
            string content = File.ReadAllText(DbFilePath);
#pragma warning disable IL2026 // We only need to deserialize properties we actually work with
            db = JsonConvert.DeserializeObject<AppDatabaseViewModel>(content, JsonSettings);
#pragma warning restore IL2026
        }
        catch (Exception) { }

        return db ?? new AppDatabaseViewModel();
    }

    private void OnDatabaseChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!DatabaseFileSemaphore.Wait(TimeSpan.Zero))
        {
            return; // There is already a save in progress
        }

        Task.Run(async () =>
        {
            await Task.Delay(400).ConfigureAwait(false);
            try
            {
#pragma warning disable IL2026 // We only need to serialize properties we actually work with
                await File.WriteAllTextAsync(DbFilePath, JsonConvert.SerializeObject(Database, JsonSettings)).ConfigureAwait(false);
#pragma warning restore IL2026
            }
            catch (Exception) { }
            finally
            {
                DatabaseFileSemaphore.Release();
            }
        });
    }

    private void OnIsTimeChanged(object? _)
    {
        OnPropertyChanged(nameof(IsTime));
        OnPropertyChanged(nameof(TargetWorkHours)); // in case we only hibernate/sleep over night and the next day is a friday
    }

    private void OnStartTimeChanged()
    {
        OnPropertyChanged(nameof(StartHours));
        OnPropertyChanged(nameof(StartMinutes));
        OnPropertyChanged(nameof(IsTime));
        OnPropertyChanged(nameof(EndTime));
    }
}
