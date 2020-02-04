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
        public static readonly int DatabaseVersion = 1;
        public static readonly string DatabasePath;
        static DatabaseHandler()
        {
            DatabasePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\FRESHMusicPlayer\\database.json";
        }
        /// <summary>
        /// Returns all of the tracks in the database.
        /// </summary>
        /// <returns>A list of file paths in the database.</returns>
        public static List<string> ReadSongs()
        {
            if (!File.Exists(DatabasePath))
            {
                Directory.CreateDirectory(DatabasePath);
                File.WriteAllText(DatabasePath, $"{{\"Version\":{DatabaseVersion},\"Songs\":[]}}");
            }
            using (StreamReader file = File.OpenText(DatabasePath)) // Read json file
            {
                JsonSerializer serializer = new JsonSerializer();
                Format database = (Format)serializer.Deserialize(file, typeof(Format));
                return database.Songs;
            }
        }

        public static void ImportSong(string filepath)
        {
            List<string> ExistingSongs;

            List<string> database = ReadSongs();
            ExistingSongs = database; // Add the existing songs to a list to use later

            ExistingSongs.Add(filepath); // Add the new song in
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;

            using (StreamWriter file = File.CreateText(DatabasePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(string[] filepath)
        {
            List<string> ExistingSongs;

            List<string> database = ReadSongs();
            ExistingSongs = database; // Add the existing songs to a list to use later

            ExistingSongs.AddRange(filepath);
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;

            using (StreamWriter file = File.CreateText(DatabasePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(List<string> filepath)
        {
            List<string> ExistingSongs;

            List<string> database = ReadSongs();
            ExistingSongs = database; // Add the existing songs to a list to use later

            ExistingSongs.AddRange(filepath);
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;

            using (StreamWriter file = File.CreateText(DatabasePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        public static void ImportSong(IList<string> filepath)
        {
            List<string> ExistingSongs;

            List<string> database = ReadSongs();
            ExistingSongs = database; // Add the existing songs to a list to use later

            ExistingSongs.AddRange(filepath);
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;

            using (StreamWriter file = File.CreateText(DatabasePath))
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
            

            using (StreamWriter file = File.CreateText(DatabasePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }
        }
        public static void ClearLibrary()
        {
            if (File.Exists(DatabasePath))
            {
                File.Delete(DatabasePath);
                File.WriteAllText("database.json", @"{""Version"":1,""Songs"":[]}");
            }
        }
    }

}
