using System;
using System.Collections.Generic;
using System.Data;
using Oracle.DataAccess.Client;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for HalReportDetailBuku.xaml
    /// </summary>
    public partial class HalReportDetailBuku : Window
    {
        OracleConnection conn;
        public HalReportDetailBuku()
        { 
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            this.conn = ConnectionPage.conn;
            loadData();
        }
        private void loadData()
        {
            loadComboBoxBahasa();
            loadComboBoxGenre();
            tampilanAwal();
            
        }

        private void tampilanAwal()
        {
            txtAuthor.Text = "";
            txtHalamanAkhir.Text = "";
            txtHalamanAwal.Text = "";
            txtPenerbit.Text = "";
            rbFree.IsChecked = false;
            rbPremium.IsChecked = false;
        }

        private void loadComboBoxBahasa()
        {
            OracleCommand cmd = new OracleCommand("select bahasa from buku where status_delete = 0 group by bahasa ", conn);
            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboBoxItem temp = new ComboBoxItem{Content = reader.GetValue(0).ToString(),Name = reader.GetValue(0).ToString()};
                cbBahasa.Items.Add(temp);
            }
            conn.Close();
            cbBahasa.SelectedIndex = -1;
        }
        private void loadComboBoxGenre()
        {
            OracleCommand cmd = new OracleCommand("select kb.genre from kategori_buku kb left join buku b on kb.id_buku = b.id where b.status_delete = 0 group by genre", conn);
            conn.Open();
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ComboBoxItem temp = new ComboBoxItem { Content = reader.GetValue(0).ToString(), Name = reader.GetValue(0).ToString() };
                cbGenre.Items.Add(temp);
            }
            conn.Close();
            cbGenre.SelectedIndex = -1;
        }

        private void btnBackToMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCreateReport_Click(object sender, RoutedEventArgs e)
        {
            string author = "Semua";
            string penerbit = "Semua";
            int status = 2;
            int halaman_awal = 0;
            int halaman_akhir = int.MaxValue;
            string bahasa = "Semua";
            string genre = "Semua";

            //pengecekan value parameter

            //parameter author
            if (txtAuthor.Text.Length > 0)
            {//pengecekan author ada atau tidak
                conn.Close();
                conn.Open();
                OracleCommand cmd = new OracleCommand($"select count(*) from buku where author = '{txtAuthor.Text}'", conn);
                int temp_ctr = 0;
                temp_ctr = Convert.ToInt32(cmd.ExecuteScalar());
                if (temp_ctr == 0)
                {

                    OracleCommand cmd2 = new OracleCommand($"select count(*) from buku where upper(author) like upper('%{txtAuthor.Text}%')",conn);
                    int jum_author = Convert.ToInt32(cmd2.ExecuteScalar());
                    //MessageBox.Show('"'+txtAuthor.Text+'"');
                    if (jum_author == 0)
                    {
                        MessageBox.Show("Tidak Ditemukan Author, Periksa Kembali", "Author Not Found(1)");
                        return;
                    }

                    cmd2.CommandText = $"select Author from buku where upper(author) like upper('%{txtAuthor.Text}%')";
                    string possible = cmd2.ExecuteScalar().ToString();
                    if (possible.Length == 0)
                    {//bila tidak ditemukan author di database (atau typo ditengah")
                        MessageBox.Show("Tidak Ditemukan Author, Periksa Kembali", "Author Not Found(2)");
                        return;
                    }
                    else
                    {//ditemukan author menggunakan like
                        if (MessageBox.Show($"Tidak Ditemukan Author, Apakah yang dikmaksud : '{possible}'", "Author Not Found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            author = possible;
                            txtAuthor.Text = possible;
                        }
                        else
                        {
                            MessageBox.Show("Tidak Ditemukan Author, Periksa Kembali", "Author Not Found(3)");
                            return;
                        }
                    }
                }
                else
                {
                    author = txtAuthor.Text;
                }
                conn.Close();
            }
            //parameter Penerbit
            if (txtPenerbit.Text.Length > 0)
            {//pengecekan Penerbit ada atau tidak
                conn.Close();
                conn.Open();
                OracleCommand cmd = new OracleCommand($"select count(*) from buku where Penerbit = '{txtPenerbit.Text}'", conn);
                int temp_ctr = 0;
                temp_ctr = Convert.ToInt32(cmd.ExecuteScalar());
                if (temp_ctr == 0)
                {
                    OracleCommand cmd2 = new OracleCommand($"select count(*) from buku where upper(Penerbit) like upper('%{txtPenerbit.Text}%')", conn);
                    int jumlah_penerbit = Convert.ToInt32(cmd2.ExecuteScalar());
                    if (jumlah_penerbit == 0)
                    {
                        MessageBox.Show("Tidak Ditemukan Penerbit, Periksa Kembali", "Penerbit Not Found(1)");
                        return;
                    }
                    cmd2.CommandText = $"select penerbit from buku where upper(penerbit) like upper('%{txtPenerbit.Text}%')";
                    string possible = cmd2.ExecuteScalar().ToString();
                    if (possible.Length == 0)
                    {//bila tidak ditemukan Penerbit di database (atau typo ditengah")
                        MessageBox.Show("Tidak Ditemukan Penerbit, Periksa Kembali", "Penerbit Not Found(2)");
                        return;
                    }
                    else
                    {//ditemukan Penerbit menggunakan like
                        if (MessageBox.Show($"Tidak Ditemukan Penerbit, Apakah yang dikmaksud : {possible}", "Penerbit Not Found", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            penerbit = possible;
                            txtPenerbit.Text = possible;
                        }
                        else
                        {
                            MessageBox.Show("Tidak Ditemukan Penerbit, Periksa Kembali", "Penerbit Not Found(3)");
                            return;
                        }
                    }
                }
                else
                {
                    penerbit = txtPenerbit.Text;
                }
                conn.Close();
            }
            //parameter status
            if ((bool)rbFree.IsChecked) status = 0;
            else if ((bool)rbPremium.IsChecked) status = 1;

            //parameter halaman
            if (txtHalamanAwal.Text.Length > 0) halaman_awal = Convert.ToInt32(txtHalamanAwal.Text);
            if (txtHalamanAkhir.Text.Length > 0) halaman_akhir = Convert.ToInt32(txtHalamanAkhir.Text);

            //parameter bahasa
            if (cbBahasa.SelectedIndex != -1)
            {
                ComboBoxItem temp = (ComboBoxItem)cbBahasa.SelectedItem;
                bahasa = temp.Content.ToString();
            }

            //parameter Genre
            if (cbGenre.SelectedIndex != -1)
            {
                ComboBoxItem temp = (ComboBoxItem)cbGenre.SelectedItem;
                genre = temp.Content.ToString();
            }


            //end pengecekan

            DetailBuku rptDB = new DetailBuku();
            rptDB.SetDatabaseLogon(ConnectionPage.userId, ConnectionPage.pass, ConnectionPage.source, "");

            //memasukkan parameter
            //rptDB.SetParameterValue(nameParam,Value);
            rptDB.SetParameterValue("Author", author);
            rptDB.SetParameterValue("status", status);
            rptDB.SetParameterValue("Penerbit", penerbit);
            rptDB.SetParameterValue("halamanAwal", halaman_awal);
            rptDB.SetParameterValue("HalamanAkhir", halaman_akhir);
            rptDB.SetParameterValue("Bahasa", bahasa);
            rptDB.SetParameterValue("Genre", genre);
            crvReport.ViewerCore.ReportSource = rptDB;
        }

        private void txtHalamanAwal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void txtHalamanAkhir_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tampilanAwal();
            cbBahasa.SelectedIndex = -1;
            cbGenre.SelectedIndex = -1;
        }
    }
}
