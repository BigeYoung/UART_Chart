using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
                MainWindow.s_StatusBar.SetInfoTxt("读取ELF文件："+openFileDialog.FileName);

                // Open the file to read from.
                using (StreamReader sr = File.OpenText(openFileDialog.FileName))
                {
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
        }
    }
}
