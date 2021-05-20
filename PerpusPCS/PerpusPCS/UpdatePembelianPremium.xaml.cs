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
            cmd.CommandText = $"select PP.ID ,U.username,U.nama,P.jenis,case PP.status when 0 then 'Pending' when 1 then 'Accepted' when 2 then 'Rejected' end as {'"'+"Status"+'"'} ,PP.metode_pembayaran,to_char(PP.created_at, 'dd/MM/yyyy') from pembelian_premium PP, users U, premium P where PP.id_user = U.ID and PP.id_premium = p.ID order by 1 asc";
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
        private void doSort(string kode)
        {
            dv.Sort = kode;
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            doFilter("");
            doSort("");
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            doFilter("id = 0");
        }

        

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
