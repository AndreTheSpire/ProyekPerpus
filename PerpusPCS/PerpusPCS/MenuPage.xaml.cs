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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }


        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            MainPage mp = new MainPage();
            mp.Show();
            this.Close();
        }

        private void btnMasterBuku_Click(object sender, RoutedEventArgs e)
        {
            MasterBukuPage mbp = new MasterBukuPage();
            mbp.ShowDialog();
        }

        private void btnMasterUsers_Click(object sender, RoutedEventArgs e)
        {
            MasterUser mu = new MasterUser();
            mu.ShowDialog();
        }

        private void btnTransaksiPembelian_Click(object sender, RoutedEventArgs e)
        {
            TransaksiPembelian tp = new TransaksiPembelian();
            tp.ShowDialog();
        }

        private void BtnKategoriBuku_click(object sender, RoutedEventArgs e)
        {
            MasterKategoriBuku KB = new MasterKategoriBuku();
            KB.ShowDialog();
        }

        private void btnPengembalianBuku_Click(object sender, RoutedEventArgs e)
        {
            TransaksiPengembalianBuku tpb = new TransaksiPengembalianBuku();
            tpb.ShowDialog();
        }

        private void pinjamdanbeli_Click(object sender, RoutedEventArgs e)
        {
            HalReportPeminjamanPengembalian report = new HalReportPeminjamanPengembalian();
            report.ShowDialog();
        }
    }
}
