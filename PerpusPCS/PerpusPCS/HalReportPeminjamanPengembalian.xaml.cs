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
    /// Interaction logic for HalReportPeminjamanPengembalian.xaml
    /// </summary>
    public partial class HalReportPeminjamanPengembalian : Window
    {
        OracleConnection conn;
        public HalReportPeminjamanPengembalian()
        {
            InitializeComponent();
            this.conn = ConnectionPage.conn;
            loaduser();
        }
        public void loaduser()
        {
            OracleCommand cmd2 = new OracleCommand();
            cmd2.Connection = conn;
            conn.Open();
            cmd2.CommandText = "select * from users order by 1";
            OracleDataReader reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                cbuser.Items.Add(new ComboBoxItem
                {
                    Name = reader2.GetValue(1).ToString(),
                    Content = reader2.GetValue(0).ToString() + " - " + reader2.GetValue(1).ToString()
                });
            }
            cbuser.SelectedValuePath = "Name";
            cbuser.SelectedIndex = 0;
            conn.Close();
        }

        private void btncari_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnback_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
