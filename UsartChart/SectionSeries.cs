using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UsartChart
{
    public class SectionSeries : INotifyPropertyChanged
    {
        public SectionSeries()
        {
            Series = new SeriesCollection();
            Formatter = x => x.ToString("F0");
        }

        public void AddSeries(uint address)
        {
            SectionDictionary.Add(address, new LineSeries
            {
                Values = new ChartValues<double>(),
                Fill = Brushes.Transparent,
                StrokeThickness = .5,
                PointGeometry = null
            });
            Series.Add(SectionDictionary[address]);
        }

        public void DeleteSeries(uint address)
        {
            SectionDictionary.Remove(address);
            Series.Remove(SectionDictionary[address]);
        }

        public void AddValue(uint address, double value)
        {
            if (!SectionDictionary.ContainsKey(address))
                AddSeries(address);
            ChartValues<double> Values = (ChartValues<double>)SectionDictionary[address].Values;
            Values.Add(value);
            var first = Values.DefaultIfEmpty(0).FirstOrDefault();
            if (Values.Count > MAX_COUNT - 1) Values.Remove(first);
            if (Values.Count < MAX_COUNT) Values.Add(value);
        }

        public Func<double, string> Formatter { get; set; }

        const uint MAX_COUNT = 1000;
        private readonly Dictionary<uint, LineSeries> SectionDictionary = new Dictionary<uint, LineSeries>();
        public SeriesCollection Series { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
