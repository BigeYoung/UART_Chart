using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            serial_port.DataReceived += SerialPort_DataReceived;
            ScanPort();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(serial_port.ReadExisting());

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

        static public SerialPort serial_port = new SerialPort(" ", 115200)
        {
            Encoding = Encoding.UTF8
        };

        public List<string> AvailablePorts { get; set; } = new List<string>();

        async void ScanPort()
        {
            while (true)
            {
                var available_ports = SerialPort.GetPortNames();
                if (!AvailablePorts.SequenceEqual(available_ports))
                {
                    foreach (var item in available_ports)
                    {
                        if (!AvailablePorts.Contains(item))
                            AvailablePorts.Add(item);
                    }
                    foreach (var item in AvailablePorts)
                    {
                        if (!available_ports.Contains(item))
                            AvailablePorts.Remove(item);
                    }
                    OnPropertyChanged("AvailablePorts");
                }
                OnPropertyChanged("IsOpen");
                await Task.Delay(200);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
