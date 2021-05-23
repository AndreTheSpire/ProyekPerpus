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
        int user_id = -1;
        int userlama = -1;
        String user_username = "";
        String user_nama = "";
        String user_tanggal_lahir = "";
        String user_no_telp = "";
        bool pilihuser = false;
        private void btnPilihUser_Click(object sender, RoutedEventArgs e)
        {
            if (pilihuser) 
            {
                userlama = user_id;
            }
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
            if (userlama == user_id)
            {
                pilihuser = true;
                loadDataBuku(null);
            }
            else
            {
                pilihuser = false;
                loadDataBuku(null);
            }
            pilihuser = true;
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
            if (!pilihuser)
            {
                dt2 = new DataTable();
                cmd.CommandText = $"select id, judul, author, penerbit, halaman, bahasa from buku where 1 = 2";
                conn.Open();
                cmd.ExecuteReader();
                da.SelectCommand = cmd;
                da.Fill(dt2);
                dgvPilih.ItemsSource = dt2.DefaultView;
                conn.Close();
            }
        }

        private void btnPilih_Click(object sender, RoutedEventArgs e)
        {
            DataRow dr = dt2.NewRow();
            try
            {
                conn.Close();
                conn.Open();
                //pengecekan user udah pernah dipilih atau belum
                if (user_id == -1)
                {
                    MessageBox.Show("Belum Memilih User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

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
                int cekValid = Convert.ToInt32(cmd.Parameters["returnval"].Value.ToString());
                conn.Close();
                String statusbuku = dt.Rows[dgvBuku.SelectedIndex][5].ToString();
                int banyakbukupilihan = dgvPilih.Items.Count;
                int[] daftarpilihan = new int[banyakbukupilihan];
                for (int i = 0; i < dgvPilih.Items.Count; i++)
                {
                    daftarpilihan[i] = Convert.ToInt32(dt2.Rows[i][0]);
                }
                bool sudahada = false;
                for (int i = 0; i < daftarpilihan.Length; i++)
                {
                    if (Convert.ToInt32(dt.Rows[dgvBuku.SelectedIndex][0]) == daftarpilihan[i])
                    {
                        sudahada = true;
                    }
                }
                if (user_id != -1 && dgvBuku.SelectedIndex != -1)
                {
                    if ((cekValid == 0 && statusbuku.Equals("Free")) || (cekValid == 1 && (statusbuku.Equals("Free") || statusbuku.Equals("Premium"))))
                    {
                        if (!sudahada)
                        {
                            dr["ID"] = dt.Rows[dgvBuku.SelectedIndex][0];
                            dr["Judul"] = dt.Rows[dgvBuku.SelectedIndex][1];
                            dr["Author"] = dt.Rows[dgvBuku.SelectedIndex][2];
                            dr["Penerbit"] = dt.Rows[dgvBuku.SelectedIndex][3];
                            dr["Halaman"] = dt.Rows[dgvBuku.SelectedIndex][4];
                            dr["Bahasa"] = dt.Rows[dgvBuku.SelectedIndex][6];
                            dt2.Rows.Add(dr);
                        }
                        else
                        {
                            MessageBox.Show("Buku Sudah Dipilih");
                        }
                    }
                    else
                    {
                        MessageBox.Show("User Tidak Memiliki Premium Tidak Bisa Pinjam Premium");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            dgvBuku.SelectedIndex = -1;
        }
        private void dgvPilih_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPilih.Columns[0].Width = DataGridLength.SizeToCells;
            dgvPilih.Columns[4].Width = DataGridLength.Auto;
            dgvPilih.Columns[0].Header = "ID";
            dgvPilih.Columns[1].Header = "Judul";
            dgvPilih.Columns[2].Header = "Author";
            dgvPilih.Columns[3].Header = "Penerbit";
            dgvPilih.Columns[4].Header = "Halaman";
            dgvPilih.Columns[5].Header = "Bahasa";
        }
    }
}
