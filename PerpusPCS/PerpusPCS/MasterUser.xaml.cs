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
            loadData(null);
            tbID.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnInsert.IsEnabled = true;
            btnUpdate.IsEnabled = false;
            tbPassword.IsEnabled = false;
        }
        private void loadData(String kode)
        {
            ds = new DataTable();
            OracleCommand cmd = new OracleCommand();
            da = new OracleDataAdapter();
            cmd.Connection = conn;
            if (kode == null)
            {
                cmd.CommandText = "select id, username, password, nama, to_char(tanggal_lahir, 'dd/MM/yyyy'), no_telp from users where status_delete = 0 order by id";
            }
            else
            {
                cmd.CommandText = kode;
            }
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
                OracleDbType = OracleDbType.Varchar2,
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
                    tbPassword.IsEnabled = true;
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
            loadData(null);
            dgvUser_Loaded(sender, e);
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
            tbPassword.IsEnabled = false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(tbID.Text);
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"update users set status_delete = 1 where id = {id}";
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
                clear();
                loadData(null);
                dgvUser_Loaded(sender, e);
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
                //try
                {
                    if (DPTglLahir.SelectedDate > DateTime.Now)
                    {
                        MessageBox.Show("Tanggal Lahir Tidak Boleh Melebihi Hari Ini");
                    }
                    else
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select count(*) from users";
                        conn.Open();
                        int jumlah = Convert.ToInt32(cmd.ExecuteScalar());
                        String[] daftaruser = new string[jumlah];
                        conn.Close();
                        cmd.CommandText = "select username from users";
                        conn.Open();
                        OracleDataReader reader = cmd.ExecuteReader();
                        int ctr = 0;
                        while (reader.Read())
                        {
                            daftaruser[ctr] = reader.GetString(0);
                            ctr++;
                        }
                        conn.Close();
                        bool userada = false;
                        for (int i = 0; i < daftaruser.Length; i++)
                        {
                            if (tbUsername.Text.Equals(daftaruser[i]))
                            {
                                userada = true;
                            }
                        }
                        if (userada)
                        {
                            MessageBox.Show("Username pernah dipakai");
                        }
                        else
                        {
                            int id = Convert.ToInt32(tbID.Text);
                            String username = tbUsername.Text;
                            String password = tbPassword.Text;
                            String nama = tbNama.Text;
                            String[] pecah = DPTglLahir.SelectedDate.Value.ToString().Split(' ');
                            String[] pecahtanggallahir = pecah[0].Split('/');
                            String tanggallahir = pecahtanggallahir[1] + "/" + pecahtanggallahir[0] + "/" + pecahtanggallahir[2];
                            String notelp = tbNoTelp.Text;
                            cmd.Connection = conn;
                            cmd.CommandText = $"insert into users values({id}, '{username}', '{password}', '{nama}', to_date('{tanggallahir}', 'dd/MM/yyyy'), '{notelp}', 0)";
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            clear();
                            loadData(null);
                            dgvUser_Loaded(sender, e);
                        }
                    }
                }
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
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
                    clear();
                    loadData(null);
                    dgvUser_Loaded(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            String kode = "select id, username, password, nama, to_char(tanggal_lahir, 'dd/MM/yyyy'), no_telp from users where status_delete = 0";
            if (tbFilterUsername.Text.Length > 0)
            {
                kode += $" and username like '%{tbFilterUsername.Text}%'";
            }
            if (tbFilterNama.Text.Length > 0)
            {
                kode += $" and upper(nama) like upper('%{tbFilterNama.Text}%')";
            }
            if (DPFilterTgl.SelectedDate != null)
            {
                String[] pecah = DPFilterTgl.SelectedDate.Value.ToString().Split(' ');
                String[] pecahtanggallahir = pecah[0].Split('/');
                String tanggallahir = pecahtanggallahir[1] + "/" + pecahtanggallahir[0] + "/" + pecahtanggallahir[2];
                kode += $" and tanggal_lahir = to_date('{tanggallahir}', 'dd/MM/yyyy')";
            }
            if (tbFilterNoTelp.Text.Length > 0)
            {
                kode += $" and no_telp like '%{tbFilterNoTelp.Text}%'";
            }
            loadData(kode);
            tbFilterNoTelp.Text = "";
            tbFilterNama.Text = "";
            tbFilterUsername.Text = "";
            DPFilterTgl.SelectedDate = null;
        }

        private void dgvUser_Loaded(object sender, RoutedEventArgs e)
        {
            dgvUser.Columns[0].Width = DataGridLength.Auto;
            dgvUser.Columns[0].Header = "ID";
            dgvUser.Columns[1].Header = "Username";
            dgvUser.Columns[2].Header = "Password";
            dgvUser.Columns[3].Header = "Nama";
            dgvUser.Columns[4].Header = "Tanggal Lahir";
            dgvUser.Columns[5].Header = "No Telp";
        }

        private void tbUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbPassword.IsEnabled == false)
            {
                tbPassword.Text = tbUsername.Text;
            }
        }
    }
}
