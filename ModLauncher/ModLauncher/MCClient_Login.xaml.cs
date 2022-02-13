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
using System.IO;

namespace ModLauncher
{
    public partial class MCClient_Login : Window
    {
        public static MainWindow main;

        public MCClient_Login() {
            InitializeComponent();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        public void GetError(string error) {
            MessageBox.Show(error);
        }

        private void Login_Btn_Click(object sender, RoutedEventArgs e) {
            if (EMail_Box.Text.Length > 1 && Password_Box.Password.Length > 1) {
                File.WriteAllText("BlockyCrafters/Login.txt", EMail_Box.Text + " " + Password_Box.Password);

                Dispatcher.Invoke(() =>
                {
                    main.Delete_Btn.IsEnabled = true;
                    main.Start_Btn.IsEnabled = true;
                });

                Close();
                MC_Game.Login();
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
                main.Delete_Btn.IsEnabled = true;
                main.Start_Btn.IsEnabled = true;
            });
            Close();
        }

        private void XBoxLogin_Btn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            Dispatcher.Invoke(() =>
            {
                main.Delete_Btn.IsEnabled = true;
                main.Start_Btn.IsEnabled = true;
            });
            MC_Game.XBoxLogin();
            Close();
        }
    }
}
