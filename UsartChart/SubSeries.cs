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
    public class SubSeries : INotifyPropertyChanged
    {
        public SubSeries()
        {
            Series = new SeriesCollection();
            Formatter = x => x.ToString("F0");
        }

        public void Update(List<Tuple<Section, double>> dataList)
        {
            foreach (var data in dataList)
            {
                var section = data.Item1;
                var value = data.Item2;
                if (!SeriesDictionary.ContainsKey(section))
                    AddSeries(section);
                ChartValues<double> Values = (ChartValues<double>)SeriesDictionary[section].Values;
                Values.Add(value);
                var first = Values.DefaultIfEmpty(0).FirstOrDefault();
                if (Values.Count > MAX_COUNT - 1) Values.Remove(first);
                if (Values.Count < MAX_COUNT) Values.Add(value);
            }
            if (dataList.Count != SeriesDictionary.Count)
            {
                foreach (var series in SeriesDictionary)
                {
                    if (!dataList.Where(x=>x.Item1 == series.Key).Any())
                    {
                        DeleteSeries(series.Key);
                    }
                }
            }
        }

        public void AddSeries(Section section)
        {
            SeriesDictionary.Add(section, new LineSeries
            {
                Values = new ChartValues<double>(),
                Fill = Brushes.Transparent,
                StrokeThickness = .5,
                PointGeometry = null
            });
            Series.Add(SeriesDictionary[section]);
            OnPropertyChanged("SeriesDictionary");
        }

        public void DeleteSeries(Section section)
        {
            SeriesDictionary.Remove(section);
            Series.Remove(SeriesDictionary[section]);
        }

        public Func<double, string> Formatter { get; set; }

        const uint MAX_COUNT = 1000;
        public Dictionary<Section, LineSeries> SeriesDictionary { get; set; } = new Dictionary<Section, LineSeries>();
        public SeriesCollection Series { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
