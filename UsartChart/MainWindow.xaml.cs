using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace UsartChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SectionX.DataLabel = true;
            SectionX.StrokeThickness = 1;
            SectionX.Stroke = Brushes.Orange;
            SectionX.DisableAnimations = true;
            SectionX.DataLabelForeground = Brushes.White;

            SectionY.DataLabel = true;
            SectionY.StrokeThickness = 1;
            SectionY.Stroke = Brushes.Orange;
            SectionY.DisableAnimations = true;
            SectionY.DataLabelForeground = Brushes.White;

            m_UART.RecieveValue += (_, pair) =>
            {
                if (sections.First(x => x.Addr == pair.Item1) != null)
                    m_SectionSeries.AddValue(pair.Item1, pair.Item2);
            };

            DataContext = this;

            m_ListBox.DataContext = sections;
        }

        static readonly AxisSection SectionX = new AxisSection();
        static readonly AxisSection SectionY = new AxisSection();

        private void ChartMouseEnter(object sender, MouseEventArgs e)
        {
            if (m_AxisX.Sections.Count == 0)
                m_AxisX.Sections.Add(SectionX);

            if (m_AxisY.Sections.Count == 0)
                m_AxisY.Sections.Add(SectionY);
        }

        private void ChartMouseLeave(object sender, MouseEventArgs e)
        {
            m_AxisX.MaxValue = double.NaN;
            if (m_AxisX.MinValue < 0)
                m_AxisX.MinValue = double.NaN;

            try
            {
                m_AxisX.Sections.Clear();
                m_AxisY.Sections.Clear();
            }
            catch (NullReferenceException) { }
        }

        private void ChartMouseMoving(object sender, MouseEventArgs e)
        {
            var chart = (CartesianChart)sender;
            var mouseCoordinate = e.GetPosition(chart);
            var p = chart.ConvertToChartValues(mouseCoordinate);
            SectionX.Value = p.X;
            SectionY.Value = p.Y;
        }

        public ObservableCollection<Section> sections = new ObservableCollection<Section>();

        private void SelectELF_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                //MainWindow.s_StatusBar.SetInfoTxt("读取ELF文件：" + openFileDialog.FileName);

                // Open the file to read from.
                using (StreamReader sr = File.OpenText(openFileDialog.FileName))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.Length == 0 || s[0] != '0')
                            continue;
                        try
                        {
                            string[] s_list = Regex.Split(s, @"\s{2,}");
                            var type = SectionTypeParse(s_list[3]);
                            if (type == SectionType.UNKNOWN)
                                continue;
                            sections.Add(new Section()
                            {
                                Addr = Convert.ToUInt32(s_list[0], 16),
                                Size = Convert.ToUInt16(s_list[1], 16),
                                Name = s_list[2],
                                Type = type,
                                Read = false
                            });
                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine(exp.Message);
                            continue;
                        }
                    }
                }
            }
        }

        private SectionType SectionTypeParse(string s)
        {
            if (s.StartsWith("volatile "))
            {
                s.Remove(0, "volatile ".Length);
            }
            switch (s)
            {
                case "uint8_t":
                    return SectionType.UINT8;
                case "uint16_t":
                    return SectionType.UINT16;
                case "uint32_t":
                    return SectionType.UINT32;
                case "int8_t":
                    return SectionType.INT8;
                case "int16_t":
                    return SectionType.INT16;
                case "int32_t":
                    return SectionType.INT32;
                case "unsigned char":
                    return SectionType.UINT8;
                case "short unsigned int":
                    return SectionType.UINT16;
                case "unsigned int":
                    return SectionType.UINT32;
                case "char":
                    return SectionType.INT8;
                case "short int":
                    return SectionType.INT16;
                case "int":
                    return SectionType.INT32;
                case "float":
                    return SectionType.Float;
                case "double":
                    return SectionType.Double;
                default:
                    return SectionType.UNKNOWN;
            }
        }

    }
}
