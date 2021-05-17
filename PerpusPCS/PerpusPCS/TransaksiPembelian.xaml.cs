﻿using System;
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
        DataTable ds, dUser;
        OracleDataAdapter da;
        OracleConnection conn;
        string username = "";
        public TransaksiPembelian()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
            //loadDataUser();
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

        //private void loadDataUser()
        //{
        //    dUser = new DataTable();
        //    OracleCommand cmd = new OracleCommand();
        //    da = new OracleDataAdapter();
        //    cmd.Connection = conn;
        //    cmd.CommandText = "select id as " + '"' + "No" + '"' + ", username as " + '"' + "Username" + '"' + "," +
        //            "nama as " + '"' + "Nama" + '"' + ", to_char(tanggal_lahir, 'dd/MM/yyyy') as " + '"' + "Tanggal Lahir" + '"' + ", no_telp as " + '"' + "No Telp" + '"' + "from users where status_delete = 0";
        //    conn.Open();
        //    cmd.ExecuteReader();
        //    da.SelectCommand = cmd;
        //    da.Fill(dUser);
        //    dgvUser.ItemsSource = dUser.DefaultView;
        //    conn.Close();
        //}

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
                });
            }
            cbPremium.SelectedValuePath = "Name";
            cbPremium.SelectedIndex = -1;
            conn.Close();
        }

        private void dgvPremium_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgvPremium.SelectedIndex != -1)
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
                    if (ds.Rows[index][3].ToString() == "NewComer")
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

                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"Select ID,username,password,nama,to_char(tanggal_lahir,'DD/MM/YYYY'),no_telp,status_delete from users where username = '{tbUsername.Text}'";
                    conn.Close();
                    conn.Open();
                    OracleDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lblUsername.Text = reader.GetString(1);
                        lblNama.Text = reader.GetString(3);
                        lblTanggalLahir.Text = reader.GetString(4);
                        lblNoTelp.Text = reader.GetString(5);
                    }
                    conn.Close();
                }
                catch (Exception ex)
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

             user_id = -1;
             user_username = "";
             user_password = "";
             user_nama = "";
             user_tanggal_lahir = "";
             user_no_telp = "";

            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
            tbUsername.Text = user_username;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int index = dgvPremium.SelectedIndex;
            int status = -1;
            if (ds.Rows[index][4].ToString() == "Pending")
            {
                if (rbPending.IsChecked == true)
                {
                    MessageBox.Show("Status sudah pending");
                    rbPending.IsChecked = true;
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
                if (rbAccepted.IsChecked == true || rbPending.IsChecked == true || rbRejected.IsChecked == true)
                {
                    MessageBox.Show("Status sudah di accept");
                    rbAccepted.IsChecked = true;
                    rbPending.IsChecked = false;
                    rbRejected.IsChecked = false;
                }
            }
            else if (ds.Rows[index][4].ToString() == "Rejected")
            {
                if (rbAccepted.IsChecked == true || rbPending.IsChecked == true || rbRejected.IsChecked == true)
                {
                    MessageBox.Show("Status sudah di Reject");
                    rbAccepted.IsChecked = false;
                    rbPending.IsChecked = false;
                    rbRejected.IsChecked = true;
                }
            }

        }

        private bool cekKondisi()
        {
            int index = dgvPremium.SelectedIndex;
            string nama = tbUsername.Text;
            OracleCommand cmd1 = new OracleCommand();
            cmd1.Connection = conn;
            conn.Close();
            conn.Open();
            cmd1.CommandText = $"select id from users where username = '{nama}'";
            int id_user = Convert.ToInt32(cmd1.ExecuteScalar());
            OracleCommand cmd = new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                Connection = conn,
                CommandText = "cekValidPremium"
            };
            cmd.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.ReturnValue,
                ParameterName = "returnval",
                OracleDbType = OracleDbType.Int32,
                Size = 20
            });
            cmd.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.Input,
                ParameterName = "p_id",
                OracleDbType = OracleDbType.Int32,
                Size = 20,
                Value = id_user
            });
            cmd.ExecuteNonQuery();
            int cekValid = Convert.ToInt32(cmd.Parameters["returnval"].Value.ToString());
            //MessageBox.Show(cekValid.ToString());
            if (cekValid == 1)
            {
                return false;
            }
            else
            {
                return true;
            }

            if (tbUsername.Text == "")
            {
                MessageBox.Show("Username wajib diisi");
                return false;
            }
            else if (cbPremium.SelectedIndex == -1)
            {
                MessageBox.Show("Jenis premium wajib diisi");
                return false;
            }
            else if (cbMetodePembayaran.SelectedIndex == -1)
            {
                MessageBox.Show("Metode pembayaran wajib diisi");
                return false;
            }
            else if (tbID.Text == "")
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
                    catch (Exception ex)
                    {
                        transaksi.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("User sudah memiliki premium yang aktif");
            }

        }

        //private void dgvUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    int idx = dgvUser.SelectedIndex;
        //    if (idx != -1)
        //    {
        //        username = dUser.Rows[idx][1].ToString();
        //        //mengatur judul
        //        //IDbuku = Convert.ToInt32(dUser.Rows[idx][0]);
        //        //judulbuku = dUser.Rows[idx][1].ToString();
        //    }
        //}

        private void btnMasuk_Click(object sender, RoutedEventArgs e)
        {
            tbUsername.Text = username;
            tbUsername.IsEnabled = false;
        }

        private void btnCari_Click(object sender, RoutedEventArgs e)
        {
            //cari();
            //tbKeyword.Text = "";
            //rbUsername.IsChecked = false;
            //rbNamaLengkap.IsChecked = false;
        }
        int user_id = -1;
        string user_username = "";
        string user_password = "";
        string user_nama = "";
        string user_tanggal_lahir = "";
        string user_no_telp = "";
        private void btnPilihUser_Click(object sender, RoutedEventArgs e)
        {
            PilihUser pu = new PilihUser();
            pu.ShowDialog();
            clear();
            user_id = pu.id;
            user_username = pu.username;
            user_password = pu.password;
            user_nama = pu.nama;
            user_tanggal_lahir = pu.tanggal_lahir;
            user_no_telp = pu.no_telp;
            lblUsername.Text = user_username.ToString();
            lblNama.Text = user_nama.ToString();
            lblTanggalLahir.Text = user_tanggal_lahir.ToString();
            lblNoTelp.Text = user_no_telp.ToString();
            tbUsername.Text = user_username;
            tbUsername.IsEnabled = false;
        }

        //private void cari()
        //{
        //    dUser = new DataTable();
        //    OracleCommand cmd = new OracleCommand();
        //    da = new OracleDataAdapter();
        //    int cekprem = 0;
        //    cmd.Connection = conn;
        //    cmd.CommandText = $"select id as " + '"' + "No" + '"' + ", username as " + '"' + "Username" + '"' + "," +
        //            "nama as " + '"' + "Nama" + '"' + ", to_char(tanggal_lahir, 'dd/MM/yyyy') as " + '"' + "Tanggal Lahir" + '"' + ", no_telp as " + '"' + "No Telp" + '"' + "from users";
        //    string comm = " where";
        //    string keyword = Convert.ToString(tbKeyword.Text);
        //    string berdasarkan = "username";

        //    if (rbUsername.IsChecked == true)
        //    {
        //        berdasarkan = "username";
        //    }
        //    else if (rbNamaLengkap.IsChecked == true)
        //    {
        //        berdasarkan = "nama";
        //    }
            
            
            
        //    comm += $" upper({berdasarkan}) like upper('%{keyword}%')";

        //    comm += $" order by ID";
        //    cmd.CommandText += comm;
        //    Console.WriteLine(cmd.CommandText);
        //    conn.Close();
        //    conn.Open();
        //    cmd.ExecuteReader();
        //    da.SelectCommand = cmd;
        //    da.Fill(dUser);
        //    dgvUser.ItemsSource = dUser.DefaultView;
        //    conn.Close();
        //}
    }
}
