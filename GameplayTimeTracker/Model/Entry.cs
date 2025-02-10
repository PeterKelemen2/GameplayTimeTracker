using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GameplayTimeTracker.SGDB;
using Application = System.Windows.Application;

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
        private int[] _totalArray = new int[3]; // H M S
        private int[] _lastArray = new int[3];
        private bool _isRunning;
        private string _runningString;
        private string _lastPlayStateString = "Started: ";
        private string _localSavePath = "";
        private bool _isSavePathValid = true;
        private bool _isRemoteSaveEnabled = false;
        public bool _isEditing = false;

        private DateTime _lastDate;

        // private Dictionary<DateTime, int[]> playTimeHistory = new();
        private string _lastDateString = "Never";
        private bool _wasRunning = false;
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
            get => _totalArray;
            set
            {
                if (SetField(ref _totalArray, value))
                {
                    OnPropertyChanged(nameof(TotalPlayFormatted));
                    if (Repository != null)
                    {
                        Repository.UpdateTotalPercentages();
                        LastPerc = Math.Round(GetLastPlaytimeAsDouble() / GetTotalPlaytimeAsDouble(), 2);
                        // Repository.PrintEntryList();
                    }
                }
            }
        }

        [JsonIgnore]
        public string TotalPlayFormatted =>
            TotalPlay != null && TotalPlay.Length == 3
                ? $"{TotalPlay[0]}h {TotalPlay[1]}m {TotalPlay[2]}s"
                : "0h 0m 0s";

        [JsonPropertyName("lastPlay")]
        public int[] LastPlay
        {
            get => _lastArray;
            set
            {
                SetField(ref _lastArray, value);
                OnPropertyChanged(nameof(LastPlayFormatted));
            }
        }

        [JsonIgnore]
        public string LastPlayFormatted =>
            LastPlay != null && LastPlay.Length == 3
                ? $"{LastPlay[0]}h {LastPlay[1]}m {LastPlay[2]}s"
                : "0h 0m 0s";

        [JsonIgnore]
        public string RunningFormatted
        {
            get => _runningString;
            set { SetField(ref _runningString, value); }
        }

        [JsonIgnore]
        public string LastRunningStateFormatted
        {
            get => _lastPlayStateString;
            set { SetField(ref _lastPlayStateString, value); }
        }

        [JsonPropertyName("exePath")]
        public string ExePath
        {
            get => _exePath;
            set
            {
                SetField(ref _exePath, value);
                IsLaunchable = File.Exists(_exePath) && Path.GetExtension(_exePath).ToLower() == ".exe";
                Console.WriteLine($"Launchable: {IsLaunchable}");
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
            set { SetField(ref _totalPerc, value); }
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
        public string LastDateFormatted =>
            LastDate.Year > 1000
                ? (LastDate.Date == DateTime.Now.Date
                    ? $"Today, {LastDate.ToString("HH:mm")}"
                    : LastDate.ToString("yyyy.MM.dd HH:mm"))
                : "Never";

        [JsonIgnore]
        public bool IsRunning
        {
            get => _runningString == "Running!";
            set
            {
                if (value != IsRunning)
                {
                    _runningString = value ? "Running!" : "";
                    _lastPlayStateString = value ? "Started: " : "Ended: ";
                    _wasRunning = value;
                    LastDate = DateTime.Now;

                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(RunningFormatted));
                    OnPropertyChanged(nameof(LastDateFormatted));
                    OnPropertyChanged(nameof(LastRunningStateFormatted));

                    _repository?.UpdateRunningEntryCount();

                    if (value)
                    {
                        ResetLastPlaytime();
                        ((MainWindow)Application.Current.MainWindow).MainScrollViewer.ScrollToTop();
                    }
                    else
                    {
                        IncrementTodaysHistory();
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

        public void IncrementTodaysHistory(bool toSave = true)
        {
            if (PlaytimeHistory.ContainsKey(DateTime.Today))
            {
                PlaytimeHistory[DateTime.Today] =
                    Common.NormalizeTimeArray(Common.AddTimeArrays(PlaytimeHistory[DateTime.Today],
                        LastPlay));
                Console.WriteLine($"Todays play time: {PlaytimeHistory[DateTime.Today]}");
            }

            if (toSave)
            {
                DataHandler.WriteEntriesToFile(_repository.EntriesList, AppFiles.DataFilePath);
            }
        }

        [JsonIgnore]
        public bool WasRunning
        {
            get => _wasRunning;
            set => SetField(ref _wasRunning, value);
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
            if (_isRemoteSaveEnabled && Common.Settings.IsRemoteSavingEnabled &&
                !string.IsNullOrWhiteSpace(_localSavePath))
            {
                RemoteSave();
            }
        }

        public async void RemoteSave()
        {
            if (IsRunning) return;

            string uploadPath =
                $"{Common.Settings.RemoteMachine.RemoteFolder.TrimEnd('/')}/{Name}/{DateTime.Now:yyyy-MM-dd-HH-mm-ss}";
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
            // LastDate = DateTime.Now;
        }

        public void IncrementTime()
        {
            IncTArray(LastPlay);
            IncTArray(TotalPlay);
        }

        private void IncTArray(int[] arr)
        {
            int[] newArray = (int[])arr.Clone(); // Clone the array
            newArray[2]++; // Increment seconds
            newArray = Common.NormalizeTimeArray(newArray);

            if (arr == LastPlay)
                LastPlay = newArray; // Reassign to trigger notification
            else if (arr == TotalPlay)
                TotalPlay = newArray; // Reassign to trigger notification
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
            // AppSettings settings = DataHandler.GetSettingsFromFile();
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