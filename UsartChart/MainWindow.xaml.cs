using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

            for (uint i = 0; i < 32; i++)
            {
                m_SubSeries.AddSeries(new Section() { Addr = i, Name = "Happy" + i.ToString(), Type = SectionType.Double, Size = 10 });
            }
        }

        public SubSeries m_SubSeries = new SubSeries();

        private void SerialPort_DataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] serialData = System.Text.Encoding.UTF8.GetBytes(UART.m_SerialPort.ReadExisting());
            Dictionary<Section, double> data_dict = new Dictionary<Section, double>();

            for (int i = 0; (i + 1) * 16 <= serialData.Length; i++)
            {
                if (serialData[1 + i * 16] == 2)
                {
                    //TODO 解析...
                    uint addr = BitConverter.ToUInt32(serialData, 3 + i * 16);
                    var section = sections.First((x) => x.Addr == addr);
                    //TODO 根据 section.Type 解析
                    double value = 0;
                    switch (section.Type)
                    {
                        case SectionType.INT8:
                            //TODO value = BitConverter.???(serialData, 8 + i * 16);
                            break;
                        case SectionType.INT16:
                            value = BitConverter.ToInt16(serialData, 8 + i * 16);
                            break;
                        case SectionType.INT32:
                            value = BitConverter.ToInt32(serialData, 8 + i * 16);
                            break;
                        case SectionType.UINT8:
                            //TODO value = BitConverter.???(serialData, 8 + i * 16);
                            break;
                        case SectionType.UINT16:
                            value = BitConverter.ToUInt16(serialData, 8 + i * 16);
                            break;
                        case SectionType.UINT32:
                            value = BitConverter.ToUInt32(serialData, 8 + i * 16);
                            break;
                        case SectionType.Float:
                            value = BitConverter.ToSingle(serialData, 8 + i * 16);
                            break;
                        case SectionType.Double:
                            value = BitConverter.ToDouble(serialData, 8 + i * 16);
                            break;
                        default:
                            break;
                    }
                    data_dict.Add(section, value);
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                m_SubSeries.Update(data_dict);
            });
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
                                Size = Convert.ToByte(s_list[1], 16),
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

        private void PortName_DropDownOpened(object sender, EventArgs e)
        {
            ((UART)((ComboBox)sender).DataContext).RefreshAvailblePort();
        }
    }
}
