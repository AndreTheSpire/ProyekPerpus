using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
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
    /// Interaction logic for MasterBukuPage.xaml
    /// </summary>
    public partial class MasterBukuPage : Window
    {
        DataTable ds;
        OracleDataAdapter da;
        OracleConnection conn;
        public MasterBukuPage()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void loadData()
        {
            dgvBuku.SelectedIndex = -1;
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            cmd.CommandText = "select * from buku";


            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvBuku.ItemsSource = ds.DefaultView;
            conn.Close();
        }

        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtHalaman_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtHalaman.Text;
            try
            {
                int numberpassed = Convert.ToInt32(text);
            }
            catch (Exception)
            {
                int len = text.Length - 1;
                if (len < 0) len = 0;
                txtHalaman.Text = text.Substring(0, len);
            }
        }

        private void isiEditor()
        {
            int idx = dgvBuku.SelectedIndex;
            //mengatur judul
            txtID.Text = ds.Rows[idx][0].ToString();
            txtJudul.Text = ds.Rows[idx][1].ToString();
            txtAuthor.Text = ds.Rows[idx][2].ToString();
            txtPenerbit.Text = ds.Rows[idx][3].ToString();
            txtHalaman.Text = ds.Rows[idx][4].ToString();
            if (Convert.ToInt32(ds.Rows[idx][5]) == 0)
            {
                rbFree.IsChecked = true;
                rbPremium.IsChecked = false;
            }
            else
            {
                rbFree.IsChecked = false;
                rbPremium.IsChecked = true;
            }
            txtBahasa.Text = ds.Rows[idx][6].ToString();
        }
        private void resetTampilan()
        {
            txtID.Text = "-";
            txtJudul.Text = "";
            txtAuthor.Text = "";
            txtPenerbit.Text = "";
            txtHalaman.Text = "";
            rbFree.IsChecked = true;
            rbPremium.IsChecked = false;
            txtBahasa.Text = "";
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            loadData();
        }

        private void autogenID()
        {
            if (txtID.Text != "-")
            {
                return;
            }
            //mengatur ID
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                conn.Close();
                conn.Open();
                cmd.CommandText = "select max(id)+1 from buku";
                txtID.Text = Convert.ToString(cmd.ExecuteScalar());
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvBuku_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvBuku.SelectedIndex != -1)
            {
                isiEditor();
                btnInsert.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
        }

        private void txtJudul_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtJudul.Text.Length > 0)
            {
                autogenID();
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            resetTampilan();
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);
                    string judul = txtJudul.Text;
                    string author = txtAuthor.Text;
                    string penerbit = txtPenerbit.Text;
                    int halaman = Convert.ToInt32(txtHalaman.Text);
                    int status = 0;
                    if ((bool)rbPremium.IsChecked) status = 1;
                    string bahasa = txtBahasa.Text;
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"insert into buku values({id},'{judul}','{author}','{penerbit}',{halaman},{status},'{bahasa}')";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    resetTampilan();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private bool cekValid()
        {
            bool valid = true;
            if (txtID.Text == "-") valid = false;
            if (txtJudul.Text.Length <= 0) valid = false;
            if (txtAuthor.Text.Length <= 0) valid = false;
            if (txtPenerbit.Text.Length <= 0) valid = false;
            if (txtHalaman.Text.Length <= 0) valid = false;
            if (txtBahasa.Text.Length <= 0) valid = false;
            return valid;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);
                    string judul = txtJudul.Text;
                    string author = txtAuthor.Text;
                    string penerbit = txtPenerbit.Text;
                    int halaman = Convert.ToInt32(txtHalaman.Text);
                    int status = 0;
                    if ((bool)rbPremium.IsChecked) status = 1;
                    string bahasa = txtBahasa.Text;
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"update buku set judul = '{judul}', author = '{author}', penerbit = '{penerbit}', halaman = {halaman} , status_premium = {status}, bahasa = '{bahasa}' where id = {id} ";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    resetTampilan();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);

                    //melakukan delete untuk kategori buku
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    conn.Close();
                    conn.Open();
                    cmd.CommandText = $"delete from kategori_buku where id_buku = {id}";
                    cmd.ExecuteNonQuery();

                    //melakukan delete d_peminjaman
                    cmd.CommandText = $"delete from d_peminjaman where id_buku = {id}";
                    cmd.ExecuteNonQuery();

                    //melakukan delete buku
                    cmd.CommandText = $"delete from buku where id = {id}";
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    resetTampilan();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
        }
    }
}
