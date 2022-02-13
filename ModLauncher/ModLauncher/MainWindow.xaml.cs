using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ModLauncher
{
    public partial class MainWindow : Window
    {
        public static MainWindow CurrentWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AppStartup()
        {
            CurrentWindow = this;

            FileServer.Connect(0);
            FileServer.Connect(1);

            if (FileServer.isConnected)
            {
                Dispatcher.Invoke(() =>
                {
                    AvalVerLabel.Content = FileServer.info[0];
                    PatchBox.Text = FileServer.info[1];
                });
            }
            else
            {
                SetOffline();
            }
        }

        private void CheckModpack()
        {
            bool version = FileManager.CheckVersion();
            Dispatcher.Invoke(() =>
            {
                CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;
            });


            if (FileServer.isConnected)
            {
                if (Directory.Exists("BlockyCrafters"))
                {
                    if (version)
                    {
                        ClientState(2);
                    }
                    else
                    {
                        ClientState(1);
                    }
                }
                else
                {
                    ClientState(0);
                }
            }
            else
            {
                if (Directory.Exists("BlockyCrafters"))
                {
                    ClientState(2);
                }
                else
                {
                    ClientState(0);
                }
            }
        }

        private void SetOffline()
        {
            Dispatcher.Invoke(() =>
            {
                AvalVerLabel.Content = "Available Version: No Version";
                PatchBox.Text = "Server offline";
            });
        }

        public void GetError(string error)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Error: " + error);
            });
        }

        private void UpdateGame()
        {
            try
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Visibility = Visibility.Visible;
                    });
                    ClientState(3);
                    FileServer.Connect(3);

                    ClientState(2);
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Visibility = Visibility.Hidden;
                        MessageBox.Show("Modpack updated!");
                    });
                });
            }
            catch (Exception)
            {
                File.Delete("Cache/blockycrafters_update.zip");

                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Visibility = Visibility.Hidden;
                });
                GetError("Failed to update modpack!\n" +
                    "Check for internet connection and firewall.");
                ClientState(2);
            }
        }

        private void InstallGame()
        {
            if (Directory.Exists("BlockyCrafters"))
            {
                Dispatcher.Invoke(() =>
                {
                    Start_Btn.Content = "Spielen";
                    MessageBox.Show("Modpack already installed!");
                });
                ClientState(4);
            }
            else
            {
                try
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Visibility = Visibility.Visible;
                        });

                        FileServer.Connect(2);

                        ClientState(2);
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Visibility = Visibility.Hidden;
                            MessageBox.Show("Modpack installed!");
                        });
                    });
                }
                catch (Exception)
                {
                    File.Delete("Cache/blockycrafters.zip");

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar.Visibility = Visibility.Hidden;
                    });
                    GetError("Failed to install modpack!\n" +
                        "Check for internet connection and firewall.");

                    ClientState(4);
                }
            }
        }

        private void StartGame()
        {
            if (Directory.Exists("BlockyCrafters"))
            {
                if (File.Exists("BlockyCrafters/Login.txt"))
                {
                    MC_Game.Login();
                }
                else
                {
                    ClientState(3);
                    MCClient_Login.main = this;
                    MCClient_Login mcClient = new MCClient_Login();
                    mcClient.Show();
                }
            }
            else
            {
                ClientState(0);

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Failed to start modpack!");
                });
            }
        }

        // 0 = Install | 1 = Update | 2 = Play | 3 = In Progress | 4 = Error
        private void ClientState(int state)
        {
            Dispatcher.Invoke(() =>
            {
                switch (state)
                {
                    case 0:
                        Start_Btn.Content = "Installieren";
                        CurrVerLabel.Content = "Current Version: " + "No Version";
                        Delete_Btn.IsEnabled = true;
                        Start_Btn.IsEnabled = true;
                        break;

                    case 1:
                        Start_Btn.Content = "Update";
                        CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;
                        Delete_Btn.IsEnabled = true;
                        Start_Btn.IsEnabled = true;
                        break;

                    case 2:
                        Start_Btn.Content = "Spielen";
                        CurrVerLabel.Content = "Current Version: " + FileManager.MOD_VERSION;
                        Delete_Btn.IsEnabled = true;
                        Start_Btn.IsEnabled = true;
                        break;

                    case 3:
                        Delete_Btn.IsEnabled = false;
                        Start_Btn.IsEnabled = false;
                        break;

                    case 4:
                        Delete_Btn.IsEnabled = true;
                        Start_Btn.IsEnabled = true;
                        break;
                }
            });
        }

        #region UI

        private void Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (FileServer.isConnected)
            {
                switch (Start_Btn.Content)
                {
                    case "Installieren":
                        InstallGame();
                        break;

                    case "Update":
                        UpdateGame();
                        break;

                    case "Spielen":
                        StartGame();
                        break;
                }
            }
            else
            {
                GetError(
                    "Connection failed!\n" +
                    "Please check your connection and firewall settings.");
            }
        }

        private void ModLauncher_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ModLauncher_ContentRendered(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                SaveSystem.LoadSettings();
                AppStartup();
                CheckModpack();
            });
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Minimize_Btn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Settings_Btn_Click(object sender, RoutedEventArgs e)
        {
            var menu = new Settings();
            menu.Show();
        }

        private void Info_Btn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Info coming soon!");
        }

        private void Delete_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (FileServer.isConnected)
            {
                if (Directory.Exists("BlockyCrafters"))
                {
                    try
                    {
                        ClientState(3);
                        Directory.Delete("BlockyCrafters", true);

                        Dispatcher.Invoke(() =>
                        {
                            ClientState(0);
                            MessageBox.Show("Modpack deleted!");
                            CurrVerLabel.Content = "Current Version: " + "No Version";
                        });

                    }
                    catch (Exception)
                    {
                        GetError("Failed to delete modpack!");
                        ClientState(4);
                    }
                }
                else
                {
                    GetError(
                        "Failed to delete modpack!\n" +
                        "Modpack doesn't exist.");
                    ClientState(4);
                }
            }
            else
            {
                GetError("No connection to the server!\n" +
                    "If you really want to delete the modpack, delete this folder:\n\n" +
                    Directory.GetCurrentDirectory() +
                    "\\BlockyCrafters");
            }
        }

        #endregion
    }
}
