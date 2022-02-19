using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

            for (int i = 0; i < Enum.GetNames(typeof(FileManager.Modpacks)).Length; i++)
                RefreshTab((FileManager.Modpacks)i);
        }

        private void RefreshTab(FileManager.Modpacks modpack)
        {
            switch (modpack)
            {
                case FileManager.Modpacks.BlockyCrafters:
                    FileServer.Connect(0, FileManager.Modpacks.BlockyCrafters);
                    FileServer.Connect(1, FileManager.Modpacks.BlockyCrafters);
                    BC_CheckModpack();
                    break;
                case FileManager.Modpacks.ProjectCenturos:
                    FileServer.Connect(0, FileManager.Modpacks.ProjectCenturos);
                    FileServer.Connect(1, FileManager.Modpacks.ProjectCenturos);
                    PC_CheckModpack();
                    break;
            }

            if (FileServer.isConnected)
            {
                Dispatcher.Invoke(() =>
                {
                    switch (modpack)
                    {
                        case FileManager.Modpacks.BlockyCrafters:
                            AvalVerLabel_BC.Content = "Available Version: " + FileServer.info[0];
                            PatchBox_BC.Text = FileServer.info[1];
                            break;
                        case FileManager.Modpacks.ProjectCenturos:
                            AvalVerLabel_PC.Content = "Available Version: " + FileServer.info[0];
                            PatchBox_PC.Text = FileServer.info[1];
                            break;
                    }
                });
            }
            else
            {
                switch (modpack)
                {
                    case FileManager.Modpacks.BlockyCrafters:
                        BC_SetOffline();
                        break;
                    case FileManager.Modpacks.ProjectCenturos:
                        PC_SetOffline();
                        break;
                }
            }

        }

        public void GetError(string error)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Error: " + error);
            });
        }


        #region BlockyCrafters
        // 0 = Install | 1 = Update | 2 = Play | 3 = In Progress | 4 = Error
        private void BC_ClientState(int state)
        {
            Dispatcher.Invoke(() =>
            {
                switch (state)
                {
                    case 0:
                        StartBtn_BC.Content = "Installieren";
                        CurrVerLabel_BC.Content = "Current Version: " + "No Version";
                        DeleteBtn_BC.IsEnabled = true;
                        StartBtn_BC.IsEnabled = true;
                        break;

                    case 1:
                        StartBtn_BC.Content = "Update";
                        CurrVerLabel_BC.Content = "Current Version: " + FileManager.MOD_VERSION;
                        DeleteBtn_BC.IsEnabled = true;
                        StartBtn_BC.IsEnabled = true;
                        break;

                    case 2:
                        StartBtn_BC.Content = "Spielen";
                        CurrVerLabel_BC.Content = "Current Version: " + FileManager.MOD_VERSION;
                        DeleteBtn_BC.IsEnabled = true;
                        StartBtn_BC.IsEnabled = true;
                        break;

                    case 3:
                        DeleteBtn_BC.IsEnabled = false;
                        StartBtn_BC.IsEnabled = false;
                        break;

                    case 4:
                        DeleteBtn_BC.IsEnabled = true;
                        StartBtn_BC.IsEnabled = true;
                        break;
                }
            });
        }

        private void BC_CheckModpack()
        {
            bool version = FileManager.CheckVersion(FileManager.Modpacks.BlockyCrafters);
            Dispatcher.Invoke(() =>
            {
                CurrVerLabel_BC.Content = "Current Version: " + FileManager.MOD_VERSION;
            });


            if (FileServer.isConnected)
            {
                if (Directory.Exists($"Launcher/{FileManager.Modpacks.BlockyCrafters}"))
                {
                    if (version)
                    {
                        BC_ClientState(2);
                    }
                    else
                    {
                        BC_ClientState(1);
                    }
                }
                else
                {
                    BC_ClientState(0);
                }
            }
            else
            {
                if (Directory.Exists($"Launcher/{FileManager.Modpacks.BlockyCrafters}"))
                {
                    BC_ClientState(2);
                }
                else
                {
                    BC_ClientState(0);
                }
            }
        }

        private void BC_SetOffline()
        {
            Dispatcher.Invoke(() =>
            {
                AvalVerLabel_BC.Content = "Available Version: No Version";
                PatchBox_BC.Text = "Server offline";
            });
        }

        private void BC_UpdateGame()
        {
            try
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_BC.Visibility = Visibility.Visible;
                    });
                    BC_ClientState(3);
                    FileServer.Connect(3, FileManager.Modpacks.BlockyCrafters);

                    BC_ClientState(2);
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_BC.Visibility = Visibility.Hidden;
                        MessageBox.Show($"{FileManager.Modpacks.BlockyCrafters} updated!");
                    });
                });
            }
            catch (Exception)
            {
                File.Delete($"Cache/{FileManager.Modpacks.BlockyCrafters.ToString().ToLower()}_update.zip");

                Dispatcher.Invoke(() =>
                {
                    ProgressBar_BC.Visibility = Visibility.Hidden;
                });
                GetError($"Failed to update {FileManager.Modpacks.BlockyCrafters}!\n" +
                    "Check for internet connection and firewall.");
                BC_ClientState(2);
            }
        }

        private void BC_InstallGame()
        {
            if (Directory.Exists($"Launcher/{FileManager.Modpacks.BlockyCrafters}"))
            {
                Dispatcher.Invoke(() =>
                {
                    StartBtn_BC.Content = "Spielen";
                    MessageBox.Show($"{FileManager.Modpacks.BlockyCrafters} already installed!");
                });
                BC_ClientState(4);
            }
            else
            {
                try
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar_BC.Visibility = Visibility.Visible;
                        });

                        FileServer.Connect(2, FileManager.Modpacks.BlockyCrafters);

                        BC_ClientState(2);
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar_BC.Visibility = Visibility.Hidden;
                            MessageBox.Show($"{FileManager.Modpacks.BlockyCrafters} installed!");
                        });
                    });
                }
                catch (Exception)
                {
                    File.Delete($"Cache/{FileManager.Modpacks.BlockyCrafters.ToString().ToLower()}.zip");

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_BC.Visibility = Visibility.Hidden;
                    });
                    GetError($"Failed to install {FileManager.Modpacks.BlockyCrafters}!\n" +
                        "Check for internet connection and firewall.");

                    BC_ClientState(4);
                }
            }
        }

        private void BC_StartGame()
        {
            if (Directory.Exists($"Launcher/{FileManager.Modpacks.BlockyCrafters}"))
            {
                if (File.Exists($"Launcher/{FileManager.Modpacks.BlockyCrafters}/Login.txt"))
                {
                    MCClient_Login.MODPACK = (int)FileManager.Modpacks.BlockyCrafters;
                    MC_Game.Login((FileManager.Modpacks)MCClient_Login.MODPACK);
                }
                else
                {
                    BC_ClientState(3);
                    MCClient_Login.main = this;
                    MCClient_Login mcClient = new MCClient_Login();
                    mcClient.Show();
                }
            }
            else
            {
                BC_ClientState(0);

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Failed to start {FileManager.Modpacks.BlockyCrafters}!");
                });
            }
        }

        #endregion

        #region Project Centuros
        // 0 = Install | 1 = Update | 2 = Play | 3 = In Progress | 4 = Error
        private void PC_ClientState(int state)
        {
            Dispatcher.Invoke(() =>
            {
                switch (state)
                {
                    case 0:
                        StartBtn_PC.Content = "Installieren";
                        CurrVerLabel_PC.Content = "Current Version: " + "No Version";
                        DeleteBtn_PC.IsEnabled = true;
                        StartBtn_PC.IsEnabled = true;
                        break;

                    case 1:
                        StartBtn_PC.Content = "Update";
                        CurrVerLabel_PC.Content = "Current Version: " + FileManager.MOD_VERSION;
                        DeleteBtn_PC.IsEnabled = true;
                        StartBtn_PC.IsEnabled = true;
                        break;

                    case 2:
                        StartBtn_PC.Content = "Spielen";
                        CurrVerLabel_PC.Content = "Current Version: " + FileManager.MOD_VERSION;
                        DeleteBtn_PC.IsEnabled = true;
                        StartBtn_PC.IsEnabled = true;
                        break;

                    case 3:
                        DeleteBtn_PC.IsEnabled = false;
                        StartBtn_PC.IsEnabled = false;
                        break;

                    case 4:
                        DeleteBtn_PC.IsEnabled = true;
                        StartBtn_PC.IsEnabled = true;
                        break;
                }
            });
        }

        private void PC_CheckModpack()
        {
            bool version = FileManager.CheckVersion(FileManager.Modpacks.ProjectCenturos);
            Dispatcher.Invoke(() =>
            {
                CurrVerLabel_PC.Content = "Current Version: " + FileManager.MOD_VERSION;
            });


            if (FileServer.isConnected)
            {
                if (Directory.Exists($"Launcher/{FileManager.Modpacks.ProjectCenturos}"))
                {
                    if (version)
                    {
                        PC_ClientState(2);
                    }
                    else
                    {
                        PC_ClientState(1);
                    }
                }
                else
                {
                    PC_ClientState(0);
                }
            }
            else
            {
                if (Directory.Exists($"Launcher/{FileManager.Modpacks.ProjectCenturos}"))
                {
                    PC_ClientState(2);
                }
                else
                {
                    PC_ClientState(0);
                }
            }
        }

        private void PC_SetOffline()
        {
            Dispatcher.Invoke(() =>
            {
                AvalVerLabel_PC.Content = "Available Version: No Version";
                PatchBox_PC.Text = "Server offline";
            });
        }

        private void PC_UpdateGame()
        {
            try
            {
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_PC.Visibility = Visibility.Visible;
                    });
                    BC_ClientState(3);
                    FileServer.Connect(3, FileManager.Modpacks.ProjectCenturos);

                    BC_ClientState(2);
                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_PC.Visibility = Visibility.Hidden;
                        MessageBox.Show($"{FileManager.Modpacks.ProjectCenturos} updated!");
                    });
                });
            }
            catch (Exception)
            {
                File.Delete($"Cache/{FileManager.Modpacks.ProjectCenturos.ToString().ToLower()}_update.zip");

                Dispatcher.Invoke(() =>
                {
                    ProgressBar_PC.Visibility = Visibility.Hidden;
                });
                GetError($"Failed to update {FileManager.Modpacks.ProjectCenturos}!\n" +
                    "Check for internet connection and firewall.");
                PC_ClientState(2);
            }
        }

        private void PC_InstallGame()
        {
            if (Directory.Exists($"Launcher/{FileManager.Modpacks.ProjectCenturos}"))
            {
                Dispatcher.Invoke(() =>
                {
                    StartBtn_PC.Content = "Spielen";
                    MessageBox.Show($"{FileManager.Modpacks.ProjectCenturos} already installed!");
                });
                PC_ClientState(4);
            }
            else
            {
                try
                {
                    Task.Run(() =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar_PC.Visibility = Visibility.Visible;
                        });

                        FileServer.Connect(2, FileManager.Modpacks.ProjectCenturos);

                        PC_ClientState(2);
                        Dispatcher.Invoke(() =>
                        {
                            ProgressBar_PC.Visibility = Visibility.Hidden;
                            MessageBox.Show($"{FileManager.Modpacks.ProjectCenturos} installed!");
                        });
                    });
                }
                catch (Exception)
                {
                    File.Delete($"Cache/{FileManager.Modpacks.ProjectCenturos.ToString().ToLower()}.zip");

                    Dispatcher.Invoke(() =>
                    {
                        ProgressBar_PC.Visibility = Visibility.Hidden;
                    });
                    GetError($"Failed to install {FileManager.Modpacks.ProjectCenturos}!\n" +
                        "Check for internet connection and firewall.");

                    PC_ClientState(4);
                }
            }
        }

        private void PC_StartGame()
        {
            if (Directory.Exists($"Launcher/{FileManager.Modpacks.ProjectCenturos}"))
            {
                MCClient_Login.MODPACK = (int)FileManager.Modpacks.ProjectCenturos;

                if (File.Exists($"Launcher/{FileManager.Modpacks.ProjectCenturos}/Login.txt"))
                {
                    MC_Game.Login((FileManager.Modpacks)MCClient_Login.MODPACK);
                }
                else
                {
                    PC_ClientState(3);
                    MCClient_Login.main = this;
                    MCClient_Login mcClient = new MCClient_Login();
                    mcClient.Show();
                }
            }
            else
            {
                PC_ClientState(0);

                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Failed to start {FileManager.Modpacks.ProjectCenturos}!");
                });
            }
        }

        #endregion


        #region UI

        private void Start_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (FileServer.isConnected)
            {
                switch ((sender as Button).Name.ToString().Split('_')[1])
                {
                    case "BC":
                        switch ((sender as Button).Content.ToString())
                        {
                            case "Installieren":
                                BC_InstallGame();
                                break;

                            case "Update":
                                BC_UpdateGame();
                                break;

                            case "Spielen":
                                BC_StartGame();
                                break;
                        }
                        break;

                    case "PC":
                        switch ((sender as Button).Content.ToString())
                        {
                            case "Installieren":
                                PC_InstallGame();
                                break;

                            case "Update":
                                PC_UpdateGame();
                                break;

                            case "Spielen":
                                PC_StartGame();
                                break;
                        }
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
                if (Directory.Exists("Launcher/BlockyCrafters"))
                {
                    try
                    {
                        BC_ClientState(3);
                        Directory.Delete("Launcher/BlockyCrafters", true);

                        Dispatcher.Invoke(() =>
                        {
                            BC_ClientState(0);
                            MessageBox.Show("Modpack deleted!");
                            CurrVerLabel_BC.Content = "Current Version: " + "No Version";
                        });

                    }
                    catch (Exception)
                    {
                        GetError("Failed to delete modpack!");
                        BC_ClientState(4);
                    }
                }
                else
                {
                    GetError(
                        "Failed to delete modpack!\n" +
                        "Modpack doesn't exist.");
                    BC_ClientState(4);
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
