using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DatabaseFormat;
using System.IO;
using FRESHMusicPlayer.Forms;
namespace FRESHMusicPlayer.Handlers
{
    static class DatabaseHandler
    {
        public static List<string> ReadSongs()
        {
            if (!File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json"))
            {
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer");
                File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json", @"{""Version"":1,""Songs"":[]}");
            }
            using (StreamReader file = File.OpenText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json")) // Read json file
            {
                JsonSerializer serializer = new JsonSerializer();
                Format database = (Format)serializer.Deserialize(file, typeof(Format));
                return database.Songs;
            }
        }
        public static void ImportSong(string filepath)
        {
            List<string> ExistingSongs = new List<string>();

            List<string> database = ReadSongs();
            ExistingSongs = database; // Add the existing songs to a list to use later

            ExistingSongs.Add(filepath); // Add the new song in
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;

            using (StreamWriter file = File.CreateText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void DeleteSong(string filepath)
        {
            List<string> database = ReadSongs();
            database.Remove(filepath);
            Format format = new Format();
            format.Version = 1;
            format.Songs = database;
            

            using (StreamWriter file = File.CreateText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }
        }
        public static void ClearLibrary()
        {
            if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json"))
            {
                File.Delete($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json");
                File.WriteAllText("database.json", @"{""Version"":1,""Songs"":[]}");
            }
        }
    }

}
