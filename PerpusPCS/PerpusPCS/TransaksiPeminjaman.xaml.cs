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
            loadDataBuku(null);
        
        }
        bool pernah = false;
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

        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //private void btnPinjam_Click(object sender, RoutedEventArgs e)
        //{
        //    if (dgvUser.SelectedIndex == -1 || dgvBuku.SelectedIndex == -1)
        //    {
        //        MessageBox.Show("Harus Pilih Item pada Masing-Masing DataGrid");
        //        return;
        //    }
        //    if (MessageBox.Show("Yakin Pinjam Buku?", "Pinjam Buku", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        //    {
        //        conn.Close();
        //        conn.Open();
        //        using(OracleTransaction trans = conn.BeginTransaction())
        //        {
        //            try
        //            {
        //                int idx = dgvUser.SelectedIndex;
        //                int user_id = Convert.ToInt32(dt.Rows[idx][0]);
        //                //periksa user premium atau tidak
        //                OracleCommand cmd = new OracleCommand()
        //                {
        //                    CommandType = CommandType.StoredProcedure,
        //                    Connection = conn,
        //                    CommandText = "cekValidPremium"
        //                };
        //                cmd.Parameters.Add(new OracleParameter()
        //                {
        //                    Direction = ParameterDirection.ReturnValue,
        //                    ParameterName = "returnval",
        //                    OracleDbType = OracleDbType.Int32,
        //                    Size = 20
        //                });
        //                cmd.Parameters.Add(new OracleParameter()
        //                {
        //                    Direction = ParameterDirection.Input,
        //                    ParameterName = "p_id",
        //                    OracleDbType = OracleDbType.Int32,
        //                    Size = 20,
        //                    Value = user_id
        //                });
        //                cmd.ExecuteNonQuery();
        //                int isValid = Convert.ToInt32(cmd.Parameters["returnval"].Value.ToString());
        //                cmd = new OracleCommand();
        //                cmd.Connection = conn;
        //                cmd.CommandText = "select count(*) from buku where status_delete = 0";
        //                int data = Convert.ToInt32(cmd.ExecuteScalar());
        //                bool[] pilihdata = new bool[data];
        //                String[] statusbuku = new String[data];
        //                int[] row_buku = new int[data];
        //                for (int i = 0; i < dgvBuku.Items.Count; i++)
        //                {
        //                    for (int j = 0; j < dgvBuku.Columns.Count; j++)
        //                    {                     
        //                            }
        //                }
                        
        //                cmd.CommandText = $"select max(id) from h_peminjaman";
        //                int idhpeminjaman = Convert.ToInt32(cmd.ExecuteScalar());
        //                idhpeminjaman++;
        //                if ((isValid == 0 && statusbuku.Equals("Free")) || (isValid == 1 && (statusbuku.Equals("Premium") || statusbuku.Equals("Free"))))
        //                {
        //                    cmd.CommandText = $"insert into h_peminjaman values({idhpeminjaman}, {user_id}, sysdate)";
        //                    cmd.ExecuteNonQuery();
        //                    cmd.CommandText = $"select max(id) from h_peminjaman";
        //                    int idhpeminjamanlagi = Convert.ToInt32(cmd.ExecuteScalar());
        //                    cmd.CommandText = $"select max(id) from d_peminjaman";
        //                    int iddpeminjaman = Convert.ToInt32(cmd.ExecuteScalar());
        //                    iddpeminjaman++;
        //                    cmd.CommandText = $"insert into d_peminjaman values({iddpeminjaman}, {idhpeminjamanlagi}, {row_buku})";
        //                    cmd.ExecuteNonQuery();
        //                }
        //                else
        //                {
        //                    MessageBox.Show("User Tidak Memiliki Status Premium (Tidak Pinjam Buku Premium)");
        //                }
        //                trans.Commit();
        //                dgvBuku.SelectedIndex = -1;\
        //            }
        //            catch (Exception ex)
        //            {
        //                trans.Rollback();
        //                MessageBox.Show(ex.Message);
        //            }
        //        }
        //    }
        //    conn.Close();\
        //    dgvBuku.SelectedIndex = -1;
        //}
        int user_id = -1;
        String user_username = "";
        String user_nama = "";
        String user_tanggal_lahir = "";
        String user_no_telp = "";
        private void btnPilihUser_Click(object sender, RoutedEventArgs e)
        {
            PilihUser pu = new PilihUser();
            pu.ShowDialog();
            user_id = pu.id;
            user_username = pu.username;
            user_nama = pu.nama;
            user_tanggal_lahir = pu.tanggal_lahir;
            user_no_telp = pu.no_telp;
            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
            dgvBuku_Loaded(sender, e);
            dgvPilih_Loaded(sender, e);
        }

        private void loadDataBuku(String kode)
        {
            dt = new DataTable();
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
            da.Fill(dt);
            dgvBuku.ItemsSource = dt.DefaultView;
            conn.Close();
            if (!pernah)
            {
                dt2 = new DataTable();
                cmd.CommandText = $"select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa from buku where 1 = 2";
                conn.Open();
                cmd.ExecuteReader();
                da.SelectCommand = cmd;
                da.Fill(dt2);
                dgvPilih.ItemsSource = dt2.DefaultView;
                conn.Close();
            }
            pernah = true;
        }

        private void btnPilih_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dt2.NewRow();
            dr["ID"] = dt.Rows[dgvBuku.SelectedIndex][0];
            dr["Judul"] = dt.Rows[dgvBuku.SelectedIndex][1];
            dr["Author"] = dt.Rows[dgvBuku.SelectedIndex][2];
            dr["Penerbit"] = dt.Rows[dgvBuku.SelectedIndex][3];
            dr["Halaman"] = dt.Rows[dgvBuku.SelectedIndex][4];
            //if (dt.Rows[dgvBuku.SelectedIndex][5].Equals("Free"))
            //{
            //    dr["Premium"] = "0";
            //}
            //else
            //{
            //    dr["Premium"] = "1";
            //}
            dr["Bahasa"] = dt.Rows[dgvBuku.SelectedIndex][6];
            dt2.Rows.Add(dr);
        }

        private void dgvPilih_Loaded(object sender, RoutedEventArgs e)
        {
            if (dgvPilih.Items.Count > 0)
            {
                dgvPilih.Columns[0].Width = DataGridLength.SizeToCells;
                dgvPilih.Columns[4].Width = DataGridLength.Auto;
                dgvPilih.Columns[0].Header = "ID";
                dgvPilih.Columns[1].Header = "Judul";
                dgvPilih.Columns[2].Header = "Author";
                dgvPilih.Columns[3].Header = "Penerbit";
                dgvPilih.Columns[4].Header = "Halaman";
                dgvPilih.Columns[5].Header = "Premium";
                dgvPilih.Columns[6].Header = "Bahasa";
            }
        }
    }
}
