using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using GameplayTimeTracker.SGDB;
using Application = System.Windows.Application;
using DateTime = System.DateTime;
using Task = System.Threading.Tasks.Task;

namespace GameplayTimeTracker
{
    public class Entry : INotifyPropertyChanged
    {
        private string _name;
        private string _exePath;
        private bool _isLaunchable = true;
        private string _iconPath = AppFiles.DefaultIconPath;
        private string _heroPath = AppFiles.DefaultHeroPath;
        private string _arguments;
        private double _totalTime;
        private double _lastTime;
        private double _totalPerc;
        private double _lastPerc;
        private int[] _totalPlayArray = new int[3]; // H M S
        private int[] _lastPlayArray = new int[3];
        private bool _isRunning = false;
        private string _localSavePath = "";
        private bool _isSavePathValid = true;
        private bool _isRemoteSaveEnabled = false;
        private int _remoteSaveAfterMinutes = 60;
        private int _retainSavesForDays = 30;
        public bool _isEditing = false;
        private DateTime _prevDate;
        private DateTime _lastDate;
        private EntryRepository _repository;

        [JsonIgnore]
        public EntryRepository Repository
        {
            get => _repository;
            set => SetField(ref _repository, value);
        }

        [JsonIgnore]
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetField(ref _isEditing, value);
                if (!_isEditing) Name = Name.Trim();
            }
        }


        [JsonPropertyName("gameName")]
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
                InitSave();
            }
        }

        public int[] TotalPlay
        {
            get => _totalPlayArray;
            set
            {
                SetField(ref _totalPlayArray, value);
                if (Repository != null)
                {
                    Repository.UpdateTotalPercentages();
                    LastPerc = Math.Round(GetLastPlaytimeAsDouble() / GetTotalPlaytimeAsDouble(), 2);
                    // Repository.PrintEntryList();
                }
            }
        }

        [JsonPropertyName("lastPlay")]
        public int[] LastPlay
        {
            get => _lastPlayArray;
            set { SetField(ref _lastPlayArray, value); }
        }

        [JsonPropertyName("exePath")]
        public string ExePath
        {
            get => _exePath;
            set
            {
                SetField(ref _exePath, value);
                IsLaunchable = File.Exists(_exePath) && Path.GetExtension(_exePath).ToLower() == ".exe";
                InitSave();
            }
        }

        [JsonIgnore]
        public bool IsLaunchable
        {
            get => _isLaunchable;
            set { SetField(ref _isLaunchable, value); }
        }

        [JsonIgnore]
        public bool IsSavePathValid
        {
            get => _isSavePathValid;
            set { SetField(ref _isSavePathValid, value); }
        }

        [JsonPropertyName("arguments")]
        public string Arguments
        {
            get => _arguments;
            set
            {
                SetField(ref _arguments, value);
                InitSave();
            }
        }

        [JsonPropertyName("isRemoteSaveEnabled")]
        public bool IsRemoteSaveEnabled
        {
            get => _isRemoteSaveEnabled;
            set
            {
                SetField(ref _isRemoteSaveEnabled, value);
                InitSave();
            }
        }

        [JsonPropertyName("remoteSaveAfterMinutes")]
        public int RemoteSaveAfterMinutes
        {
            get => _remoteSaveAfterMinutes;
            set
            {
                SetField(ref _remoteSaveAfterMinutes, value);
                if (value < 0) _remoteSaveAfterMinutes = 0;
                InitSave();
            }
        }

        [JsonPropertyName("retainSaveForDays")]
        public int RetainSaveForDays
        {
            get => _retainSavesForDays;
            set
            {
                SetField(ref _retainSavesForDays, value);
                if (value < 7) _retainSavesForDays = 7;
                InitSave();
            }
        }

        [JsonPropertyName("localSavePath")]
        public string LocalSavePath
        {
            get => _localSavePath;
            set
            {
                SetField(ref _localSavePath, value);
                IsSavePathValid = Directory.Exists(_localSavePath);
                Console.WriteLine($"Entry - IsSavePathValid: {_isSavePathValid}");
                InitSave();
            }
        }

        [JsonPropertyName("iconPath")]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                if (value == _iconPath || !File.Exists(value)) return;
                string newIconPath = value;
                try
                {
                    if (Path.GetExtension(value).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        string baseName = $"{Name.Replace(" ", "_")}_{Guid.NewGuid()}";
                        newIconPath = Path.Combine(AppFiles.SavedImagesPath, $"{baseName}_icon.png");
                        ImageHelper.SaveIconFromExe(value, newIconPath);
                    }

                    SetField(ref _iconPath, newIconPath);
                    InitSave();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    SetField(ref _iconPath, AppFiles.DefaultIconPath);
                    InitSave();
                }
            }
        }

        [JsonPropertyName("heroPath")]
        public string HeroPath
        {
            get => _heroPath;
            set
            {
                if (value == _heroPath || !File.Exists(value)) return;

                try
                {
                    string newHeroPath = value;
                    if (Path.GetExtension(value).Equals(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        string baseName = $"{Name.Replace(" ", "_")}_{Guid.NewGuid()}";
                        string auxImagePath = Path.Combine(AppFiles.SavedImagesPath, $"{baseName}_auxicon.png");
                        newHeroPath = Path.Combine(AppFiles.SavedImagesPath, $"{baseName}_hero.png");

                        ImageHelper.SaveIconFromExe(value, auxImagePath);
                        ImageHelper.ScatterImage(auxImagePath, newHeroPath);
                        File.Delete(auxImagePath);
                    }

                    SetField(ref _heroPath, newHeroPath);
                    InitSave();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    SetField(ref _heroPath, AppFiles.DefaultHeroPath);
                    InitSave();
                }
            }
        }


        [JsonPropertyName("totalTime")]
        public double TotalTime
        {
            get => _totalTime;
            set => SetField(ref _totalTime, value);
        }

        [JsonPropertyName("lastPlayedTime")]
        public double LastTime
        {
            get => _lastTime;
            set => SetField(ref _lastTime, value);
        }

        [JsonIgnore]
        public double TotalPerc
        {
            get => _totalPerc;
            set => SetField(ref _totalPerc, value);
        }

        [JsonIgnore]
        public double LastPerc
        {
            get => _lastPerc;
            set => SetField(ref _lastPerc, value);
        }

        [JsonPropertyName("lastPlayDate")]
        public DateTime LastDate
        {
            get => _lastDate;
            set => SetField(ref _lastDate, value);
        }

        [JsonIgnore]
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (value != _isRunning)
                {
                    _isRunning = value;
                    _prevDate = LastDate;
                    LastDate = DateTime.Now;

                    OnPropertyChanged(nameof(IsRunning));
                    _repository?.UpdateRunningEntryCount();

                    if (value)
                    {
                        ResetLastPlaytime();
                        ((MainWindow)Application.Current.MainWindow).MainScrollViewer.ScrollToTop();
                    }
                    else
                    {
                        IncrementPlaytimeHistory();
                        InitSave();
                        InitRemoteSave();
                    }

                    if (_repository != null)
                    {
                        _repository.SortEntries();
                        TaskbarManager.UpdateTrayEntries();
                    }
                }
            }
        }

        public void IncrementPlaytimeHistory(bool toSave = true)
        {
            EnsureLastWeekData();
            DateTime today = DateTime.Today;

            int daysBetween = (LastDate.Date - _prevDate.Date).Days;

            if (daysBetween >= 1) // Session spans multiple days
            {
                DateTime currentDay = _prevDate.Date;
                DateTime midnight = currentDay.AddDays(1); // Midnight of the next day

                // First day: from _prevDate to midnight
                TimeSpan toMid = midnight - _prevDate;
                int[] firstDayTime = Common.NormalizeTime(new[] { toMid.Hours, toMid.Minutes, toMid.Seconds });

                if (PlaytimeHistory.ContainsKey(currentDay))
                {
                    PlaytimeHistory[currentDay] = Common.NormalizeTime(
                        Common.AddTimeArrays(PlaytimeHistory[currentDay], firstDayTime));
                }
                else
                {
                    PlaytimeHistory[currentDay] = firstDayTime;
                }

                // Full days between _prevDate and LastDate
                for (int i = 1; i < daysBetween; i++)
                {
                    currentDay = _prevDate.Date.AddDays(i);
                    if (!PlaytimeHistory.ContainsKey(currentDay))
                    {
                        PlaytimeHistory[currentDay] = new[] { 24, 0, 0 };
                    }

                    int[] fullDayTime = new[] { 24, 0, 0 }; // Full 24 hours
                    PlaytimeHistory[currentDay] =
                        Common.NormalizeTime(Common.AddTimeArrays(PlaytimeHistory[currentDay], fullDayTime));
                }

                // Last day: from midnight to LastDate
                DateTime lastMidnight = LastDate.Date;
                TimeSpan fromMid = LastDate - lastMidnight;
                int[] lastDayTime = Common.NormalizeTime(new[] { fromMid.Hours, fromMid.Minutes, fromMid.Seconds });

                if (PlaytimeHistory.ContainsKey(lastMidnight))
                {
                    PlaytimeHistory[lastMidnight] = Common.NormalizeTime(
                        Common.AddTimeArrays(PlaytimeHistory[lastMidnight], lastDayTime));
                }
                else
                {
                    PlaytimeHistory[lastMidnight] = lastDayTime;
                }
            }
            else
            {
                // Normal case: Just add LastPlay to today's time
                PlaytimeHistory[today] = Common.NormalizeTime(
                    Common.AddTimeArrays(PlaytimeHistory[today], LastPlay));
            }

            Console.WriteLine($"Today's play time: {string.Join(":", PlaytimeHistory[today])}");

            if (toSave)
            {
                DataHandler.WriteEntriesToFile(_repository.EntriesList, AppFiles.DataFilePath);
            }
        }


        [JsonPropertyName("playtimeHistory")] public Dictionary<DateTime, int[]> PlaytimeHistory { get; set; }

        public void InitSave()
        {
            if (_repository != null)
            {
                DataHandler.WriteEntriesToFile(_repository.EntriesList, AppFiles.DataFilePath);
            }
        }

        public void InitRemoteSave()
        {
            if (_isRemoteSaveEnabled &&
                Common.Settings.IsRemoteSavingEnabled &&
                // Common.IsEntryEligibleForBackup(this) &&
                !string.IsNullOrWhiteSpace(_localSavePath))
            {
                RemoteSave();
            }
        }

        public async void RemoteSave()
        {
            if (IsRunning) return;
            if (!Directory.Exists(_localSavePath)) return;

            string savesPath = $"{Common.Settings.RemoteMachine.RemoteFolder.TrimEnd('/')}/{Name}/";
            string uploadPath = $"{savesPath}{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
            await RemoteController.DeleteSavesOlderThanDays(savesPath, _retainSavesForDays);
            await RemoteController.UploadFolderAsync(_localSavePath, uploadPath);
        }

        public void InitRemoteLoad()
        {
            if (!string.IsNullOrWhiteSpace(_localSavePath))
            {
                RemoteLoad();
            }
        }

        public async void RemoteLoad()
        {
            if (IsRunning) return;

            string remoteGameFolder = Path.Combine(Common.Settings.RemoteMachine.RemoteFolder, Name)
                .Replace("\\", "/");
            await RemoteController.DownloadFolderAsync(RemoteController.GetPathWithLatestName(remoteGameFolder),
                LocalSavePath);
        }

        public void EnsureLastWeekData()
        {
            if (PlaytimeHistory == null)
            {
                PlaytimeHistory = new Dictionary<DateTime, int[]>();
            }

            DateTime today = DateTime.Today;
            DateTime weekAgo = today.AddDays(-6);

            // Remove entries older than a week
            var filteredHistory = PlaytimeHistory
                .Where(entry => entry.Key >= weekAgo)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            // Ensure last 7 days are present
            for (int i = 0; i < 7; i++)
            {
                DateTime date = today.AddDays(-i);
                if (!filteredHistory.ContainsKey(date))
                {
                    filteredHistory[date] = new[] { 0, 0, 0 };
                }
            }

            PlaytimeHistory = filteredHistory.OrderBy(entry => entry.Key).ToDictionary(k => k.Key, v => v.Value);
        }

        public void PrintHistory()
        {
            Console.WriteLine($"\nHistory data for {Name}");
            foreach (var entry in PlaytimeHistory)
            {
                string date = entry.Key.ToString("yyyy-MM-dd");
                string playtimeData = entry.Value.Length > 0 ? string.Join(", ", entry.Value) : "No data";

                Console.WriteLine($"Date: {date}, Playtime: [{playtimeData}]");
            }
        }

        public void ResetLastPlaytime()
        {
            LastPlay = new int[3];
        }

        public void IncrementTime()
        {
            LastPlay = IncTArray(LastPlay);
            TotalPlay = IncTArray(TotalPlay);
        }

        private int[] IncTArray(int[] arr)
        {
            arr[2]++;
            return Common.NormalizeTime(arr);
        }


        public double GetTotalPlaytimeAsDouble()
        {
            return Math.Round(TotalPlay[0] + TotalPlay[1] / 60.0 + TotalPlay[2] / 3600.0, 2);
        }

        public double GetLastPlaytimeAsDouble()
        {
            return Math.Round(LastPlay[0] + LastPlay[1] / 60.0 + LastPlay[2] / 3600.0, 2);
        }

        public override string ToString()
        {
            int[] p = Common.p;
            return
                $" | {Common.Truncate(Name, p[0])}" +
                $" | {Common.Truncate(string.Join(", ", TotalPlay), p[1])}" +
                $" | {Common.Truncate(string.Join(", ", LastPlay), p[2])}" +
                $" | {Common.Truncate(TotalPerc.ToString(), p[3])}" +
                $" | {Common.Truncate(LastPerc.ToString(), p[4])}" +
                $" | {Common.Truncate(LastDate.ToString("yyyy-MM-dd"), p[5])}" +
                $" | {Common.Truncate(Path.GetFileName(ExePath), p[6])}" +
                $" | {Common.Truncate(Path.GetFileName(IconPath), p[7])}" +
                $" | {Common.Truncate(Arguments, p[8])} |";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T field, T value,
            [System.Runtime.CompilerServices.CallerMemberName]
            string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            Console.WriteLine($"Entry - PropertyChanged: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task RefreshImagesFromSGDB()
        {
            Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles(Name);

            if (!Common.Settings.SGDBApiKey.Equals(string.Empty))
            {
                await SGDBFetch.FetchSGDBAsync(Common.Settings.SGDBApiKey, Name, iconFiles);
            }

            IconPath = iconFiles["icon"];
            HeroPath = iconFiles["hero"];
        }
    }
}