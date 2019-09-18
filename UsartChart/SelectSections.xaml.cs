using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UsartChart
{
    /// <summary>
    /// SelectData.xaml 的交互逻辑
    /// </summary>
    public partial class SelectSections : UserControl
    {
        public SelectSections()
        {
            InitializeComponent();
            m_DataGrid.ItemsSource = sections;
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
                MainWindow.s_StatusBar.SetInfoTxt("读取ELF文件：" + openFileDialog.FileName);

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
