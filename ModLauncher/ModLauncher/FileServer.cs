﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ModLauncher
{
    static class FileServer
    {
        public static string[] info = new string[2];
        public static bool isConnected = true;
        static string[] lastInput;
        static string lastError;
        static Socket socket;

        public static void Connect(int index, FileManager.Modpacks modpack)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(Settings.IP_ADDRESS), Settings.PORT);
                socket.Connect(endPoint);
                HandleData(index, modpack);

            }
            catch (Exception e)
            {
                if (lastError != e.Message || lastError == null)
                {
                    lastError = $"Connection failed: {e.Message}";
                    MainWindow.CurrentWindow.GetError($"Connection failed: {e.Message}");
                }
                isConnected = false;
            }
            socket.Close();
        }

        static void HandleData(int index, FileManager.Modpacks modpack)
        {
            try
            {
                string[] input = { "VER", "PAT", "NEW", "UPD" };

                lastInput = (input[index] + "_" + (int)modpack).Split('_');
                byte[] data = Encoding.ASCII.GetBytes(input[index] + "_" + (int)modpack);

                socket.Send(data);

                switch (lastInput[0])
                {
                    case "VER":
                        GetVersion();
                        break;
                    case "PAT":
                        GetPatch();
                        break;
                    case "NEW":
                        GetModpack(modpack);
                        break;
                    case "UPD":
                        GetUpdate(modpack);
                        break;

                    default:
                        break;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                socket.Close();
            }
        }

        static byte[] ReceiveData()
        {
            byte[] buffer = new byte[socket.SendBufferSize];
            int bytesRead = socket.Receive(buffer);
            byte[] formatted = new byte[bytesRead];

            for (int i = 0; i < bytesRead; i++)
            {
                formatted[i] = buffer[i];
            }
            return formatted;
        }

        static void GetVersion()
        {
            byte[] formatted = ReceiveData();
            string data = Encoding.ASCII.GetString(formatted);
            info[0] = data;
        }

        static void GetPatch()
        {
            byte[] formatted = ReceiveData();
            string data = Encoding.ASCII.GetString(formatted);
            info[1] = data;
        }

        static void GetModpack(FileManager.Modpacks modpack)
        {
            if (File.Exists($"Cache/{modpack.ToString().ToLower()}.zip"))
            {
                File.Delete($"Cache/{modpack.ToString().ToLower()}.zip");
            }
            if (Directory.Exists($"Launcher/{modpack}"))
            {
                Directory.Delete($"Launcher/{modpack}", true);
            }
            DownloadFile(0, modpack);
            FileManager.ModInstall(modpack);
        }

        static void GetUpdate(FileManager.Modpacks modpack)
        {
            DownloadFile(1, modpack);
            FileManager.ModUpdate(modpack);
        }

        static void DownloadFile(int index, FileManager.Modpacks modpack)
        {
            string[] fileName = { $"{modpack.ToString().ToLower()}.zip", $"{modpack.ToString().ToLower()}_update.zip" };


            if (!File.Exists("Cache/" + fileName[index]))
            {
                byte[] formatted = ReceiveData();
                while (formatted.Length != 0)
                {
                    using (var stream = new FileStream("Cache/" + fileName[index], FileMode.Append))
                    {
                        stream.Write(formatted, 0, formatted.Length);
                    }
                    formatted = ReceiveData();
                }
            }
            else
            {
                Console.WriteLine("{0} already exists!", fileName[index]);
            }
        }
    }
}
