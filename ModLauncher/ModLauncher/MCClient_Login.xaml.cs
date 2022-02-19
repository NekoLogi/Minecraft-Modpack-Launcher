using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ModLauncher
{
    public partial class MCClient_Login : Window
    {
        public static MainWindow main;
        public static int MODPACK { get; set; }

        public MCClient_Login()
        {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public void GetError(string error)
        {
            MessageBox.Show(error);
        }

        private void Login_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (EMail_Box.Text.Length > 1 && Password_Box.Password.Length > 1)
            {
                File.WriteAllText("Launcher/BlockyCrafters/Login.txt", EMail_Box.Text + " " + Password_Box.Password);

                Dispatcher.Invoke(() =>
                {
                    main.DeleteBtn_BC.IsEnabled = true;
                    main.StartBtn_BC.IsEnabled = true;
                });

                Close();
                MC_Game.Login((FileManager.Modpacks)MODPACK);
            }
            else
            {
                MessageBox.Show("Email or Password is invalid!");
            }
        }

        private void LoginClose_Btn_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                main.DeleteBtn_BC.IsEnabled = true;
                main.StartBtn_BC.IsEnabled = true;
            });
            Close();
        }

        private void XBoxLogin_Btn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Dispatcher.Invoke(() =>
            {
                main.DeleteBtn_BC.IsEnabled = true;
                main.StartBtn_BC.IsEnabled = true;
            });
            MC_Game.XBoxLogin((FileManager.Modpacks)MODPACK);
            Close();
        }
    }
}
