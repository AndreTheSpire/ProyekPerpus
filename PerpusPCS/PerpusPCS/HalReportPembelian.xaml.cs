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
    /// Interaction logic for HalReportPembelian.xaml
    /// </summary>
    public partial class HalReportPembelian : Window
    {
        int iduser = 0;
        int user_id = -1;
        string user_username = "";
        string user_password = "";
        string user_nama = "";
        string user_tanggal_lahir = "";
        string user_no_telp = "";
        public HalReportPembelian()
        {
            InitializeComponent();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMasuk_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btncari_Click(object sender, RoutedEventArgs e)
        {
            cari();
        }

        private void cari()
        {
            PilihUser pu = new PilihUser();
            pu.ShowDialog();
            clear();
            user_id = pu.id;
            user_username = pu.username;
            user_password = pu.password;
            user_nama = pu.nama;
            user_tanggal_lahir = pu.tanggal_lahir;
            user_no_telp = pu.no_telp;
            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
            iduser = user_id;
        }
        private void clear()
        {

            user_id = -1;
            user_username = "";
            user_password = "";
            user_nama = "";
            user_tanggal_lahir = "";
            user_no_telp = "";

            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
        }
    }
}
