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
    /// Interaction logic for HalReportPeminjamanPengembalian.xaml
    /// </summary>
    public partial class HalReportPeminjamanPengembalian : Window
    {
        int iduser = 0;
        DataTable dUser;
        OracleDataAdapter da;
        OracleConnection conn;
        public HalReportPeminjamanPengembalian()
        {
            InitializeComponent();
            this.conn = ConnectionPage.conn;

        }
        private void loadDataUser()
        {
            dUser = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = "select id as " + '"' + "No" + '"' + ", username as " + '"' + "Username" + '"' + "," +
                    "nama as " + '"' + "Nama" + '"' + ", to_char(tanggal_lahir, 'dd/MM/yyyy') as " + '"' + "Tanggal Lahir" + '"' + ", no_telp as " + '"' + "No Telp" + '"' + "from users where status_delete = 0";
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(dUser);
            dgvUser.ItemsSource = dUser.DefaultView;
            conn.Close();
        }

        private void btncari_Click(object sender, RoutedEventArgs e)
        {
            if (rbNamaLengkap.IsChecked == true || rbUsername.IsChecked == true)
            {
                cari();
            }
            else
            {
                MessageBox.Show("tolong pilih radio button keyword dahulu");
            }
            
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            creport.Owner = Window.GetWindow(this);
            loadDataUser();
        }

        private void cari()
        {
            dUser = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            int cekprem = 0;
            cmd.Connection = conn;
            cmd.CommandText = $"select id as " + '"' + "No" + '"' + ", username as " + '"' + "Username" + '"' + "," +
                    "nama as " + '"' + "Nama" + '"' + ", to_char(tanggal_lahir, 'dd/MM/yyyy') as " + '"' + "Tanggal Lahir" + '"' + ", no_telp as " + '"' + "No Telp" + '"' + "from users";
            string comm = " where";
            string keyword = Convert.ToString(tbKeyword.Text);
            string berdasarkan = "username";

            if (rbUsername.IsChecked == true)
            {
                berdasarkan = "username";
            }
            else if (rbNamaLengkap.IsChecked == true)
            {
                berdasarkan = "nama";
            }
            comm += $" upper({berdasarkan}) like upper('%{keyword}%')";


            comm += $" order by ID";
            cmd.CommandText += comm;
            Console.WriteLine(cmd.CommandText);
            conn.Close();
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(dUser);
            dgvUser.ItemsSource = dUser.DefaultView;
            conn.Close();
        }
        private void btnMasuk_Click(object sender, RoutedEventArgs e)
        {
            ReportPinjamDanBeli rpdb = new ReportPinjamDanBeli();
            rpdb.SetDatabaseLogon(ConnectionPage.userId, ConnectionPage.pass, ConnectionPage.source, "");

            int idcari = iduser;
            string usernamee = " ", namaa = " ", notelp = " ";
            DateTime tanggallahir = DateTime.Now;
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = conn;
            conn.Open();
            cmd2.CommandText = $"select * from users where ID={idcari}";
            OracleDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {

                usernamee = reader2.GetValue(1).ToString();
                namaa = reader2.GetValue(3).ToString();
                tanggallahir = Convert.ToDateTime(reader2.GetValue(4));
                notelp = reader2.GetValue(5).ToString();
            }
            conn.Close();

            rpdb.SetParameterValue("idusercari", idcari);
            rpdb.SetParameterValue("usernameusercari", usernamee);
            rpdb.SetParameterValue("namausercari", namaa);
            rpdb.SetParameterValue("tgllahitusercari", tanggallahir);
            rpdb.SetParameterValue("notelpusecarir", notelp);

            creport.ViewerCore.ReportSource = rpdb;
        }

        private void dgvUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idx = dgvUser.SelectedIndex;
            if (idx != -1)
            {
                iduser = Convert.ToInt32(dUser.Rows[idx][0]);
                //mengatur judul
                //IDbuku = Convert.ToInt32(dUser.Rows[idx][0]);
                //judulbuku = dUser.Rows[idx][1].ToString();
            }
        }
    }
}
