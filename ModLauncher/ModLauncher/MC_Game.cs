using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ModLauncher
{
    static class MC_Game
    {
        static MainWindow main = new MainWindow();
        static string forgeVersion;
        static MSession SESSION;

        public static void Login(FileManager.Modpacks modpack)
        {
            var login = new MLogin();
            string[] userData = File.ReadAllText("Launcher/BlockyCrafters/Login.txt").Split(' ');
            var response = login.Authenticate(userData[0], userData[1]);

            if (!response.IsSuccess)
            {
                MCClient_Login mcClient = new MCClient_Login();
                mcClient.Show();
                mcClient.GetError($"Login Failed!\n" +
                        "E-Mail or Password invalid!");
            }
            else
            {
                SESSION = response.Session;
                _ = LaunchGameAsync(modpack);
            }
        }

        public static void XBoxLogin(FileManager.Modpacks modpack)
        {
            MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
            MSession session = loginWindow.ShowLoginDialog();
            if (session != null)
            {
                SESSION = session;
                _ = LaunchGameAsync(modpack);
            }
        }

        public static void XBoxLogout()
        {
            MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
            loginWindow.ShowLogoutDialog();
        }

        static MSession OffLogin()
        {
            return MSession.GetOfflineSession("");
        }

        public static async Task LaunchGameAsync(FileManager.Modpacks modpack)
        {
            try
            {
                var path = new MinecraftPath(Directory.GetCurrentDirectory() + $@"\Launcher\{modpack}");
                var launcher = new CMLauncher(path);


                // more options : https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = Settings.RAM,
                    Session = SESSION
                };

                forgeVersion = File.ReadAllText($"Launcher/{modpack}/Forge Version.txt");
                var process = await launcher.CreateProcessAsync(forgeVersion, launchOption);
                process.Start();
            }
            catch (Exception)
            {
                main.GetError($"Failed Launching {modpack}!");
            }
        }
    }
}
