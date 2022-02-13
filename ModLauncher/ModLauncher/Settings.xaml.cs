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

namespace ModLauncher
{
    public partial class Settings : Window
    {
        // Client Settings.
        public static int RAM { get; set; }

        // Server Settings.
        public static string IP_ADDRESS { get; set; }
        public static int PORT { get; set; }


        public Settings()
        {
            InitializeComponent();
        }

        public static void GetError(string error)
        {
            MessageBox.Show("Error: " + error);
        }


        #region UI
        private void ModLauncher_ContentRendered(object sender, EventArgs e)
        {
            SaveSystem.LoadSettings();

            RamUsage_Box.Text = RAM.ToString();
            IPAddress_Box.Text = IP_ADDRESS;
            Port_Box.Text = PORT.ToString();
        }

        private void SettingsClose_Btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {
            RAM = int.Parse(RamUsage_Box.Text);
            IP_ADDRESS = IPAddress_Box.Text;
            PORT = int.Parse(Port_Box.Text);

            SaveSystem.SaveSettings();
        }

        private void Reset_Btn_Click(object sender, RoutedEventArgs e)
        {
            RAM = 6144;
            IP_ADDRESS = "116.202.144.25";
            PORT = 20;

            RamUsage_Box.Text = "6144";
            IPAddress_Box.Text = "116.202.144.25";
            Port_Box.Text = "20";

            SaveSystem.SaveSettings();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        #endregion

        private void LogOutMicrosoft_Btn_Click(object sender, RoutedEventArgs e)
        {
            MC_Game.XBoxLogout();
        }
    }
}
