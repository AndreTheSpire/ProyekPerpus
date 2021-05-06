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
            cmd.CommandText = "select PP.ID as " + '"' + "No." + '"' + ",U.nama,P.jenis," +
                "case PP.status when 0 then 'Pending' when 1 then 'Accepted' when 2 then 'Rejected' end as " + '"' + "Status" + '"' + ",PP.metode_pembayaran as " + '"' + "Metode" + '"' + ",PP.created_at as " + '"' + "Tanggal Buat" + '"' + " from pembelian_premium PP, users U, premium P where PP.id_user = U.ID and PP.id_premium = p.ID order by 1 asc";
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
                    //selected index premium
                    if(ds.Rows[index][2].ToString()== "NewComer")
                    {
                        cbPremium.SelectedIndex = 0;
                    }
                    else if (ds.Rows[index][2].ToString() == "Regular")
                    {
                        cbPremium.SelectedIndex = 1;
                    }
                    else if (ds.Rows[index][2].ToString() == "Double")
                    {
                        cbPremium.SelectedIndex = 2;
                    }
                    else if (ds.Rows[index][2].ToString() == "Triple")
                    {
                        cbPremium.SelectedIndex = 3;
                    }
                    else if (ds.Rows[index][2].ToString() == "Semester")
                    {
                        cbPremium.SelectedIndex = 4;
                    }
                    else if (ds.Rows[index][2].ToString() == "Yearly")
                    {
                        cbPremium.SelectedIndex = 5;
                    }
                    else if (ds.Rows[index][2].ToString() == "TriYear")
                    {
                        cbPremium.SelectedIndex = 6;
                    }
                    else if (ds.Rows[index][2].ToString() == "Permanent")
                    {
                        cbPremium.SelectedIndex = 7;
                    }

                    //selected index metode pembayaran
                    if (ds.Rows[index][4].ToString() == "BCA")
                    {
                        cbMetodePembayaran.SelectedIndex = 0;
                    }
                    else if (ds.Rows[index][4].ToString() == "OVO")
                    {
                        cbMetodePembayaran.SelectedIndex = 1;
                    }
                    else if (ds.Rows[index][4].ToString() == "DANA")
                    {
                        cbMetodePembayaran.SelectedIndex = 2;
                    }
                    else if (ds.Rows[index][4].ToString() == "Gopay")
                    {
                        cbMetodePembayaran.SelectedIndex = 3;
                    }
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
            cbMetodePembayaran.SelectedIndex = -1;
            rbAccepted.IsChecked = false;
            rbPending.IsChecked = false;
            rbRejected.IsChecked = false;
        }
    }
}
