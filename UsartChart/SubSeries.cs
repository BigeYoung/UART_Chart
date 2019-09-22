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
            Series_Collection = new SeriesCollection();
            Formatter = x => x.ToString("F0");
            ColorIndex = 0;
        }

        static readonly DateTime LaunchTime = DateTime.Now;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataList">Name, Addr, Value</param>
        public void Update(List<SectionData> dataList)
        {
            foreach (var data in dataList)
            {
                LineSeries series = (LineSeries)Series_Collection.Where((x) => (uint)((LineSeries)x).Tag == data.Addr).SingleOrDefault();
                if (series == null)
                {
                    series = new LineSeries
                    {
                        Name = data.Name.Replace(".","_"),
                        Tag = data.Addr,
                        Values = new ChartValues<ObservablePoint>(),
                        Stroke = GetNextColor(),
                        Fill = Brushes.Transparent,
                        StrokeThickness = .5,
                        PointGeometry = null,
                    };
                    Series_Collection.Add(series);
                }
                ChartValues<ObservablePoint> Values = (ChartValues<ObservablePoint>)series.Values;
                if (Values.Count > MAX_COUNT - 1)
                    Values.Remove(Values.First());
                else if (Values.Count < MAX_COUNT)
                    Values.Add(new ObservablePoint(
                        (DateTime.Now - LaunchTime).TotalSeconds,
                        data.Value
                    ));
            }
            if (dataList.Count < Series_Collection.Count)
            {
                var UnfoundSeries = Series_Collection.Where((series) => !dataList.Any((data) => data.Addr == (uint)((LineSeries)series).Tag));
                foreach (var item in UnfoundSeries)
                {
                    Series_Collection.Remove(item);
                }
            }
        }

        private static readonly byte[][] RGBs = new byte[][]
        {
            new byte[3]{36,36,36},
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

        public Func<double, string> Formatter { get; set; }

        const uint MAX_COUNT = 1000;
        public SeriesCollection Series_Collection { get; set; } = new SeriesCollection();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
