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
            ColorIndex = 0;
        }

        public void Update(Dictionary<Section, double> dataDictionary)
        {
            foreach (var data in dataDictionary)
            {
                var section = data.Key;
                var value = data.Value;
                if (!SeriesDictionary.ContainsKey(section))
                    AddSeries(section);
                ChartValues<double> Values = (ChartValues<double>)SeriesDictionary[section].Values;
                Values.Add(value);
                var first = Values.DefaultIfEmpty(0).FirstOrDefault();
                if (Values.Count > MAX_COUNT - 1) Values.Remove(first);
                if (Values.Count < MAX_COUNT) Values.Add(value);
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

        private static byte[][] RGBs = new byte[][]
        {
            new byte[3]{255,0,0},
            new byte[3]{0,255,0},
            new byte[3]{0,0,255},
            new byte[3]{128,128,0},
            new byte[3]{128,0,128},
            new byte[3]{0,128,128},
            new byte[3]{0,64,192},
            new byte[3]{0,192,64},
            new byte[3]{192,0,64},
            new byte[3]{255,255,0},
            new byte[3]{255,0,255},
            new byte[3]{0,255,255},
            new byte[3]{64,0,192},
            new byte[3]{64,192,0},
            new byte[3]{192,64,0}
        };
        static int ColorIndex;
        private static Brush getNextColor()
        {
            ColorIndex++;
            return new SolidColorBrush(Color.FromRgb(RGBs[ColorIndex % 15][0], RGBs[ColorIndex % 15][1], RGBs[ColorIndex % 15][2]));
        }

        public void AddSeries(Section section)
        {
            SeriesDictionary.Add(section, new LineSeries
            {
                Values = new ChartValues<double>(),
                Stroke = getNextColor(),
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
