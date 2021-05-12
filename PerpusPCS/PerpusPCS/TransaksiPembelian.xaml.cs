using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
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
            rbAccepted.IsEnabled = false;
            rbPending.IsEnabled = false;
            rbRejected.IsEnabled = false;
            btnUpdate.IsEnabled = false;
        }
        private void loadData()
        {
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = "select PP.ID as " + '"' + "No" + '"' + ",U.username,U.nama,P.jenis," +
                "case PP.status when 0 then 'Pending' when 1 then 'Accepted' when 2 then 'Rejected' end as " + '"' + "Status" + '"' + ",PP.metode_pembayaran as " + '"' + "Metode" + '"' + ",to_char(PP.created_at, 'dd/MM/yyyy') as " + '"' + "Tanggal Buat" + '"' + " from pembelian_premium PP, users U, premium P where PP.id_user = U.ID and PP.id_premium = p.ID order by 1 asc";
            conn.Close();
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
                OracleDbType = OracleDbType.Varchar2,
                Size = 100
            });
            conn.Open();
            cmdFunction.ExecuteNonQuery();
            tbID.Text = cmdFunction.Parameters["id_pembelian_premium"].Value.ToString();
            conn.Close();
            btnInsert.IsEnabled = true;
            tbUsername.Text = "";
            tbUsername.IsEnabled = true;
            cbPremium.IsEnabled = true;
            cbMetodePembayaran.IsEnabled = true;
            cbPremium.SelectedIndex = -1;
            cbMetodePembayaran.SelectedIndex = -1;
            rbAccepted.IsChecked = false;
            rbPending.IsChecked = false;
            rbRejected.IsChecked = false;
            rbAccepted.IsEnabled = false;
            rbPending.IsEnabled = false;
            rbRejected.IsEnabled = false;
            btnUpdate.IsEnabled = false;
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
                    tbUsername.IsEnabled = false;
                    cbPremium.IsEnabled = false;
                    cbMetodePembayaran.IsEnabled = false;
                    btnInsert.IsEnabled = false;
                    rbAccepted.IsEnabled = true;
                    rbPending.IsEnabled = true;
                    rbRejected.IsEnabled = true;
                    btnUpdate.IsEnabled = true;
                    int index = dgvPremium.SelectedIndex;
                    tbID.Text = ds.Rows[index][0].ToString();
                    tbUsername.Text = ds.Rows[index][1].ToString();
                    //selected index premium
                    if(ds.Rows[index][3].ToString()== "NewComer")
                    {
                        cbPremium.SelectedIndex = 0;
                    }
                    else if (ds.Rows[index][3].ToString() == "Regular")
                    {
                        cbPremium.SelectedIndex = 1;
                    }
                    else if (ds.Rows[index][3].ToString() == "Double")
                    {
                        cbPremium.SelectedIndex = 2;
                    }
                    else if (ds.Rows[index][3].ToString() == "Triple")
                    {
                        cbPremium.SelectedIndex = 3;
                    }
                    else if (ds.Rows[index][3].ToString() == "Semester")
                    {
                        cbPremium.SelectedIndex = 4;
                    }
                    else if (ds.Rows[index][3].ToString() == "Yearly")
                    {
                        cbPremium.SelectedIndex = 5;
                    }
                    else if (ds.Rows[index][3].ToString() == "TriYear")
                    {
                        cbPremium.SelectedIndex = 6;
                    }
                    else if (ds.Rows[index][3].ToString() == "Permanent")
                    {
                        cbPremium.SelectedIndex = 7;
                    }

                    //selected index metode pembayaran
                    if (ds.Rows[index][5].ToString() == "BCA")
                    {
                        cbMetodePembayaran.SelectedIndex = 0;
                    }
                    else if (ds.Rows[index][5].ToString() == "OVO")
                    {
                        cbMetodePembayaran.SelectedIndex = 1;
                    }
                    else if (ds.Rows[index][5].ToString() == "DANA")
                    {
                        cbMetodePembayaran.SelectedIndex = 2;
                    }
                    else if (ds.Rows[index][5].ToString() == "Gopay")
                    {
                        cbMetodePembayaran.SelectedIndex = 3;
                    }
                    string tempStatus = ds.Rows[index][4].ToString();
                    if (tempStatus == "Pending")
                    {
                        rbPending.IsChecked = true;
                        rbAccepted.IsChecked = false;
                        rbRejected.IsChecked = false;
                    }
                    else if (tempStatus == "Accepted")
                    {
                        rbPending.IsChecked = false;
                        rbAccepted.IsChecked = true;
                        rbRejected.IsChecked = false;
                    }
                    else if (tempStatus == "Rejected")
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
            clear();
        }

        private void clear()
        {
            loadData();
            btnInsert.IsEnabled = true;
            tbUsername.Text = "";
            cbPremium.SelectedIndex = -1;
            cbMetodePembayaran.SelectedIndex = -1;
            rbAccepted.IsChecked = false;
            rbPending.IsChecked = false;
            rbRejected.IsChecked = false;
            rbAccepted.IsEnabled = false;
            rbPending.IsEnabled = false;
            rbRejected.IsEnabled = false;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int index = dgvPremium.SelectedIndex;
            int status = -1;
            if(ds.Rows[index][4].ToString() == "Rejected")
            {
                if(rbAccepted.IsChecked == true)
                {
                    MessageBox.Show("Status sudah rejected");
                    rbAccepted.IsChecked = false;
                    rbRejected.IsChecked = true;
                }
                else
                {
                    try
                    {
                        if (rbAccepted.IsChecked == true)
                        {
                            status = 1;
                        }
                        else if (rbPending.IsChecked == true)
                        {
                            status = 0;
                        }
                        else if (rbRejected.IsChecked == true)
                        {
                            status = 2;
                        }
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        conn.Close();
                        conn.Open();
                        cmd.CommandText = $"select id from pembelian_premium";
                        int idPembelian = Convert.ToInt32(ds.Rows[index][0]);
                        cmd.CommandText = $"update pembelian_premium set status = '{status}' where id = {idPembelian}";
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else if (ds.Rows[index][4].ToString() == "Accepted")
            {
                if (rbRejected.IsChecked == true)
                {
                    MessageBox.Show("Status sudah di accept");
                    rbAccepted.IsChecked = true;
                    rbRejected.IsChecked = false;
                }
                else
                {
                    try
                    {
                        if (rbAccepted.IsChecked == true)
                        {
                            status = 1;
                        }
                        else if (rbPending.IsChecked == true)
                        {
                            status = 0;
                        }
                        else if (rbRejected.IsChecked == true)
                        {
                            status = 2;
                        }
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        conn.Close();
                        conn.Open();
                        cmd.CommandText = $"select id from pembelian_premium";
                        int idPembelian = Convert.ToInt32(ds.Rows[index][0]);
                        cmd.CommandText = $"update pembelian_premium set status = '{status}' where id = {idPembelian}";
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                try
                {
                    if (rbAccepted.IsChecked == true)
                    {
                        status = 1;
                    }
                    else if (rbPending.IsChecked == true)
                    {
                        status = 0;
                    }
                    else if (rbRejected.IsChecked == true)
                    {
                        status = 2;
                    }
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    conn.Close();
                    conn.Open();
                    cmd.CommandText = $"select id from pembelian_premium";
                    int idPembelian = Convert.ToInt32(ds.Rows[index][0]);
                    cmd.CommandText = $"update pembelian_premium set status = '{status}' where id = {idPembelian}";
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            
            
        }

        private bool cekKondisi()
        {
            if(tbUsername.Text == "")
            {
                MessageBox.Show("Username wajib diisi");
                return false;
            }
            else if(cbPremium.SelectedIndex == -1)
            {
                MessageBox.Show("Jenis premium wajib diisi");
                return false;
            }
            else if (cbMetodePembayaran.SelectedIndex == -1)
            {
                MessageBox.Show("Metode pembayaran wajib diisi");
                return false;
            }
            else if(tbID.Text == "")
            {
                MessageBox.Show("ID wajib diisi");
                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void rbAccepted_Checked(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (cekKondisi())
            {
                conn.Close();
                conn.Open();
                //MessageBox.Show("cek kondisi jalan");
                using (OracleTransaction transaksi = conn.BeginTransaction())
                {
                    //MessageBox.Show("sebelom try catch");
                    try
                    {
                        string username = tbUsername.Text;
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transaksi;
                        cmd.CommandText = $"select id from users where username = '{username}'";
                        int id_user = Convert.ToInt32(cmd.ExecuteScalar());
                        int id_nota = Convert.ToInt32(tbID.Text);
                        ComboBoxItem selectedMetodePembayaran = (ComboBoxItem)cbMetodePembayaran.SelectedItem;
                        string pembayaran = selectedMetodePembayaran.Name.ToString();
                        ComboBoxItem selectedPremium = (ComboBoxItem)cbPremium.SelectedItem;
                        string premium = selectedPremium.Name.ToString();
                        cmd.CommandText = $"select id from premium where jenis = '{premium}'";
                        int id_premium = Convert.ToInt32(cmd.ExecuteScalar());
                        int status = 0;
                        //MessageBox.Show("test sebelom cmd");
                        cmd.CommandText = $"insert into pembelian_premium(id,id_user,id_premium,status,metode_pembayaran,created_at) values ({id_nota},{id_user},{id_premium},{status},'{pembayaran}',sysdate)";
                        //MessageBox.Show("id nota : " + id_nota);
                        //MessageBox.Show("id user : " + id_user);
                        //MessageBox.Show("id premium : " + id_premium);
                        //MessageBox.Show("status : " + status);
                        //MessageBox.Show("pembayaran : " + pembayaran);
                        //MessageBox.Show("test sesudah cmd");
                        cmd.ExecuteNonQuery();
                        //MessageBox.Show("test sesudah execute query");
                        transaksi.Commit();
                        conn.Close();
                        //MessageBox.Show("transaksi berhasil");
                        clear();

                    }
                    catch(Exception ex)
                    {
                        transaksi.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            
        }
    }
}
