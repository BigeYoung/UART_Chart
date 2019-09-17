using LiveCharts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsartChart
{
    public class SpeedTestVm : INotifyPropertyChanged
    {
        private double _trend;
        private double _count;
        private double _currentLecture;

        public SpeedTestVm()
        {
            Values = new ChartValues<double>();
            ReadCommand = new RelayCommand(Read);
            StopCommand = new RelayCommand(Stop);
            CleaCommand = new RelayCommand(Clear);
         
            //the formatter or labels property is shared 
            Formatter = x => x.ToString("F0");
        }

        public bool IsReading { get; set; }
        public RelayCommand ReadCommand { get; set; }
        public RelayCommand StopCommand { get; set; }
        public RelayCommand CleaCommand { get; set; }
        public ChartValues<double> Values { get; set; }

        public double Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }

        public double CurrentLecture
        {
            get { return _currentLecture; }
            set
            {
                _currentLecture = value;
                OnPropertyChanged("CurrentLecture");
            }
        }
      
        private void Stop()
        {
            IsReading = false;
        }

        private void Clear()
        {
            Values.Clear();
        }

        private async void Read()
        {
            if (IsReading) return;

            //lets keep in memory only the last 20000 records,
            //to keep everything running faster
            const int keepRecords = 20000;
            IsReading = true;

            while (IsReading)
            {
                var r = new Random();
                _trend += (r.NextDouble() < 0.5 ? 1 : -1) * r.Next(0, 10);
                //when multi threading avoid indexed calls like -> Values[0] 
                //instead enumerate the collection
                //ChartValues/GearedValues returns a thread safe copy once you enumerate it.
                //TIPS: use foreach instead of for
                //LINQ methods also enumerate the collections
                var first = Values.DefaultIfEmpty(0).FirstOrDefault();
                if (Values.Count > keepRecords - 1) Values.Remove(first);
                if (Values.Count < keepRecords) Values.Add(_trend);
                Count = Values.Count;
                CurrentLecture = _trend;
                await Task.Delay(10);
            }
        }
        
        public Func<double, string> Formatter { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
