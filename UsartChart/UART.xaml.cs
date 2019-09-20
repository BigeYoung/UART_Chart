using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private const byte RM_BOARD_1 = 0x01;
        private const byte RM_BOARD_2 = 0x02;
        private const byte RM_BOARD_3 = 0x03;

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(serial_port.ReadLine());
        }

        private void SerialPort_Operate_Flash(byte board, byte act, byte size, uint addr, ulong dataBuf = 0)
        {
            byte[] reAddr = BitConverter.GetBytes(addr);
            Array.Reverse(reAddr);
            byte[] reDataBuf = BitConverter.GetBytes(dataBuf);
            Array.Reverse(reDataBuf);
            byte[] data = new byte[16];
            data[0] = board;
            data[1] = act;
            data[2] = size;
            Array.Copy(reAddr, 0, data, 3, 4);
            Array.Copy(reDataBuf, 0, data, 7, 8);
            serial_port.WriteLine(System.Text.Encoding.UTF8.GetString(data));
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

        public ObservableCollection<Section> sections;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<Tuple<uint, double>> RecieveValue;
        private void OnRecieveValue(uint address, double value)
        {
            RecieveValue?.Invoke(this, new Tuple<uint, double>(address, value));
        }
    }
}
