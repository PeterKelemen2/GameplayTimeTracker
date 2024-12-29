using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace GameplayTimeTracker
{
    public class Entry : INotifyPropertyChanged
    {
        private string _name;
        private string _exePath;
        private string _iconPath;
        private string _arguments;
        private double _totalTime;
        private double _lastTime;
        private double _totalPerc;
        private double _lastPerc;
        private int[] _totalArray = new int[3]; // H M S
        private int[] _lastArray = new int[3];
        private bool _isRunning;
        private DateTime _lastDate;
        private bool _wasRunning;

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
                    OnPropertyChanged(nameof(TotalPlayFormatted)); // Ensure this is called to notify changes
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
        public string RunningFormatted =>
            IsRunning ? "Running!" : "";

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
            set => SetField(ref _iconPath, value);
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
                if (_isRunning != value)
                {
                    _isRunning = value;
                    OnPropertyChanged(nameof(IsRunning));
                }
            }
        }

        [JsonIgnore]
        public bool WasRunning
        {
            get => _wasRunning;
            set => SetField(ref _wasRunning, value);
        }

        // [JsonIgnore]
        // public EntryRepository EntryRepo { get; set; }

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
            // return TotalH + (TotalM / 60) + (TotalS / 3600);
        }

        public double GetLastPlaytimeAsDouble()
        {
            return Math.Round(LastPlay[0] + LastPlay[1] / 60.0 + LastPlay[2] / 3600.0, 2);
        }

        public override string ToString()
        {
            int[] p = Utils.p;
            return
                $" | {Utils.Truncate(Name, p[0])}" +
                $" | {Utils.Truncate(string.Join(", ", TotalPlay), p[1])}" +
                $" | {Utils.Truncate(string.Join(", ", LastPlay), p[2])}" +
                $" | {Utils.Truncate(TotalPerc.ToString(), p[3])}" +
                $" | {Utils.Truncate(LastPerc.ToString(), p[4])}" +
                $" | {Utils.Truncate(LastDate.ToString("yyyy-MM-dd"), p[5])}" +
                $" | {Utils.Truncate(Path.GetFileName(ExePath), p[6])}" +
                $" | {Utils.Truncate(Path.GetFileName(IconPath), p[7])}" +
                $" | {Utils.Truncate(Arguments, p[8])} |";
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
            Console.WriteLine($"PropertyChanged: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}