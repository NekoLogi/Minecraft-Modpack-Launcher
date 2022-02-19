using System.IO;
using System.IO.Compression;

namespace ModLauncher
{
    static class FileManager
    {
        public enum Modpacks
        {
            BlockyCrafters,
            ProjectCenturos
        }

        public static string MOD_VERSION { get; set; }

        public static void ModInstall(Modpacks modpack)
        {
            if (File.Exists($"Cache/{modpack.ToString().ToLower()}.zip"))
            {
                string zipPath = $"Cache/{modpack.ToString().ToLower()}.zip";
                string extractpath = $"Launcher/{modpack}";

                ZipFile.ExtractToDirectory(zipPath, extractpath);

                File.Delete($"Cache/{modpack.ToString().ToLower()}.zip");
                CheckVersion(modpack);
            }
        }

        public static void ModUpdate(Modpacks modpack)
        {
            if (File.Exists($"Cache/{modpack.ToString().ToLower()}_update.zip"))
            {
                string zipPath = $"Cache/{modpack.ToString().ToLower()}_update.zip";
                string extractpath = $"Launcher/{modpack}";
                string cache = $"Cache/{modpack.ToString().ToLower()}";

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
                CheckVersion(modpack);
            }
        }

        public static bool CheckVersion(Modpacks modpack)
        {
            if (File.Exists($"Launcher/{modpack}/Version.txt"))
            {
                MOD_VERSION = File.ReadAllText($"Launcher/{modpack}/Version.txt");
                
                if (FileServer.isConnected)
                {
                    if (MOD_VERSION == FileServer.info[0])
                    {
                        return true;
                    }
                }
            }
            else
            {
                MOD_VERSION = "No Version";
            }
            return false;
        }
    }
}