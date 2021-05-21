using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
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
    /// Interaction logic for TransaksiPembelian.xaml
    /// </summary>
    public partial class TransaksiPembelian : Window
    {
        DataTable dt;
        OracleDataAdapter da;
        OracleConnection conn;
        OracleCommandBuilder builder;
        int ctr = 0;
        int basectr = 0;
        public TransaksiPembelian()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
            //loadDataUser();
            isiKategoriPremium();
            isiCbMetodePembayaran();
        }
        private void isiCbMetodePembayaran()
        {
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "BCA", Content = "BCA" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "OVO", Content = "OVO" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "DANA", Content = "DANA" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "Gopay", Content = "Gopay" });
            cbMetodePembayaran.SelectedIndex = 0;
        }
        private void loadData()
        {
            dt = new DataTable();
            da = new OracleDataAdapter("select * from pembelian_premium where 1=2",conn);
            
            conn.Close();
            conn.Open();
            builder = new OracleCommandBuilder(da);
            da.Fill(dt);
            dgvPremium.ItemsSource = dt.DefaultView;
            OracleCommand cmd = new OracleCommand("select nvl(count(id),0) from pembelian_premium",conn);
            ctr = Convert.ToInt32(cmd.ExecuteScalar());
            basectr = ctr;
            conn.Close();
        }
        private void dgvPremium_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPremium.Columns[0].Width = DataGridLength.Auto;
            dgvPremium.Columns[1].Width = DataGridLength.Auto;
            dgvPremium.Columns[2].Width = DataGridLength.Auto;
            dgvPremium.Columns[3].Width = DataGridLength.Auto;
            dgvPremium.Columns[0].Header = "ID.";
            dgvPremium.Columns[1].Header = "ID User";
            dgvPremium.Columns[2].Header = "ID Premium";
            dgvPremium.Columns[3].Header = "Status Premium";
            dgvPremium.Columns[4].Header = "Metode Pembayaran";
            dgvPremium.Columns[5].Header = "Tanggal Pembayaran";

        }
        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void isiKategoriPremium()
        {
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            conn.Close();
            conn.Open();
            cmd.CommandText = "select * from premium";
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbPremium.Items.Add(new ComboBoxItem
                {
                    Name = "s"+reader.GetValue(0),
                    Content = reader.GetValue(1).ToString()
                });
            }
            cbPremium.SelectedValuePath = "Name";
            cbPremium.SelectedIndex = 0;
            conn.Close();
        }

        private void dgvPremium_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvPremium.SelectedIndex != -1)
            {
                if (MessageBox.Show("Yakin Akan Mendelete Row ini ?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        int idx = dgvPremium.SelectedIndex;
                        dt.Rows.Remove(dt.Rows[idx]);
                        ctr = basectr;
                        foreach (DataRow row in dt.Rows)
                        {
                            row[0] = ctr++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void clear()
        {
            loadData();
            cbPremium.SelectedIndex = 0;
            cbMetodePembayaran.SelectedIndex = 0;

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

      

        private bool cekKondisi()
        {
            conn.Close();
            conn.Open();
            //pengecekan user udah pernah dipilih atau belum
            if (user_id == -1)
            {
                MessageBox.Show("Belum Memilih User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
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
            if (cekValid == 1)
            {
                MessageBox.Show("User Masih Dalam Masa Aktif Premium", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void rbAccepted_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (cekKondisi())
            {
                DataRow dr = dt.NewRow();
                dr[0] = ctr;
                dr[1] = user_id;
                dr[2] = cbPremium.SelectedIndex;
                dr[3] = 0;
                ComboBoxItem cb = (ComboBoxItem)cbMetodePembayaran.SelectedItem;
                dr[4] = cb.Content;
                dr[5] = DateTime.Now;

                bool isTambah = true;
                foreach (DataRow row in dt.Rows)
                {
                    if (row[1].ToString() == dr[1].ToString())
                    {
                        isTambah = false;
                    }
                }
                if (isTambah)
                {
                    dt.Rows.Add(dr);
                    ctr++;
                }
                else
                {
                    MessageBox.Show("user ini sudah pernah didaftarkan");
                }
                
            }
        }

        int user_id = -1;
        string user_username = "";
        string user_password = "";
        string user_nama = "";
        string user_tanggal_lahir = "";
        string user_no_telp = "";

        private void btnSimpan_Click(object sender, RoutedEventArgs e)
        {
            conn.Close();
            conn.Open();
            using (OracleTransaction trans = conn.BeginTransaction())
            {
                try
                {
                    da.Update(dt);
                    trans.Commit();
                    MessageBox.Show("Berhasil Menyimpan", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    trans.Rollback();
                }
            }
            conn.Close();
            
        }

        private void btnPilihUser_Click(object sender, RoutedEventArgs e)
        {
            PilihUser pu = new PilihUser();
            pu.ShowDialog();
            //clear();
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
            dgvPremium_Loaded(sender, e);
        }

        private void btnToUpdatePembelianPremium_Click(object sender, RoutedEventArgs e)
        {
            UpdatePembelianPremium upp = new UpdatePembelianPremium();
            upp.ShowDialog();
        }
    }
}
