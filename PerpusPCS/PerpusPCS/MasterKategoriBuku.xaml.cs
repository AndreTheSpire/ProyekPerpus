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

using System.Data.OracleClient;
using System.Data;
namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for MasterKategoriBuku.xaml
    /// </summary>
    public partial class MasterKategoriBuku : Window
    {
        DataTable ds1;
        DataTable ds2;
        OracleDataAdapter da;
        OracleConnection conn;
        int IDbuku = 0;
        string judulbuku = "";
        public MasterKategoriBuku()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
            loadDataKat();
            isicb();
            txtID.IsEnabled = false;
            txtJudul.IsEnabled = false;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }
        private void loadData()
        {
            dgvBuku.SelectedIndex = -1;
            ds1 = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            cmd.CommandText = "select * from buku";


            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds1);
            dgvBuku.ItemsSource = ds1.DefaultView;
            conn.Close();
        }
        private void isicb()
        {
            cbGenre.Items.Add(new ComboBoxItem { Name = "Educational", Content = "Educational" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Comedy", Content = "Comedy" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Fiction", Content = "Fiction" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Drama", Content = "Drama" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Biography", Content = "Biography" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "History", Content = "History" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Romance", Content = "Romance" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Family", Content = "Family" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Relationship", Content = "Relationship" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Economics", Content = "Economics" });
            cbGenre.Items.Add(new ComboBoxItem { Name = "Spy", Content = "Spy" });

        }
        private void loadDataKat()
        {
            dgvKatBuku.SelectedIndex = -1;
            ds2 = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            cmd.CommandText = "select KB.ID,B.Judul as " + '"' + "Judul Buku" + '"' + ",KB.genre from Kategori_Buku KB,Buku B where KB.id_buku=B.ID";


            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds2);
            dgvKatBuku.ItemsSource = ds2.DefaultView;
            conn.Close();
        }
        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void resetTampilan()
        {
            txtID.Text = "-";
            txtJudul.Text = "";
            cbGenre.SelectedIndex = -1;
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnInsertbook.IsEnabled = true;
            loadData();
            loadDataKat();

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
                cmd.CommandText = "select max(id)+1 from kategori_buku";
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
            
            int idx = dgvBuku.SelectedIndex;
            if (idx != -1)
            {
                //mengatur judul
                IDbuku = Convert.ToInt32(ds1.Rows[idx][0]);
                judulbuku = ds1.Rows[idx][1].ToString();
            }
               
        }
        private void btnInsertBook_Click(object sender, RoutedEventArgs e)
        {
            autogenID();
            txtJudul.Text = judulbuku;
        }
        private void editupdate()
        {
            int idx = dgvKatBuku.SelectedIndex;
            if (idx != -1)
            {
                //mengatur judul
                txtID.Text = ds2.Rows[idx][0].ToString();
                txtJudul.Text = ds2.Rows[idx][1].ToString();
                string genre = ds2.Rows[idx][2].ToString();
                if (genre == "Educational")
                {
                    cbGenre.SelectedIndex = 0;
                }
                else if (genre == "Comedy")
                {
                    cbGenre.SelectedIndex = 1;
                }
                else if (genre == "Fiction")
                {
                    cbGenre.SelectedIndex = 2;
                }
                else if (genre == "Drama")
                {
                    cbGenre.SelectedIndex = 3;
                }
                else if (genre == "Biography")
                {
                    cbGenre.SelectedIndex = 4;
                }
                else if (genre == "History")
                {
                    cbGenre.SelectedIndex = 5;
                }
                else if (genre == "Romance")
                {
                    cbGenre.SelectedIndex = 6;
                }
                else if (genre == "Family")
                {
                    cbGenre.SelectedIndex = 7;
                }
                else if (genre == "Relationship")
                {
                    cbGenre.SelectedIndex = 8;
                }
                else if (genre == "Economics")
                {
                    cbGenre.SelectedIndex = 9;
                }
                else if (genre == "Spy")
                {
                    cbGenre.SelectedIndex = 10;
                }

                btnInsert.IsEnabled = false;
                btnInsertbook.IsEnabled = false;
                btnUpdate.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnClear.IsEnabled = true;
            }
           
        }

        private void dgvKatBuku_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            editupdate();
        }

        private bool cekValid()
        {
            bool valid = true;
            if (txtID.Text == "-") valid = false;
            if (txtJudul.Text.Length <= 0) valid = false;
            if (cbGenre.SelectedIndex == -1) valid = false;
            return valid;
        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);
                    ComboBoxItem selectedPembayaran = (ComboBoxItem)cbGenre.SelectedItem;
                    string genre = selectedPembayaran.Name.ToString();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"insert into kategori_buku values({id},'{IDbuku}','{genre}')";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("insert berhasil");
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("insert gagal"); ;
                }
            }
            else
            {
                MessageBox.Show("tidak boleh ada data yang kosong");
            }
            resetTampilan();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);
                    ComboBoxItem selectedPembayaran = (ComboBoxItem)cbGenre.SelectedItem;
                    string genre = selectedPembayaran.Name.ToString();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"Update kategori_buku set genre='{genre}' where id={id}";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("update berhasil");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("update gagal"); ;
                }
            }
            else
            {
                MessageBox.Show("tidak boleh ada data yang kosong");
            }
            resetTampilan();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cekValid())
            {
                try
                {
                    int id = Convert.ToInt32(txtID.Text);
                    ComboBoxItem selectedPembayaran = (ComboBoxItem)cbGenre.SelectedItem;
                    string genre = selectedPembayaran.Name.ToString();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"delete from kategori_buku where id={id}";
                    conn.Close();
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    MessageBox.Show("delete berhasil");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("delete gagal"); ;
                }
            }
            else
            {
                MessageBox.Show("tidak boleh ada data yang kosong");
            }
            resetTampilan();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            resetTampilan();
        }

       
    }
}
