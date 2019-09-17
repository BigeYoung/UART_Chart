using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UsartChart
{
    /// <summary>
    /// UART.xaml 的交互逻辑
    /// </summary>
    public partial class UART : UserControl, INotifyPropertyChanged
    {
        public UART()
        {
            InitializeComponent();
            DataContext = this;
            ScanPort();
        }

        public bool IsOpen
        {
            get
            {
                return serial_port.IsOpen;
            }
            set
            {
                if (value == serial_port.IsOpen)
                    return;
                try
                {
                    if (value)
                    {
                        serial_port.PortName = PortName.Text;
                        serial_port.Open();
                    }
                    else
                        serial_port.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "串口" + (value ? "打开" : "关闭") + "失败");
                }
                OnPropertyChanged();
            }
        }

        public int BaudRate
        {
            get { return serial_port.BaudRate; }
            set { serial_port.BaudRate = value; OnPropertyChanged(); }
        }

        public SerialPort serial_port = new SerialPort(" ", 115200);

        public List<string> available_ports { get; set; } = new List<string>();

        async void ScanPort()
        {
            while (true)
            {
                var _available_ports = SerialPort.GetPortNames();
                if (!available_ports.SequenceEqual(_available_ports))
                {
                    foreach (var item in _available_ports)
                    {
                        if (!available_ports.Contains(item))
                            available_ports.Add(item);
                    }
                    foreach (var item in available_ports)
                    {
                        if (!_available_ports.Contains(item))
                            available_ports.Remove(item);
                    }
                }
                OnPropertyChanged("available_ports");
                OnPropertyChanged("IsOpen");
                await Task.Delay(200);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
