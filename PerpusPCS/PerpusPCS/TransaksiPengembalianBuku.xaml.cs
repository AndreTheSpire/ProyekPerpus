using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
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
    /// Interaction logic for TransaksiPengembalianBuku.xaml
    /// </summary>
    public partial class TransaksiPengembalianBuku : Window
    {
        OracleConnection conn;
        DataTable ds;
        DataTable ds2;
        public TransaksiPengembalianBuku()
        {
            InitializeComponent();
            this.conn = ConnectionPage.conn;
            LoadData(null, null);
        }
        public void LoadData(string username, string nama)
        {
            dgvPengembalianBuku.SelectedIndex = -1;
            ds = new DataTable();
            OracleDataAdapter da = new OracleDataAdapter();

            string str_kode = "";
            if (username!=null) str_kode = $"and username like '%{username}%'";
            if (nama != null && username == null) str_kode = $"and nama like '%{nama}%'";
            else if (nama != null && username != null) str_kode = str_kode + " and " + $"nama like '%{nama}%'";

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"select hp.id,u.username as Username, u.nama as Nama, hp.tanggal_peminjaman from users u left join h_peminjaman hp on hp.id_user = u.id where 1 = 1 {str_kode} and hp.id not in (select id_h_peminjaman from pengembalian)";

            conn.Close();
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvPengembalianBuku.ItemsSource = ds.DefaultView;
            conn.Close();
        }

        private void btnResetPencarian_Click(object sender, RoutedEventArgs e)
        {
            LoadData(null, null);
        }

        private void btnCari_Click(object sender, RoutedEventArgs e)
        {
            string nama = txtNama.Text;
            int lenNama = nama.Replace(" ", "").Length;
            if (lenNama == 0) nama = null;
            string username = txtUsername.Text;
            int lenUsername = username.Replace(" ", "").Length;
            if (lenUsername == 0) username = null;

            LoadData(username, nama);
        }

        private void btnTampilkanBuku_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPengembalianBuku.SelectedIndex != -1)
            {
                int idx = dgvPengembalianBuku.SelectedIndex;
                int id_h_peminjaman = Convert.ToInt32(ds.Rows[idx][0]);
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"select dp.id as {'"' + "ID Detail" + '"'},dp.id_h_peminjaman as {'"'+"ID Peminjaman"+'"'}, b.judul as {'"' + "Judul" + '"'}, b.author as {'"' + "Author" + '"'} from d_peminjaman dp left join buku b on b.id = dp.id_buku where id_h_peminjaman = {id_h_peminjaman}";

                dgvDetailPeminjaman.SelectedIndex = -1;
                ds2 = new DataTable();

                conn.Close();
                conn.Open();
                cmd.ExecuteReader();
                
                OracleDataAdapter da = new OracleDataAdapter();
                da.SelectCommand = cmd;

                da.Fill(ds2);
                dgvDetailPeminjaman.ItemsSource = ds2.DefaultView;

                conn.Close();
            }
            else
            {
                MessageBox.Show("Pilih Dahulu kumpulan buku yang mau dikembalikan");
            }
        }
    }
}
