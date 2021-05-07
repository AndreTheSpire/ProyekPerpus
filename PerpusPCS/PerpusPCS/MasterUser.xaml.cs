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
            tbID.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = false;
        }
        private void loadData()
        {
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = "select id as " + '"' + "No" + '"' + ", username as " + '"' + "Username" + '"' + ", password as " + '"' + "Password" + '"' + "," +
                "nama as " + '"' + "Nama" + '"' + ", to_char(tanggal_lahir, 'dd/MM/yyyy') as " + '"' + "Tanggal Lahir" + '"' + ", no_telp as " + '"' + "No Telp" + '"' + "from users";
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(ds);
            dgvUser.ItemsSource = ds.DefaultView;
            conn.Close();
            OracleCommand cmdFunction = new OracleCommand()
            {
                CommandType = CommandType.StoredProcedure,
                Connection = conn,
                CommandText = "autogen_id_user"
            };

            cmdFunction.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.ReturnValue,
                ParameterName = "id_user",
                OracleType = OracleType.VarChar,
                Size = 100
            });
            conn.Open();
            cmdFunction.ExecuteNonQuery();
            tbID.Text = cmdFunction.Parameters["id_user"].Value.ToString();
            conn.Close();
        }
        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void dgvUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
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
                    btnDelete.IsEnabled = true;
                    btnInsert.IsEnabled = false;
                    btnUpdate.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }
        private void clear()
        {
            tbID.Text = "";
            tbNama.Text = "";
            tbUsername.Text = "";
            tbPassword.Text = "";
            tbNoTelp.Text = "";
            DPTglLahir.SelectedDate = null;
            btnDelete.IsEnabled = false;
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(tbID.Text);
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"delete from pembelian_premium where id_user = '{id}'";
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                cmd.CommandText = $"select count(id) from h_peminjaman where id_user = '{id}'";
                conn.Open();
                int idpeminjaman = Convert.ToInt32(cmd.ExecuteScalar());
                int[] hpeminjamke = new int[idpeminjaman];
                int ctr = 0;
                conn.Close();
                cmd.CommandText = $"select id from h_peminjaman where id_user = '{id}'";
                conn.Open();
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    hpeminjamke[ctr] = Convert.ToInt32(reader.GetValue(0));
                    ctr++;
                }
                conn.Close();
                for (int i = 0; i < hpeminjamke.Length; i++)
                {
                    cmd.CommandText = $"delete from d_peminjaman where id_h_peminjaman = '{hpeminjamke[i]}'";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.CommandText = $"delete from pengembalian where id_h_peminjaman = '{hpeminjamke[i]}'";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    cmd.CommandText = $"delete from h_peminjaman where id = '{hpeminjamke[i]}'";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                cmd.CommandText = $"delete from users where id = '{id}'";
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                loadData();
                clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (tbID.Text.Equals("") || tbUsername.Text.Equals("") || tbPassword.Text.Equals("") || tbNama.Text.Equals("") || tbNoTelp.Text.Equals("") || DPTglLahir.SelectedDate == null)
            {
                MessageBox.Show("Field Harus Terisi");
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(tbID.Text);
                    String username = tbUsername.Text;
                    String password = tbPassword.Text;
                    String nama = tbNama.Text;
                    String[] pecah = DPTglLahir.SelectedDate.Value.ToString().Split(' ');
                    String[] pecahtanggallahir = pecah[0].Split('/');
                    String tanggallahir = pecahtanggallahir[1] + "/" + pecahtanggallahir[0] + "/" + pecahtanggallahir[2];
                    String notelp = tbNoTelp.Text;
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"insert into users values({id}, '{username}', '{password}', '{nama}', to_date('{tanggallahir}', 'dd/MM/yyyy'), '{notelp}')";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    loadData();
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void tbNoTelp_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int numberpassed = Convert.ToInt32(tbNoTelp.Text);
            }
            catch
            {
                int panjang = tbNoTelp.Text.Length;
                if (panjang >= 1)
                {
                    tbNoTelp.Text = tbNoTelp.Text.Substring(0, panjang - 1);
                    tbNoTelp.Focus();
                    tbNoTelp.SelectionStart = panjang;
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (tbID.Text.Equals("") || tbUsername.Text.Equals("") || tbPassword.Text.Equals("") || tbNama.Text.Equals("") || tbNoTelp.Text.Equals("") || DPTglLahir.SelectedDate == null)
            {
                MessageBox.Show("Field Harus Terisi");
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(tbID.Text);
                    String username = tbUsername.Text;
                    String password = tbPassword.Text;
                    String nama = tbNama.Text;
                    String[] pecah = DPTglLahir.SelectedDate.Value.ToString().Split(' ');
                    String[] pecahtanggallahir = pecah[0].Split('/');
                    String tanggallahir = pecahtanggallahir[1] + "/" + pecahtanggallahir[0] + "/" + pecahtanggallahir[2];
                    String notelp = tbNoTelp.Text;
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"update users set username = '{username}', password = '{password}', nama = '{nama}', tanggal_lahir = to_date('{tanggallahir}', 'dd/MM/yyyy'), no_telp = '{notelp}' where id = {id}";
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    loadData();
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
