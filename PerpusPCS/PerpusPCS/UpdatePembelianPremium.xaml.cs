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
    /// Interaction logic for UpdatePembelianPremium.xaml
    /// </summary>
    public partial class UpdatePembelianPremium : Window
    {
        OracleConnection conn;
        DataTable dt;
        DataView dv;
        OracleDataAdapter da;

        public UpdatePembelianPremium()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            conn = ConnectionPage.conn;
            loadAwalan();
        }
        private void loadAwalan()
        {
            loadCbStatus();
            loadCbPremium();
            loadDataGrid();
        }

        private void loadDataGrid()
        {
            conn.Close();
            dt = new DataTable();
            da = new OracleDataAdapter();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;
            cmd.CommandText = $"select PP.ID ,U.username,U.nama,P.jenis,case PP.status when 0 then 'Pending' when 1 then 'Accepted' when 2 then 'Rejected' end as {'"'+"Status"+'"'} ,PP.metode_pembayaran,to_char(PP.created_at,'DD/MM/YYYY') as {'"'+"Created_At"+'"'} from pembelian_premium PP, users U, premium P where PP.id_user = U.ID and PP.id_premium = p.ID order by 1 asc";
            conn.Open();
            cmd.ExecuteReader();
            da.SelectCommand = cmd;
            da.Fill(dt);
            dv = new DataView(dt);
            dgvPembelianPremium.ItemsSource = dv;
            conn.Close();
        }
        private void dgvPembelianPremium_Loaded(object sender, RoutedEventArgs e)
        {
            dgvPembelianPremium.Columns[0].Width = DataGridLength.Auto;
            dgvPembelianPremium.Columns[1].Width = DataGridLength.Auto;
            dgvPembelianPremium.Columns[2].Width = DataGridLength.Auto;
            dgvPembelianPremium.Columns[3].Width = DataGridLength.Auto;
            dgvPembelianPremium.Columns[4].Width = DataGridLength.Auto;
            dgvPembelianPremium.Columns[0].Header = "ID";
            dgvPembelianPremium.Columns[1].Header = "Username";
            dgvPembelianPremium.Columns[2].Header = "Nama Lengkap";
            dgvPembelianPremium.Columns[3].Header = "Jenis Premium";
            dgvPembelianPremium.Columns[4].Header = "Status";
            dgvPembelianPremium.Columns[5].Header = "Pembayaran";
            dgvPembelianPremium.Columns[6].Header = "Created At";
        }

        private void loadCbStatus()
        {
            cbStatus.Items.Add(new ComboBoxItem { Content = "Pending", Name = "S0" });
            cbStatus.Items.Add(new ComboBoxItem { Content = "Accepted", Name = "S1" });
            cbStatus.Items.Add(new ComboBoxItem { Content = "Rejected", Name = "S2" });
            cbStatus.SelectedIndex = 0;
        }
        private void loadCbPremium()
        {
            OracleCommand cmd = new OracleCommand("select * from premium", conn);
            conn.Close();
            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cbJenisPremium.Items.Add(new ComboBoxItem { Content = reader.GetValue(1).ToString(), Name = "S" + reader.GetValue(0).ToString() }) ;
            }
            cbJenisPremium.SelectedIndex = 0;
            conn.Close();
        }

        private void doFilter(string kode)
        {
            dv.RowFilter = kode;
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            doFilter("");
            clearFilter();
            btnCancel_Click(sender, e);
        }
        private void clearFilter()
        {
            txtUsername.Text = "";
            txtNamaUser.Text = "";
            cbJenisPremium.SelectedIndex = 0;
            cbStatus.SelectedIndex = 0;
            tglAkhir.SelectedDate = null;
            tglAwal.SelectedDate = null;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string kode = "1=1";
            if (txtUsername.Text.Length > 0) kode += $" and username LIKE '%{txtUsername.Text}%'";
            if (txtNamaUser.Text.Length > 0) kode += $" and nama LIKE '%{txtNamaUser.Text}%'";
            ComboBoxItem cbPremium = (ComboBoxItem)cbJenisPremium.SelectedItem;
            if (cbJenisPremium.SelectedIndex != -1) kode += $" and jenis = '{cbPremium.Content}'";
            ComboBoxItem tempCbStatus = (ComboBoxItem)cbStatus.SelectedItem;
            if (cbStatus.SelectedIndex != -1) kode += $" and status = '{tempCbStatus.Content}'";
            if (tglAwal.SelectedDate != null && tglAkhir.SelectedDate != null)
            {
                kode += $" and  CONVERT(substring(created_at,4,3)+substring(created_at,1,3)+substring(created_at,7,4), 'System.DateTime') >= '{tglAwal.SelectedDate}'";
                kode += $" and  CONVERT(substring(created_at,4,3)+substring(created_at,1,3)+substring(created_at,7,4), 'System.DateTime') <= '{tglAkhir.SelectedDate}'";
            }
            doFilter(kode);
        }



        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            conn.Close();
            conn.Open();
            using(OracleTransaction trans = conn.BeginTransaction())
            {
                try
                {
                    int status = 0;
                    int id = Convert.ToInt32(txtIdPembayaranTerpilih.Text);
                    if ((bool)rbAccepted.IsChecked) status = 1;
                    else if ((bool)rbRejected.IsChecked) status = 2;

                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"Update pembelian_premium set status = {status} where id = {id}";
                    cmd.ExecuteNonQuery();

                    trans.Commit();
                    loadDataGrid();
                    dgvPembelianPremium_Loaded(sender, e);
                    btnCancel_Click(sender, e);
                    MessageBox.Show("Berhasil DiUpdate!","Berhasil",MessageBoxButton.OK,MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    MessageBox.Show(ex.Message);
                }
            }
            conn.Close();
        }

        private void dgvPembelianPremium_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataRowView temp = (DataRowView)dgvPembelianPremium.SelectedItem;
            if (temp == null) return;
            int id = Convert.ToInt32(temp[0]);
            string status = temp[4].ToString();
            if (status != "Pending")
            {
                if (status == "Accepted") rbAccepted.IsChecked = true;
                else rbRejected.IsChecked = true;
                rbAccepted.IsEnabled = false;
                rbRejected.IsEnabled = false;
            }
            else
            {
                btnCancel_Click(sender, e);
            }
            txtIdPembayaranTerpilih.Text = id.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            rbAccepted.IsEnabled = true;
            rbRejected.IsEnabled = true;
            rbAccepted.IsChecked = false;
            rbRejected.IsChecked = false;
            txtIdPembayaranTerpilih.Text = "-";
        }

        private void btnClearStatus_Click(object sender, RoutedEventArgs e)
        {
            cbStatus.SelectedIndex = -1;
        }

        private void btnClearJenisPremium_Click(object sender, RoutedEventArgs e)
        {
            cbJenisPremium.SelectedIndex = -1;
        }
    }
}
