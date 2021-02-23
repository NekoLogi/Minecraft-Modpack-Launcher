using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System.IO;

namespace ModLauncher
{
    static class FileManager
    {
        public static string MOD_VERSION { get; set; }

        public static void ModInstall() {
            if (File.Exists("Cache/blockycrafters.zip")) {
                string zipPath = "Cache/blockycrafters.zip";
                string extractpath = "BlockyCrafters";
                ZipFile.ExtractToDirectory(zipPath, extractpath);

                File.Delete("Cache/blockycrafters.zip");
                CheckVersion();
            }
        }

        public static void ModUpdate() {
            if (File.Exists("Cache/blockycrafters_update.zip")) {
                string zipPath = "Cache/blockycrafters_update.zip";
                string extractpath = "BlockyCrafters";
                ZipFile.ExtractToDirectory(zipPath, extractpath);

                File.Delete("Cache/blockycrafters_update.zip");
                CheckVersion();
            }
        }

        public static bool CheckVersion() {
            if (File.Exists("BlockyCrafters/Version.txt")) {

                MOD_VERSION = File.ReadAllText("BlockyCrafters/Version.txt");
                if (FileServer.isConnected) {
                    if (MOD_VERSION == FileServer.info[0].Split(' ')[2]) {
                        return true;
                    }
                }
            } else {
                MOD_VERSION = "No Version";
            }
            return false;
        }
    }
}