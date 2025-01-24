using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GameplayTimeTracker.Settings;
using GameplayTimeTracker.SGDB;

namespace GameplayTimeTracker
{
    public class Entry : INotifyPropertyChanged
    {
        private string _name;
        private string _exePath;
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

        private DateTime _lastDate;

        // private Dictionary<DateTime, int[]> playTimeHistory = new();
        private string _lastDateString = "Never";
        private bool _wasRunning;
        private EntryRepository _repository;


        [JsonIgnore]
        public EntryRepository Repository
        {
            get => _repository;
            set => SetField(ref _repository, value);
        }

        [JsonPropertyName("gameName")]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
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
                        Repository.PrintEntryList();
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
                if (SetField(ref _lastArray, value))
                {
                    // Notify that the formatted playtime has changed
                    OnPropertyChanged(nameof(LastPlayFormatted));
                }
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
            set
            {
                if (_runningString != value)
                {
                    _runningString = value;
                    OnPropertyChanged(nameof(RunningFormatted));
                }
            }
        }

        [JsonIgnore]
        public string LastRunningStateFormatted
        {
            get => _lastPlayStateString;
            set
            {
                if (_lastPlayStateString != value)
                {
                    _lastPlayStateString = value;
                    OnPropertyChanged(nameof(LastRunningStateFormatted));
                }
            }
        }

        [JsonPropertyName("exePath")]
        public string ExePath
        {
            get => _exePath;
            set => SetField(ref _exePath, value);
        }

        [JsonPropertyName("arguments")]
        public string Arguments
        {
            get => _arguments;
            set => SetField(ref _arguments, value);
        }

        [JsonPropertyName("iconPath")]
        public string IconPath
        {
            get => _iconPath;
            set
            {
                string imagePath = File.Exists(value) ? value : AppFiles.DefaultIconPath;
                SetField(ref _iconPath, imagePath);
            }
        }

        [JsonPropertyName("heroPath")]
        public string HeroPath
        {
            get => _heroPath;
            set
            {
                string imagePath = File.Exists(value) ? value : AppFiles.DefaultHeroPath;
                SetField(ref _heroPath, imagePath);
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
            set
            {
                if (_totalPerc != value)
                {
                    _totalPerc = value;
                    OnPropertyChanged(nameof(TotalPerc));
                }
            }
        }

        [JsonIgnore]
        public double LastPerc
        {
            get => _lastPerc;
            // set => SetField(ref _lastPerc, value);
            set
            {
                if (_lastPerc != value)
                {
                    _lastPerc = value;
                    OnPropertyChanged(nameof(LastPerc));
                }
            }
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
                _runningString = value ? "Running!" : "";
                _lastPlayStateString = value ? "Started: " : "Ended: ";
                LastDate = value ? DateTime.Now : LastDate;

                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(RunningFormatted));
                OnPropertyChanged(nameof(LastDateFormatted));
                OnPropertyChanged(nameof(LastRunningStateFormatted));
            }
        }

        [JsonIgnore]
        public bool WasRunning
        {
            get => _wasRunning;
            set => SetField(ref _wasRunning, value);
        }

        [JsonPropertyName("Playtime History")] private Dictionary<DateTime, int[]> PlaytimeHistory { get; set; }

        public void EnsureLastWeekData()
        {
            PlaytimeHistory = new Dictionary<DateTime, int[]>();
            DateTime today = DateTime.Today;
            DateTime weekAgo = today.AddDays(-6);

            // Remove entries older than a week
            var filteredHistory = PlaytimeHistory
                .Where(entry => entry.Key >= weekAgo)
                .ToDictionary(entry => entry.Key, entry => entry.Value);

            // Ensure last 7 days are present
            Random random = new Random();

            for (int i = 0; i < 7; i++)
            {
                DateTime date = today.AddDays(-i);
                if (!filteredHistory.ContainsKey(date))
                {
                    filteredHistory[date] = new int[]
                    {
                        random.Next(0, 23), // Index 0 initialized to 0
                        random.Next(0, 60), // Index 1 with a random value between 0-59
                        random.Next(0, 60) // Index 2 with a random value between 0-59
                    };
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
            LastDate = DateTime.Now;
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
            if (newArray[2] >= 60)
            {
                newArray[2] = 0;
                newArray[1]++; // Increment minutes
                if (newArray[1] >= 60)
                {
                    newArray[1] = 0;
                    newArray[0]++; // Increment hours
                }
            }

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
            // Console.WriteLine($"Entry - PropertyChanged: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PrintRepo()
        {
            Repository.PrintEntryList();
        }

        public async Task RefreshImagesFromSGDB()
        {
            AppSettings settings = DataHandler.GetSettingsFromFile();
            Dictionary<string, string> iconFiles = SGDBFileHandler.GetSGDBFiles();

            if (!settings.SGDBApiKey.Equals(string.Empty))
            {
                await SGDBFetch.FetchSGDBAsync(settings.SGDBApiKey, Name, iconFiles);
            }

            IconPath = iconFiles["icon"];
            HeroPath = iconFiles["hero"];
        }
    }
}