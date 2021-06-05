using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
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
using System.Windows.Shapes;

namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for PilihBuku.xaml
    /// </summary>
    public partial class PilihBuku : Window
    {
        DataTable ds;
        OracleDataAdapter da;
        OracleConnection conn;

        public int ID = -1;
        public string judul = "";
        public string author = "";
        public string penerbit = "";
        public int halaman = -1;
        public string status_premium = "";
        public string bahasa = "";
        public int status_delete = -1;

        public PilihBuku()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            conn = ConnectionPage.conn;
            loadData(null);
        }

        private void loadData(string kode)
        {
            dgvBuku.SelectedIndex = -1;
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            if (kode == null)
            {
                cmd.CommandText = $"select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa,status_delete from buku where status_delete = 0 order by 1";
            }
            else
            {
                cmd.CommandText = kode + " order by 1";
            }



            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvBuku.ItemsSource = ds.DefaultView;
            conn.Close();
        }

        private void btnCari_Click(object sender, RoutedEventArgs e)
        {
            string kode = "select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa,status_delete from buku where status_delete = 0";
            if (txtJudul.Text.Length > 0) kode += $" and upper(judul) like upper('%{txtJudul.Text}%')";
            if (txtAuthor.Text.Length > 0) kode += $" and upper(author) like upper('%{txtAuthor.Text}%')";
            if (txtPenerbit.Text.Length > 0) kode += $" and upper(penerbit) like upper('%{txtPenerbit.Text}%')";
            if (txtBahasa.Text.Length > 0) kode += $" and upper(bahasa) like upper('%{txtBahasa.Text}%')";
            loadData(kode);
            dgvBuku_Loaded(sender, e);
        }

        private void dgvBuku_Loaded(object sender, RoutedEventArgs e)
        {
            dgvBuku.Columns[0].Width = DataGridLength.Auto;
            dgvBuku.Columns[4].Width = DataGridLength.Auto;
            dgvBuku.Columns[0].Header = "ID";
            dgvBuku.Columns[1].Header = "Judul";
            dgvBuku.Columns[2].Header = "Author";
            dgvBuku.Columns[3].Header = "Penerbit";
            dgvBuku.Columns[4].Header = "Halaman";
            dgvBuku.Columns[5].Header = "Premium";
            dgvBuku.Columns[6].Header = "Bahasa";
            dgvBuku.Columns[7].Header = "Status Delete";
            dgvBuku.Columns[7].Visibility = Visibility.Hidden;
        }

        private void dgvBuku_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvBuku.SelectedIndex != -1)
            {

                int idx = dgvBuku.SelectedIndex;
                ID = Convert.ToInt32(ds.Rows[idx][0]);
                judul = ds.Rows[idx][1].ToString();
                author = ds.Rows[idx][2].ToString();
                penerbit = ds.Rows[idx][3].ToString();
                halaman = Convert.ToInt32(ds.Rows[idx][4]);
                status_premium = ds.Rows[idx][5].ToString();
                bahasa = ds.Rows[idx][6].ToString();
                status_delete = Convert.ToInt32(ds.Rows[idx][7]);
            }
        }

        private void btnPilih_Click(object sender, RoutedEventArgs e)
        {
            if (ID == -1)
            {
                MessageBox.Show("pilihlah terlebih dahulu");
                return;
            }
            if (MessageBox.Show($"Yakin Pilih Buku : {judul}", "Pilih Buku", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Pilihan Dibatalkan!");
            }
        }
    }
}
