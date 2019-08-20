﻿using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UsartChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
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
        }

        public void Dispose()
        {
            var vm = (SpeedTestVm)DataContext;
            vm.Values.Dispose();
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
    }
}
