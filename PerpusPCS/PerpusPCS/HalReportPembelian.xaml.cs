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
using Oracle.DataAccess.Client;

namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for HalReportPembelian.xaml
    /// </summary>
    public partial class HalReportPembelian : Window
    {
        int iduser = -1;
        int user_id = -1;
        string user_username = "";
        string user_password = "";
        string user_nama = "";
        string user_tanggal_lahir = "";
        string user_no_telp = "";
        OracleConnection conn;
        public HalReportPembelian()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadComboBoxStatus();
            loadComboBoxMetodePembayaran();
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMasuk_Click(object sender, RoutedEventArgs e)
        {
            string status = "";
            int id = -1;
            string metode_pembayaran = "Semua";
            pembelian pembelian = new pembelian();
            pembelian.SetDatabaseLogon(ConnectionPage.userId, ConnectionPage.pass, ConnectionPage.source, "");
            //if (user_id == -1)
            //{
            //    MessageBox.Show("pilihlah user terlebih dahulu");
            //    return;
            //}

            if (tglawalinp.SelectedDate != null && tglawalinp.SelectedDate < tglakhirinp.SelectedDate)
            {
                DateTime tanggalAwal = (DateTime)tglawalinp.SelectedDate;
                pembelian.SetParameterValue("tanggalawalinp", tanggalAwal.ToString("dd-MMM-yyyy"));
            }
            else
            {
                MessageBox.Show("Tanggal tidak boleh kosong atau tanggal awal tidak boleh melebihi tanggal akhir");
                return;
            }

            if (tglakhirinp.SelectedDate != null && tglawalinp.SelectedDate < tglakhirinp.SelectedDate)
            {
                DateTime tanggalAkhir = (DateTime)tglakhirinp.SelectedDate;
                pembelian.SetParameterValue("tanggalakhirinp", tanggalAkhir.ToString("dd-MMM-yyyy"));
            }
            else
            {
                MessageBox.Show("Tanggal tidak boleh kosong atau tanggal awal tidak boleh melebihi tanggal akhir");
                return;
            }
            if (cbStatus.SelectedIndex == 0)
            {
                status = "Pending";
                id = 0;
            }
            else if (cbStatus.SelectedIndex == 1)
            {
                status = "Accepted";
                id = 1;
            }
            else if (cbStatus.SelectedIndex == 2)
            {
               status = "Rejected";
                id = 2;
            }

            if (cbMetode.SelectedIndex == 0)
            {
                metode_pembayaran = "BCA";
            }
            else if (cbMetode.SelectedIndex == 1)
            {
                metode_pembayaran = "OVO";
            }
            else if (cbMetode.SelectedIndex == 2)
            {
                metode_pembayaran = "DANA";
            }
            else if (cbMetode.SelectedIndex == 3)
            {
                metode_pembayaran = "Gopay";
            }
            pembelian.SetParameterValue("idusercari", iduser);
            pembelian.SetParameterValue("status",status);
            pembelian.SetParameterValue("id_status",id);
            pembelian.SetParameterValue("metode_pembayaran", metode_pembayaran);
            crvReport.ViewerCore.ReportSource = pembelian;
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

        private void loadComboBoxStatus()
        {
            cbStatus.Items.Add(new ComboBoxItem { Name = "Pending", Content = "Pending" });
            cbStatus.Items.Add(new ComboBoxItem { Name = "Accepted", Content = "Accepted" });
            cbStatus.Items.Add(new ComboBoxItem { Name = "Rejected", Content = "Rejected" });
            cbStatus.SelectedIndex = -1;
        }

        private void loadComboBoxMetodePembayaran()
        {
            cbMetode.Items.Add(new ComboBoxItem { Name = "BCA", Content = "BCA" });
            cbMetode.Items.Add(new ComboBoxItem { Name = "OVO", Content = "OVO" });
            cbMetode.Items.Add(new ComboBoxItem { Name = "DANA", Content = "DANA" });
            cbMetode.Items.Add(new ComboBoxItem { Name = "Gopay", Content = "Gopay" });
            cbMetode.SelectedIndex = -1;
        }

        private void clear()
        {

            user_id = -1;
            user_username = "";
            user_password = "";
            user_nama = "";
            user_tanggal_lahir = "";
            user_no_telp = "";
            cbStatus.SelectedIndex = -1;
            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
        }
    }
}
