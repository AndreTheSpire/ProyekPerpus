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
    /// Interaction logic for HalReportPeminjamanPengembalian.xaml
    /// </summary>
    public partial class HalReportPeminjamanPengembalian : Window
    {
        OracleConnection conn;
        public HalReportPeminjamanPengembalian()
        {
            InitializeComponent();
            this.conn = ConnectionPage.conn;

        }
        public void loaduser()
        {
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = conn;
            conn.Open();
            cmd2.CommandText = "select * from users order by 1";
            OracleDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                cbuser.Items.Add(new ComboBoxItem
                {
                    Name = reader2.GetValue(1).ToString(),
                    Content = reader2.GetValue(0).ToString() + " - " + reader2.GetValue(1).ToString()
                });
            }
            cbuser.SelectedValuePath = "Name";
            cbuser.SelectedIndex = 0;
            conn.Close();
        }

        private void btncari_Click(object sender, RoutedEventArgs e)
        {
            ReportPinjamDanBeli rpdb = new ReportPinjamDanBeli();
            rpdb.SetDatabaseLogon(ConnectionPage.userId, ConnectionPage.pass, ConnectionPage.source, "");

            int idcari = cbuser.SelectedIndex;
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

            //rpdb.SetParameterValue("idusercari", idcari);
            //rpdb.SetParameterValue("usernameusercari", usernamee);
            //rpdb.SetParameterValue("namausercari", namaa);
            //rpdb.SetParameterValue("tgllahitusercari", tanggallahir);
            //rpdb.SetParameterValue("notelpusecarir", notelp);

            creport.ViewerCore.ReportSource = rpdb;
        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            creport.Owner = Window.GetWindow(this);
            loaduser();
        }

    }
}
