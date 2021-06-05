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
            loadData(null);
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
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
                cmd.CommandText = $"select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa from buku where status_delete = 0 order by 1";
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
        }
        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtHalaman_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int numberpassed = Convert.ToInt32(txtHalaman.Text);
            }
            catch
            {
                int panjang = txtHalaman.Text.Length;
                if (panjang >= 1)
                {
                    txtHalaman.Text = txtHalaman.Text.Substring(0, panjang - 1);
                    txtHalaman.Focus();
                    txtHalaman.SelectionStart = panjang;
                }
            }
        }

        private void isiEditor()
        {
            try
            {
                int idx = dgvBuku.SelectedIndex;
                //mengatur judul
                txtID.Text = ds.Rows[idx][0].ToString();
                txtJudul.Text = ds.Rows[idx][1].ToString();
                txtAuthor.Text = ds.Rows[idx][2].ToString();
                txtPenerbit.Text = ds.Rows[idx][3].ToString();
                txtHalaman.Text = ds.Rows[idx][4].ToString();
                if (ds.Rows[idx][5].ToString().ToLower() == "free")
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
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
            loadData(null);
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
            dgvBuku_Loaded(sender, e);
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
                    cmd.CommandText = $"insert into buku values({id},'{judul}','{author}','{penerbit}',{halaman},{status},'{bahasa}',0)";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    resetTampilan(); 
                    dgvBuku_Loaded(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Periksa Kembali Data Yang Akan Dimasukkan","Wrong Input",MessageBoxButton.OK,MessageBoxImage.Error);
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
                    dgvBuku_Loaded(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Periksa Kembali Data Yang Akan Dimasukkan","Wrong Input",MessageBoxButton.OK,MessageBoxImage.Error);
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

                    //melakukan delete buku
                    cmd.CommandText = $"update buku set status_delete = 1 where id = {id}";
                    cmd.ExecuteNonQuery();

                    conn.Close();
                    resetTampilan();
                    dgvBuku_Loaded(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            else
            {
                MessageBox.Show("Periksa Kembali Data Yang Akan Dimasukkan", "Wrong Input", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string kode = "select id, judul, author, penerbit, halaman, case status_premium when 0 then 'Free' when 1 then 'Premium' end, bahasa from buku where status_delete = 0";
            if (txtFilterJudul.Text.Length > 0) kode += $" and upper(judul) like upper('%{txtFilterJudul.Text}%')";
            if (txtFilterAuthor.Text.Length > 0) kode += $" and upper(author) like upper('%{txtFilterAuthor.Text}%')";
            if (txtFilterPenerbit.Text.Length > 0) kode += $" and upper(penerbit) like upper('%{txtFilterPenerbit.Text}%')";
            if (txtFilterBahasa.Text.Length > 0) kode += $" and upper(bahasa) like upper('%{txtFilterBahasa.Text}%')";
            loadData(kode);
            dgvBuku_Loaded(sender, e);
        }
    }
}
