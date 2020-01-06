using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DatabaseFormat;
using System.IO;

namespace FRESHMusicPlayer.Handlers
{
    static class DatabaseHandler
    {
        public static List<string> ReadSongs()
        {
            if (!File.Exists("database.json"))
            {
                File.WriteAllText("database.json", @"{""Version"":1,""Songs"":[]}");
            }
            using (StreamReader file = File.OpenText("database.json")) // Read json file
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

            using (StreamWriter file = File.CreateText("database.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ClearLibrary()
        {
            if (File.Exists("database.json"))
            {
                File.Delete("database.json");
                File.WriteAllText("database.json", @"{""Version"":1,""Songs"":[]}");
            }
        }
    }

}
