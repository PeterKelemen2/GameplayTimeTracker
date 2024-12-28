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

        [JsonPropertyName("totalPlay")]
        public int[] TotalPlay
        {
            get => _totalArray;
            set => SetField(ref _totalArray, value);
        }

        [JsonPropertyName("lastPlay")]
        public int[] LastPlay
        {
            get => _lastArray;
            set => SetField(ref _lastArray, value);
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
            set => SetField(ref _isRunning, value);
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
        }

        public void IncrementTime()
        {
            IncTArray(LastPlay);
            IncTArray(TotalPlay);
        }

        private void IncTArray(int[] arr)
        {
            double min = 60 - 1;
            arr[2]++; // Seconds
            if (arr[2] > min)
            {
                arr[2] = 0;
                arr[1]++; // Minutes
                if (arr[1] > min)
                {
                    arr[1] = 0;
                    arr[0]++; // Hours
                }
            }
        }

        public double GetTotalPlaytimeAsDouble()
        {
            return TotalPlay[0] + (TotalPlay[1] / 60) + (TotalPlay[2] / 3600);
            // return TotalH + (TotalM / 60) + (TotalS / 3600);
        }

        public double GetLastPlaytimeAsDouble()
        {
            return LastPlay[0] + (LastPlay[1] / 60) + (LastPlay[2] / 3600);
        }

        public override string ToString()
        {
            int[] p = Utils.p;
            return
                $" | {Utils.Truncate(Name, p[0])}" +
                $" | {Utils.Truncate(string.Join(", ", TotalPlay), p[1])}" +
                $" | {Utils.Truncate(string.Join(", ", LastPlay), p[2])}" +
                // $" | {Utils.Truncate(TotalTime.ToString(), p[1])}" +
                // $" | {Utils.Truncate(LastTime.ToString(), p[2])}" +
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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}