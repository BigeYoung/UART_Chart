using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
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
            ELF_ListBox.DataContext = sections;
            UART.m_SerialPort.DataReceived += SerialPort_DataRecieved;
            m_Chart.DataContext = m_SubSeries;
            Subscription.DataContext = m_SubSeries;
            m_SubSeries.AddSeries(new Section() { Addr = 0, Name = "happy", Size = 4, Type = SectionType.Double });
        }

        public SubSeries m_SubSeries = new SubSeries();

        private void SerialPort_DataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            UART.m_SerialPort.ReadExisting();
            // TODO
            // UART.解析()
            // if (sections.First(...)!=null)
        }

        static readonly AxisSection SectionX = new AxisSection()
        {
            DataLabel = true,
            StrokeThickness = 1,
            Stroke = Brushes.Orange,
            DisableAnimations = true,
            DataLabelForeground = Brushes.White
        };
        static readonly AxisSection SectionY = new AxisSection()
        {
            DataLabel = true,
            StrokeThickness = 1,
            Stroke = Brushes.Orange,
            DisableAnimations = true,
            DataLabelForeground = Brushes.White
        };

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
                            var type = Section.TypeParse(s_list[3]);
                            if (type == SectionType.UNKNOWN)
                                continue;
                            var section = new Section()
                            {
                                Addr = Convert.ToUInt32(s_list[0], 16),
                                Size = Convert.ToUInt16(s_list[1], 16),
                                Name = s_list[2],
                                Type = type
                            };
                            sections.Add(section);
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

    }
}
