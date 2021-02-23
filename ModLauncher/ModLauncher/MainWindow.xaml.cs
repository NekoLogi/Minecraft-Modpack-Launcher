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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ModLauncher
{
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        void AppStartup() {
            FileServer.Connect(0);
            FileServer.Connect(1);

            if (FileServer.isConnected) {
                AvalVerLabel.Content = FileServer.info[0];
                PatchBox.Text = FileServer.info[1];
            } else {
                SetOffline();
            }
        }

        void CheckModpack() {
            bool version = FileManager.CheckVersion();
            CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;

            if (FileServer.isConnected) {
                if (Directory.Exists("BlockyCrafters")) {
                    if (version) {
                        Start_Btn.Content = "Spielen";
                    } else {
                        Start_Btn.Content = "Update";
                    }
                } else {
                    Start_Btn.Content = "Installieren";
                }
            } else {
                if (Directory.Exists("BlockyCrafters")) {
                    Start_Btn.Content = "Spielen";
                } else {
                    Start_Btn.Content = "Installieren";
                }
            }
        }

        void SetOffline() {
            AvalVerLabel.Content = "Available Version: No Version";
            PatchBox.Text = "Server offline";
        }

        public void GetError(string error) {
            MessageBox.Show(error);
        }

        private void Start_Btn_Click(object sender, RoutedEventArgs e) {
            switch (Start_Btn.Content) {
                case "Installieren":
                    if (FileServer.isConnected) {
                        if (Directory.Exists("BlockyCrafters")) {
                            Start_Btn.Content = "Spielen";
                        } else {
                            try {
                                FileServer.Connect(2);
                                Start_Btn.Content = "Spielen";
                                CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;

                                MessageBox.Show("Modpack installed!");
                            } catch (Exception) {
                                GetError("Failed to install modpack!\n" +
                                    "Check for internet connection and firewall.");
                            }
                        }
                    } else {
                        GetError("No connection to the server!\n" +
                            "If you really want to delete the modpack, delete this folder:\n\n" +
                            Directory.GetCurrentDirectory() +
                            "\\BlockyCrafters");
                    }
                    break;
                case "Update":
                    if (FileServer.isConnected) {
                        try {
                            FileServer.Connect(3);
                            CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;

                            MessageBox.Show("Modpack updated!");
                        } catch (Exception) {
                            GetError("Failed to update modpack!\n" +
                                "Check for internet connection and firewall.");
                        }
                    } else {
                        GetError("No connection to the server!\n" +
                            "Failed to update modpack!");
                    }
                    break;
                case "Spielen":
                    if (File.Exists("BlockyCrafters/Login.txt")) {
                        MC_Game.Login();
                    } else {
                        MCClient_Login mcClient = new MCClient_Login();
                        mcClient.Show();
                    }
                    break;
            }
        }

        private void ModLauncher_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Application.Current.Shutdown();
        }

        private void ModLauncher_ContentRendered(object sender, EventArgs e) {
            AppStartup();
            CheckModpack();
        }

        private void Reinstall_Btn_Click(object sender, RoutedEventArgs e) {
            if (FileServer.isConnected) {
                try {
                    FileServer.Connect(2);
                    Start_Btn.Content = "Spielen";

                    MessageBox.Show("Modpack installed!");
                } catch (Exception) {
                    GetError("Failed to install modpack!\n" +
                        "Check for internet connection and firewall.");
                }
            } else {
                GetError("No connection to the server!\n" +
                    "If you really want to delete the modpack, delete this folder:\n\n" +
                    Directory.GetCurrentDirectory() +
                    "\\BlockyCrafters");
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        private void Close_Btn_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void Minimize_Btn_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Settings_Btn_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Settings coming soon!");
        }

        private void Info_Btn_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Info coming soon!");
        }
    }
}
