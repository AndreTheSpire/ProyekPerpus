using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
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
        }

        private void loadData()
        {
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
    }
}
