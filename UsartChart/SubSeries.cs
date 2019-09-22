using LiveCharts;
using LiveCharts.Defaults;
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
            ColorIndex = 0;
        }

        static readonly DateTime LaunchTime = DateTime.Now;
        public void Update(Dictionary<Section, double> dataDictionary)
        {
            foreach (var data in dataDictionary)
            {
                var section = data.Key;
                var value = data.Value;
                if (!SeriesDictionary.ContainsKey(section))
                    AddSeries(section);
                ChartValues<ObservablePoint> Values = (ChartValues<ObservablePoint>)SeriesDictionary[section].Values;
                var first = Values.FirstOrDefault();
                if (Values.Count > MAX_COUNT - 1) Values.Remove(first);
                if (Values.Count < MAX_COUNT) Values.Add(new ObservablePoint(
                        (DateTime.Now - LaunchTime).TotalSeconds,
                        value
                    ));
            }
            if (dataDictionary.Count != SeriesDictionary.Count)
            {
                foreach (var series in SeriesDictionary)
                {
                    if (!dataDictionary.ContainsKey(series.Key))
                        DeleteSeries(series.Key);
                }
            }
        }

        private static readonly byte[][] RGBs = new byte[][] 
        {
            new byte[3]{72,72,72},
            new byte[3]{255,0,0},
            new byte[3]{255,128,0},
            new byte[3]{200,200,0},
            new byte[3]{0,200,0},
            new byte[3]{0,200,200},
            new byte[3]{0,0,255},
            new byte[3]{200,0,200},
            new byte[3]{128,128,128},
            new byte[3]{128,0,0},
            new byte[3]{128,128,0},
            new byte[3]{0,128,0},
            new byte[3]{0,128,128},
            new byte[3]{0,0,128}
        };
        static int ColorIndex;
        private static Brush GetNextColor()
        {
            ColorIndex++;
            return new SolidColorBrush(Color.FromRgb(
                RGBs[ColorIndex % RGBs.Length][0],
                RGBs[ColorIndex % RGBs.Length][1],
                RGBs[ColorIndex % RGBs.Length][2]
                ));
        }

        public void AddSeries(Section section)
        {
            SeriesDictionary.Add(section, new LineSeries
            {
                Values = new ChartValues<ObservablePoint>(),
                Stroke = GetNextColor(),
                Fill = Brushes.Transparent,
                StrokeThickness = .5,
                PointGeometry = null,
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
