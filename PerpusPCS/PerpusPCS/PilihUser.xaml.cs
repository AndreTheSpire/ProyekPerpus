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

using Oracle.DataAccess.Client;
using System.Data;
namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for PilihUser.xaml
    /// </summary>
    public partial class PilihUser : Window
    {
        DataTable ds;
        OracleDataAdapter da;
        OracleConnection conn;

        public int id = -1;
        public string username = "";
        public string password = "";
        public string nama = "";
        public string tanggal_lahir = "";
        public string no_telp = "";

        public PilihUser()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            conn = ConnectionPage.conn;
        }

        private void dgvUser_Loaded(object sender, RoutedEventArgs e)
        {
            dgvUser.Columns[0].Width = DataGridLength.Auto;
            dgvUser.Columns[4].Width = DataGridLength.Auto;
            dgvUser.Columns[0].Header = "ID";
            dgvUser.Columns[1].Header = "Username";
            dgvUser.Columns[2].Header = "Password";
            dgvUser.Columns[3].Header = "Nama Lengkap";
            dgvUser.Columns[4].Header = "Tanggal Lahir";
            dgvUser.Columns[5].Header = "No Telp";
            dgvUser.Columns[6].Header = "Status Delete";
            dgvUser.Columns[2].Visibility = Visibility.Hidden;
            dgvUser.Columns[6].Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadDataGrid(null);
        }
        private void loadDataGrid(string kode)
        {
            dgvUser.SelectedIndex = -1;

            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            if (kode == null)
            {
                cmd.CommandText = $"select ID,username,password,nama,to_char(tanggal_lahir,'DD/MM/YYYY'),no_telp,status_delete from users where status_delete = 0 order by 1";
            }
            else
            {
                cmd.CommandText = kode + " order by 1";
            }
            conn.Close();
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvUser.ItemsSource = ds.DefaultView;
            conn.Close();
        }

        private void btnPilih_Click(object sender, RoutedEventArgs e)
        {
            if (id == -1)
            {
                MessageBox.Show("pilihlah terlebih dahulu");
                return;
            }
            if (MessageBox.Show($"Yakin Pilih User : {username}", "Pilih User", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        private void dgvUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvUser.SelectedIndex != -1)
            {
                int idx = dgvUser.SelectedIndex;
                id = Convert.ToInt32(ds.Rows[idx][0]);
                username = Convert.ToString(ds.Rows[idx][1]);
                password = Convert.ToString(ds.Rows[idx][2]);
                nama = Convert.ToString(ds.Rows[idx][3]);
                tanggal_lahir = Convert.ToString(ds.Rows[idx][4]);
                no_telp = Convert.ToString(ds.Rows[idx][5]);
            }
        }

        private void btnCari_Click(object sender, RoutedEventArgs e)
        {
            string kode = $"select ID,username,password,nama,to_char(tanggal_lahir,'DD/MM/YYYY'),no_telp,status_delete from users where status_delete = 0";
            if (txtNama.Text.Length > 0) kode += $" and upper(nama) like upper('%{txtNama.Text}%')";
            if (txtUsername.Text.Length > 0) kode += $" and upper(username) like upper('%{txtUsername.Text}%')";
            loadDataGrid(kode);
            dgvUser_Loaded(sender, e);
        }
    }
}
