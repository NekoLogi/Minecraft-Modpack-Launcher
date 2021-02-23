using System;
using CmlLib.Core;
using CmlLib.Core.Auth;
using System.IO;

namespace ModLauncher
{
    static class MC_Game {
        static MainWindow main = new MainWindow();

        static string forgeVersion;
        static MSession session;

        public static void Login() {
            var login = new MLogin();
            string[] userData = File.ReadAllText("BlockyCrafters/Login.txt").Split(' ');
            var response = login.Authenticate(userData[0], userData[1]);

            if (!response.IsSuccess) {
                MCClient_Login mcClient = new MCClient_Login();
                mcClient.Show();
                mcClient.GetError($"Login Failed!\n" +
                        "E-Mail or Password invalid!");

            } else {
                session = response.Session;
                LaunchGame();
            }

        }

        static MSession OffLogin() {
            return MSession.GetOfflineSession("");
        }

        public static void LaunchGame() {
            try {
                var path = new MinecraftPath(Directory.GetCurrentDirectory() + @"\BlockyCrafters");
                var launcher = new CMLauncher(path);

                // more options : https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption
                var launchOption = new MLaunchOption {
                    MaximumRamMb = 6144,
                    Session = session
                };

                forgeVersion = File.ReadAllText("BlockyCrafters/Forge Version.txt");
                var process = launcher.CreateProcess(forgeVersion, launchOption);
                process.Start();
            } catch (Exception) {
                main.GetError("Failed Launching Game!");
            }
        }
    }
}
