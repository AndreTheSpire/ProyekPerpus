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
using System.IO;

namespace PerpusPCS
{
    /// <summary>
    /// Interaction logic for ConnectionPage.xaml
    /// </summary>
    public partial class ConnectionPage : Window
    {
        public static OracleConnection conn;
        public static String source, userId, pass;

        string path = Directory.GetCurrentDirectory() + "\\logins.txt";
        public ConnectionPage()
        {
             InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            loadSavedLogins();
        }

        private void dgvLogins_Loaded(object sender, RoutedEventArgs e)
        {
            //dgvLogins.Columns[1].Header = "Database";
            //dgvLogins.Columns[2].Header = "Username";
            //dgvLogins.Columns[3].Header = "Password";
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (listLogins.SelectedIndex != -1)
            {
                string line = listLogins.SelectedItem.ToString();
                Console.WriteLine(line);
                string[] lines = line.Split(' ');
                TxtDatabase.Text = lines[0];
                TxtPass.Text = lines[1];
                TxtUsername.Text = lines[2];
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string line = TxtDatabase.Text + ' ' + TxtUsername.Text+ ' ' +  TxtPass.Text;

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(line);
                sw.Close();
            }
            loadSavedLogins();
        }

        public void loadSavedLogins()
        {
            listLogins.Items.Clear();
            if (File.Exists(path))
            {
                Console.WriteLine("file ada pada : " + path);
                StreamReader sr = new StreamReader(path);
                string line = sr.ReadLine();
                while (line != null)
                {
                    Console.WriteLine(line);
                    listLogins.Items.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            else
            {
                Console.WriteLine("file tidak ada pada : "+path);
                using (FileStream fs = File.Create(path))
                {
                    
                }
            }
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            source = TxtDatabase.Text;
            userId = TxtUsername.Text;
            pass = TxtPass.Text;
            try
            {
                conn = new OracleConnection("Data Source = " + source + "; User ID = " + userId + "; password = " + pass);
                conn.Open();
                conn.Close();
                MenuPage mp = new MenuPage();
                mp.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
