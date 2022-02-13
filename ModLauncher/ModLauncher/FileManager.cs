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
                string cache = "Cache/BlockyCrafters";

                ZipFile.ExtractToDirectory(zipPath, cache);

                foreach (var cacheFile in Directory.GetFiles(cache))
                {
                    var cacheString = cacheFile.Split('\\');
                    foreach (var updateFile in Directory.GetFiles(extractpath))
                    {
                        var updateString = updateFile.Split('\\');
                        if (cacheString[cacheString.Length - 1] == updateString[updateString.Length - 1])
                        {
                            File.Delete($"{extractpath}/{cacheString[cacheString.Length - 1]}");
                        }
                    }
                }
                foreach (var cacheDir in Directory.GetDirectories(cache))
                {
                    var cacheGetDir = cacheDir.Split('\\');
                    foreach (var updateDir in Directory.GetDirectories(extractpath))
                    {
                        var updateGetDir = updateDir.Split('\\');
                        if (cacheGetDir[cacheGetDir.Length - 1] == updateGetDir[updateGetDir.Length - 1])
                        {
                            Directory.Delete($"{extractpath}/{cacheGetDir[cacheGetDir.Length - 1]}", true);
                        }
                    }
                }
                Directory.Delete(cache, true);
                ZipFile.ExtractToDirectory(zipPath, extractpath);

                File.Delete(zipPath);
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