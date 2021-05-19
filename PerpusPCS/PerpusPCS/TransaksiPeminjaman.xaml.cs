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
    /// Interaction logic for TransaksiPeminjaman.xaml
    /// </summary>
    public partial class TransaksiPeminjaman : Window
    {
        OracleConnection conn;
        DataTable dt, dt2;
        OracleDataAdapter da;
        public TransaksiPeminjaman()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadDataUser(null);
            loadDataBuku(null);
        }
        private void loadDataUser(String kode)
        {
            dt = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            if (kode == null)
            {
                cmd.CommandText = "select id, username, password, nama, to_char(tanggal_lahir, 'dd/MM/yyyy'), no_telp from users where status_delete = 0";
            }
            else
            {
                cmd.CommandText = kode;
            }
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(dt);
            dgvUser.ItemsSource = dt.DefaultView;
            conn.Close();
        }

        private void btnCariUser_Click(object sender, RoutedEventArgs e)
        {
            String kode = "select id, username, password, nama, to_char(tanggal_lahir, 'dd/MM/yyyy'), no_telp from users where status_delete = 0";
            if (tbNama.Text.Length > 0)
            {
                kode += $" and upper(nama) like upper('%{tbNama.Text}%')";
            }
            loadDataUser(kode);
            dgvUser_Loaded(sender, e);
            tbNama.Text = "";
        }

        private void dgvBuku_Loaded(object sender, RoutedEventArgs e)
        {
            dgvBuku.Columns[0].Width = DataGridLength.SizeToCells;
            dgvBuku.Columns[4].Width = DataGridLength.Auto;
            dgvBuku.Columns[0].Header = "ID";
            dgvBuku.Columns[1].Header = "Judul";
            dgvBuku.Columns[2].Header = "Author";
            dgvBuku.Columns[3].Header = "Penerbit";
            dgvBuku.Columns[4].Header = "Halaman";
            dgvBuku.Columns[5].Header = "Premium";
            dgvBuku.Columns[6].Header = "Bahasa";
        }

        private void btnResetPencarianUser_Click(object sender, RoutedEventArgs e)
        {
            loadDataUser(null);
            dgvUser_Loaded(sender, e);
        }

        private void btnResetPencarianBuku_Click(object sender, RoutedEventArgs e)
        {
            loadDataBuku(null);
            dgvBuku_Loaded(sender, e);
        }

        private void btnCariBuku_Click(object sender, RoutedEventArgs e)
        {
            String kode = $"select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa from buku where status_delete = 0";
            if (tbJudul.Text.Length > 0)
            {
                kode += $" and upper(judul) like upper('%{tbJudul.Text}%')";
            }
            loadDataBuku(kode);
            dgvBuku_Loaded(sender, e);
            tbJudul.Text = "";
        }

        private void dgvUser_Loaded(object sender, RoutedEventArgs e)
        {
            dgvUser.Columns[0].Width = DataGridLength.SizeToCells;
            dgvUser.Columns[0].Header = "ID";
            dgvUser.Columns[1].Header = "Username";
            dgvUser.Columns[2].Header = "Password";
            dgvUser.Columns[3].Header = "Nama";
            dgvUser.Columns[4].Header = "Tanggal Lahir";
            dgvUser.Columns[5].Header = "No Telp";
        }

        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPinjam_Click(object sender, RoutedEventArgs e)
        {
            if (dgvUser.SelectedIndex == -1 || dgvBuku.SelectedIndex == -1)
            {
                MessageBox.Show("Harus Pilih Item pada Masing-Masing DataGrid");
                return;
            }
            if (MessageBox.Show("Yakin Pinjam Buku?", "Pinjam Buku", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                conn.Close();
                conn.Open();
                using(OracleTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int idx = dgvUser.SelectedIndex;
                        int user_id = Convert.ToInt32(dt.Rows[idx][0]);
                        //periksa user premium atau tidak
                        OracleCommand cmd = new OracleCommand()
                        {
                            CommandType = CommandType.StoredProcedure,
                            Connection = conn,
                            CommandText = "cekValidPremium"
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
                        cmd.ExecuteNonQuery();
                        int isValid = Convert.ToInt32(cmd.Parameters["returnval"].Value.ToString());
                        cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select count(*) from buku where status_delete = 0";
                        int data = Convert.ToInt32(cmd.ExecuteScalar());
                        bool[] pilihdata = new bool[data];
                        String[] statusbuku = new String[data];
                        int[] row_buku = new int[data];
                        for (int i = 0; i < dgvBuku.Items.Count; i++)
                        {
                            for (int j = 0; j < dgvBuku.Columns.Count; j++)
                            {                     
                                    }
                        }
                        
                        cmd.CommandText = $"select max(id) from h_peminjaman";
                        int idhpeminjaman = Convert.ToInt32(cmd.ExecuteScalar());
                        idhpeminjaman++;
                        if ((isValid == 0 && statusbuku.Equals("Free")) || (isValid == 1 && (statusbuku.Equals("Premium") || statusbuku.Equals("Free"))))
                        {
                            cmd.CommandText = $"insert into h_peminjaman values({idhpeminjaman}, {user_id}, sysdate)";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = $"select max(id) from h_peminjaman";
                            int idhpeminjamanlagi = Convert.ToInt32(cmd.ExecuteScalar());
                            cmd.CommandText = $"select max(id) from d_peminjaman";
                            int iddpeminjaman = Convert.ToInt32(cmd.ExecuteScalar());
                            iddpeminjaman++;
                            cmd.CommandText = $"insert into d_peminjaman values({iddpeminjaman}, {idhpeminjamanlagi}, {row_buku})";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            MessageBox.Show("User Tidak Memiliki Status Premium (Tidak Pinjam Buku Premium)");
                        }
                        trans.Commit();
                        dgvBuku.SelectedIndex = -1;
                        dgvUser.SelectedIndex = -1;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            conn.Close();
            dgvUser.SelectedIndex = -1;
            dgvBuku.SelectedIndex = -1;
        }

        private void loadDataBuku(String kode)
        {
            dt2 = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            if (kode == null)
            {
                cmd.CommandText = $"select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa from buku where status_delete = 0";
            }
            else
            {
                cmd.CommandText = kode;
            }
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(dt2);
            dt2.Columns.Add("Pilih", typeof(bool));
            cmd.CommandText = "select count(*) from buku where status_delete = 0";
            int data = Convert.ToInt32(cmd.ExecuteScalar());
            for (int i = 0; i < data; i++)
            {
                dt2.Rows[i]["Pilih"] = false;
            }
            dgvBuku.ItemsSource = dt2.DefaultView;
            conn.Close();
        }
    }
}
