using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            LoadData(null, null);
        }
        public void LoadData(string username, string nama)
        {
            dgvPengembalianBuku.SelectedIndex = -1;
            ds = new DataTable();
            OracleDataAdapter da = new OracleDataAdapter();

            string str_kode = "";
            if (username!=null) str_kode += $"and upper(username) like upper('%{username}%')";
            if (nama != null) str_kode += $"and upper(nama) like upper('%{nama}%')";

            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"select hp.id,u.username as Username, u.nama as Nama, to_char(hp.tanggal_peminjaman,'DD/MM/YYYY'), u.id from users u left join h_peminjaman hp on hp.id_user = u.id where 1 = 1 {str_kode} and hp.id not in (select id_h_peminjaman from pengembalian)";

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
            dgvPengembalianBuku_Loaded(sender, e);
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
            dgvPengembalianBuku_Loaded(sender, e);
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

        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnKembalikan_Click(object sender, RoutedEventArgs e)
        {
            if (dgvPengembalianBuku.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Salah Satu Item pada Datagrid Pengembalian Buku !");
                return;
            }
            if (MessageBox.Show("Sudah Periksa kembali Buku yang akan dikembalikan ?", "Kembalikan Buku", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                conn.Close();
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int idx = dgvPengembalianBuku.SelectedIndex;
                        int user_id = Convert.ToInt32(ds.Rows[idx][4]);
                       
                        DateTime tanggal_pinjam = Convert.ToDateTime(DateTime.ParseExact(ds.Rows[idx][3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        //periksa user premium atau tidak
                        OracleCommand cmd = new OracleCommand()
                        {
                            CommandType = CommandType.StoredProcedure,
                            Connection = conn,
                            CommandText = "cekValidPremiumKembalikan"
                        };
                        cmd.Parameters.Add(new OracleParameter()
                        {
                            Direction = ParameterDirection.ReturnValue,
                            ParameterName = "returnval",
                            OracleDbType = OracleDbType.Int32,
                            Size = 20
                        });
                        cmd.Parameters.Add(new OracleParameter()
                        {
                            Direction = ParameterDirection.Input,
                            ParameterName = "p_id",
                            OracleDbType = OracleDbType.Int32,
                            Size = 20,
                            Value = user_id
                        });
                        cmd.Parameters.Add(new OracleParameter()
                        {
                            Direction = ParameterDirection.Input,
                            ParameterName = "tanggal_pinjam",
                            OracleDbType = OracleDbType.Date,
                            Size = 20,
                            Value = tanggal_pinjam
                        });
                        
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show(cmd.Parameters["returnval"].Value.ToString());
                        int isValid = Convert.ToInt32(cmd.Parameters["returnval"].Value.ToString());
                        int Total_Denda = 0;
                        //hitung denda
                        //MessageBox.Show(waktuBerlalu.Days.ToString());
                        if (isValid == 0 )//false // 7 hari rule
                        {
                            Total_Denda = hitungDenda(7);

                        }
                        else //true //14 hari rule
                        {
                            Total_Denda = hitungDenda(14);
                        }
                        //MessageBox.Show(Total_Denda.ToString());
                        //masukkan data
                        int id_h_peminjaman = Convert.ToInt32(ds.Rows[idx][0]);
                        cmd = new OracleCommand();
                        cmd.Connection = conn;
                        //insert into pengembalian values(id, id_h_peminjaman, tanggal_pengembalian, jumlah_denda)
                        cmd.CommandText = $"insert into pengembalian values(0,{id_h_peminjaman},to_date('01/01/0001','DD/MM/YYYY'),{Total_Denda})";
                        if (MessageBox.Show($"Sudah Membayar Denda Sebesar Rp.{Total_Denda},00 ?", "Pembayaran", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Pengembalian Buku Berhasil!");
                        }
                        else
                        {
                            MessageBox.Show("Pengembalian Buku Dibatalkan!");
                        }
                        trans.Commit();
                        dgvPengembalianBuku.SelectedIndex = -1;
                        dgvDetailPeminjaman.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
                conn.Close();
                LoadData(null, null);
                dgvPengembalianBuku_Loaded(sender, e);
            }
            
        }
        private int hitungDenda(int waktu)
        {
            int idx = dgvPengembalianBuku.SelectedIndex;
            DateTime tanggal_pinjam = Convert.ToDateTime(DateTime.ParseExact(ds.Rows[idx][3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture));
            TimeSpan waktuBerlalu = tanggal_pinjam - DateTime.Now;
            int Total_Denda = 0;
            bool lewatduabelashari = false;
            int waktuMinusTujuh = waktuBerlalu.Days + waktu;
            //MessageBox.Show(waktuMinusTujuh.ToString() + " waktu berlalu : " + waktuBerlalu.Days.ToString());
            if (waktuMinusTujuh >= 0)
            {
                Total_Denda = 0;
            }
            else
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select * from denda order by 1 desc";
                OracleDataReader reader = cmd.ExecuteReader();
                int ctr = 0;
                int hari_sisa = waktuMinusTujuh;
                while (reader.Read())
                {
                    int waktuDenda = Convert.ToInt32(reader.GetValue(1));
                    if (waktuMinusTujuh <= waktuDenda * -1)
                    {

                        if (!lewatduabelashari)
                        {
                            ctr++;
                            lewatduabelashari = true;
                            hari_sisa -= waktuDenda * -1;
                            //MessageBox.Show(hari_sisa.ToString());
                            Total_Denda = Convert.ToInt32(reader.GetValue(2));
                        }
                    }
                }
                if (lewatduabelashari)
                {
                    Total_Denda = Total_Denda + ((hari_sisa * 1000) * -1);
                }
                //MessageBox.Show("waktuminus tujuh : " + waktuMinusTujuh + ", hari_sisa : " + hari_sisa.ToString() + "  " + Total_Denda);
            }
            return Total_Denda;
        }

        private void dgvPengembalianBuku_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPengembalianBuku.Columns[0].Width = DataGridLength.Auto;
            dgvPengembalianBuku.Columns[1].Width = DataGridLength.Auto;
            dgvPengembalianBuku.Columns[1].Header = "Username";
            dgvPengembalianBuku.Columns[2].Header = "Nama";
            dgvPengembalianBuku.Columns[3].Header = "Tanggal Pinjam";
            dgvPengembalianBuku.Columns[4].Visibility = Visibility.Hidden;
        }
    }
}
