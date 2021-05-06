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
            OracleCommand cmdFunction = new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                Connection = conn,
                CommandText = "autogen_pembelian_premium"
            };

            cmdFunction.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.ReturnValue,
                ParameterName = "id_pembelian_premium",
                OracleType = OracleType.VarChar,
                Size = 100
            });
            conn.Open();
            cmdFunction.ExecuteNonQuery();
            tbID.Text = cmdFunction.Parameters["id_pembelian_premium"].Value.ToString();
            conn.Close();
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
            //data namauser, status harus dijadikan kata"
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

        private void dgvPremium_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dgvPremium.SelectedIndex != -1)
            {
                try
                {
                    int index = dgvPremium.SelectedIndex;
                    tbID.Text = ds.Rows[index][0].ToString();
                    tbNamaUser.Text = ds.Rows[index][1].ToString();
                    //belum selected index (premium, metode)
                    string tempStatus = ds.Rows[index][3].ToString();
                    if (tempStatus == "0")
                    {
                        rbPending.IsChecked = true;
                        rbAccepted.IsChecked = false;
                        rbRejected.IsChecked = false;
                    }
                    else if (tempStatus == "1")
                    {
                        rbPending.IsChecked = false;
                        rbAccepted.IsChecked = true;
                        rbRejected.IsChecked = false;
                    }
                    else if (tempStatus == "2")
                    {
                        rbPending.IsChecked = false;
                        rbAccepted.IsChecked = false;
                        rbRejected.IsChecked = true;
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
            }
            
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            loadData();
            tbNamaUser.Text = "";
            cbPremium.SelectedIndex = -1;
            cbMetodePembayaran.SelectedItem = -1;
            rbAccepted.IsChecked = false;
            rbPending.IsChecked = false;
            rbRejected.IsChecked = false;
        }
    }
}
