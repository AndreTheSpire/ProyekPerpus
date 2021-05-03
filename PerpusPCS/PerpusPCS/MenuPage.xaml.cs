using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Window
    {
        public MenuPage()
        {
            InitializeComponent();
        }


        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainPage mp = new MainPage();
            mp.Show();
            this.Close();
        }
    }
}
