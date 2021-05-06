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
    /// Interaction logic for TransaksiPembelian.xaml
    /// </summary>
    public partial class TransaksiPembelian : Window
    {
        DataTable ds;
        OracleDataAdapter da;
        OracleConnection conn;
        public TransaksiPembelian()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
            isiKategoriPremium();
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "BCA", Content = "BCA" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "OVO", Content = "OVO" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "DANA", Content = "DANA" });
            cbMetodePembayaran.Items.Add(new ComboBoxItem { Name = "Gopay", Content = "Gopay" });
            tbID.IsEnabled = false;
        }
        private void loadData()
        {
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = "select * from pembelian_premium";
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvPremium.ItemsSource = ds.DefaultView;
            conn.Close();
            //OracleCommand cmdFunction = new OracleCommand()
            //{
            //    CommandType = CommandType.StoredProcedure,
            //    Connection = conn,
            //    CommandText = "autogen_pembelian_premium"
            //};

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
                    Name = reader.GetValue(1).ToString(),
                    Content = reader.GetValue(1).ToString()
                }) ;  
            }
            cbPremium.SelectedValuePath = "Name";
            cbPremium.SelectedIndex = -1;
            conn.Close();
        }
    }
}
