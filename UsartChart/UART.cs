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
using System.Windows.Data;

namespace UsartChart
{
    /// <summary>
    /// UART.xaml 的交互逻辑
    /// </summary>
    public partial class UART : INotifyPropertyChanged
    {
        public UART()
        {
            ScanPort();
        }

        private enum RM_BOARD : byte
        {
            RM_BOARD_1 = 0x01,
            RM_BOARD_2 = 0x02,
            RM_BOARD_3 = 0x03
        };

        //private void SerialPort_Operate_Flash(byte board, byte act, byte size, uint addr, ulong dataBuf = 0)
        //{
        //    byte[] reAddr = BitConverter.GetBytes(addr);
        //    Array.Reverse(reAddr);
        //    byte[] reDataBuf = BitConverter.GetBytes(dataBuf);
        //    Array.Reverse(reDataBuf);
        //    byte[] data = new byte[16];
        //    data[0] = board;
        //    data[1] = act;
        //    data[2] = size;
        //    Array.Copy(reAddr, 0, data, 3, 4);
        //    Array.Copy(reDataBuf, 0, data, 7, 8);
        //    m_SerialPort.WriteLine(System.Text.Encoding.UTF8.GetString(data));
        //}

        public static void Subscript(uint address, byte size)
        {
            //TODO 订阅对应地址
            byte[] reAddr = BitConverter.GetBytes(address);
            byte[] data = new byte[16];
            data[0] = 1;
            data[1] = 1;
            data[2] = size;
            Array.Copy(reAddr, 0, data, 3, 4);
            m_SerialPort.Write(data, 0, 16);
            MessageBox.Show("0x" + address.ToString("X") + " has been subscript.");
        }
        public static void Unsubscript(uint address)
        {
            //TODO 取消订阅对应地址
            byte[] reAddr = BitConverter.GetBytes(address);
            byte[] data = new byte[16];
            data[0] = 1;
            data[1] = 5;
            data[2] = 0;
            Array.Copy(reAddr, 0, data, 3, 4);
            m_SerialPort.Write(data, 0, 16);
            MessageBox.Show("0x" + address.ToString("X") + " has been unsubscript.");
        }

        public string PortName
        {
            get
            {
                return m_SerialPort.PortName;
            }
            set
            {

                m_SerialPort.PortName = value == "" ? " " : value;
            }
        }

        public bool IsOpen
        {
            get
            {
                return m_SerialPort.IsOpen;
            }
            set
            {
                if (value == m_SerialPort.IsOpen)
                    return;
                try
                {
                    if (value)
                        m_SerialPort.Open();
                    else
                        m_SerialPort.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "串口" + (value ? "打开" : "关闭") + "失败");
                    RefreshAvailblePort();
                }
                OnPropertyChanged("BaudRate");
                OnPropertyChanged();
            }
        }

        public int BaudRate
        {
            get { return m_SerialPort.BaudRate; }
            set { m_SerialPort.BaudRate = value; OnPropertyChanged(); }
        }

        static public SerialPort m_SerialPort = new SerialPort(" ", 115200)
        {
            Encoding = Encoding.UTF8
        };

        public List<string> AvailablePorts { get; set; } = new List<string>();

        async void ScanPort()
        {
            while (true)
            {
                OnPropertyChanged("IsOpen");
                await Task.Delay(200);
            }
        }

        public void RefreshAvailblePort()
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
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class Not : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
