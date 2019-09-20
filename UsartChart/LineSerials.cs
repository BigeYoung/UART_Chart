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
    public class LineSerials : INotifyPropertyChanged
    {
        public LineSerials()
        {
            Series = new SeriesCollection();
            Formatter = x => x.ToString("F0");
        }

        public void AddSeries(string section_name)
        {
            SectionDictionary.Add(section_name, new LineSeries
            {
                Values = new ChartValues<double>(),
                Fill = Brushes.Transparent,
                StrokeThickness = .5,
                PointGeometry = null
            });
            Series.Add(SectionDictionary[section_name]);
        }

        public void DeleteSeries(string section_name)
        {
            SectionDictionary.Remove(section_name);
            Series.Remove(SectionDictionary[section_name]);
        }

        public void AddValue(string section_name, double value)
        {
            if (!SectionDictionary.ContainsKey(section_name))
                AddSeries(section_name);
            ChartValues<double> Values = (ChartValues<double>)SectionDictionary[section_name].Values;
            Values.Add(value);
            var first = Values.DefaultIfEmpty(0).FirstOrDefault();
            if (Values.Count > MAX_COUNT - 1) Values.Remove(first);
            if (Values.Count < MAX_COUNT) Values.Add(value);
        }

        public Func<double, string> Formatter { get; set; }

        const uint MAX_COUNT = 1000;
        private Dictionary<string, LineSeries> SectionDictionary = new Dictionary<string, LineSeries>();
        public SeriesCollection Series { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
