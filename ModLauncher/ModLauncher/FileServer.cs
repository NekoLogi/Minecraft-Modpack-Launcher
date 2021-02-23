using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ModLauncher
{
    static class FileServer
    {
        public static string[] info = new string[2];
        public static bool isConnected = true;
        static string lastInput;
        static string lastError;
        static Socket socket;

        public static void Connect(int index) {
            try {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("136.243.216.73"), 20);
                socket.Connect(endPoint);
                HandleData(index);

            } catch (Exception e) {
                if (lastError != e.Message || lastError == null) {
                    lastError = $"Connection failed: {e.Message}";
                    MainWindow main = new MainWindow();
                    main.GetError($"Connection failed: {e.Message}");
                }
                isConnected = false;
            }
            socket.Close();
        }

        static void HandleData(int index) {
            try {
                string[] input = { "VER", "PAT", "NEW", "UPD" };

                byte[] data = Encoding.ASCII.GetBytes(lastInput = input[index]);
                socket.Send(data);

                switch (lastInput) {
                    case "VER":
                        GetVersion();
                        break;
                    case "PAT":
                        GetPatch();
                        break;
                    case "NEW":
                        GetModpack();
                        break;
                    case "UPD":
                        GetUpdate();
                        break;

                    default:
                        break;
                }

            } catch (Exception e) {
                Console.WriteLine("Error: {0}", e.Message);
                socket.Close();
            }
            Console.ReadLine();
        }

        static byte[] ReceiveData() {
            byte[] buffer = new byte[socket.SendBufferSize];
            int bytesRead = socket.Receive(buffer);
            byte[] formatted = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++) {
                formatted[i] = buffer[i];
            }
            return formatted;
        }

        static void GetVersion() {
            byte[] formatted = ReceiveData();
            string data = Encoding.ASCII.GetString(formatted);
            info[0] = data;
        }

        static void GetPatch() {
            byte[] formatted = ReceiveData();
            string data = Encoding.ASCII.GetString(formatted);
            info[1] = data;
        }

        static void GetModpack() {
            if (File.Exists("Cache/blockycrafters.zip")) {
                File.Delete("Cache/blockycrafters.zip");
            }
            if (Directory.Exists("BlockyCrafters")) {
                Directory.Delete("BlockyCrafters", true);
            }
            DownloadFile(0);
            FileManager.ModInstall();
        }

        static void GetUpdate() {
            DownloadFile(1);
            FileManager.ModUpdate();
        }

        static void DownloadFile(int index) {
            string[] fileName = { "blockycrafters.zip", "blockycrafters_update.zip" };

            if (!File.Exists("Cache/" + fileName[index])) {
                byte[] formatted = ReceiveData();
                while (formatted.Length != 0) {
                    using (var stream = new FileStream("Cache/" + fileName[index], FileMode.Append)) {
                        stream.Write(formatted, 0, formatted.Length);
                    }
                    formatted = ReceiveData();
                }
            } else {
                Console.WriteLine("{0} already exists!", fileName[index]);
            }
        }
    }
}
