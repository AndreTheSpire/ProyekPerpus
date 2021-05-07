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
    /// Interaction logic for MasterUser.xaml
    /// </summary>
    public partial class MasterUser : Window
    {
        DataTable ds;
        OracleDataAdapter da;
        OracleConnection conn;
        public MasterUser()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
        }
        private void loadData()
        {
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();

            cmd.Connection = conn;
            cmd.CommandText = "select id, username, password, nama, to_char(tanggal_lahir, 'dd/MM/yyyy'), no_telp from users";


            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvUser.ItemsSource = ds.DefaultView;
            conn.Close();
        }
        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgvUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvUser.SelectedIndex != -1)
            {
                tbID.Text = ds.Rows[dgvUser.SelectedIndex][0].ToString();
                tbUsername.Text = ds.Rows[dgvUser.SelectedIndex][1].ToString();
                tbPassword.Text = ds.Rows[dgvUser.SelectedIndex][2].ToString();
                tbNama.Text = ds.Rows[dgvUser.SelectedIndex][3].ToString();
                DateTime tanggallahir = DateTime.ParseExact(ds.Rows[dgvUser.SelectedIndex][4].ToString().Replace('/', '/'), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                DPTglLahir.SelectedDate = Convert.ToDateTime(tanggallahir);
                tbNoTelp.Text = ds.Rows[dgvUser.SelectedIndex][5].ToString();
            }
        }
    }
}
