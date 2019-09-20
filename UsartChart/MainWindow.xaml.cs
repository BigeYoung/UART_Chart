using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UsartChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static StatusBar s_StatusBar;
        public MainWindow()
        {
            InitializeComponent();

            s_StatusBar = m_StatusBar;

            m_UART.RecieveValue += (_, pair) => {
                ((LineSerials)m_Chart.DataContext).AddValue(pair.Item1, pair.Item2);
            };
        }


        // var line_serials = (LineSerials)m_Chart.DataContext;

    }
}
