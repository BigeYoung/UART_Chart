using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
