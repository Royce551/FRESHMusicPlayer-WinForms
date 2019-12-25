using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using ATL.Playlist;
using DiscordRPC;
namespace FRESHMusicPlayer
{
    public partial class Player : Form
    {
        public static string filePath = ""; //Path used globally for metadata functions
        public static string path = ""; // Path used by PlayMusic
        public static string fileContent = "";
        public static bool playing = false;
        public static bool paused = false;
        private static WaveOutEvent outputDevice;
        private static AudioFileReader audioFile;
        public static int position;
        public static float currentvolume = 1;
        static Queue<string> queue = new Queue<string>();
        public static bool songchanged = false;
        public static bool avoidnextqueue = false;
        public static DiscordRpcClient client;

        public static event EventHandler songChanged;
        public Player()
        {
            InitializeComponent();
            UserInterface userInterface = new UserInterface();
            userInterface.Show();

        }
        // Interaction with other forms
        public static (string Artist, string Title) GetMetadata()
        {
            ATL.Track theTrack = new ATL.Track(filePath);
            return (theTrack.Artist, theTrack.Title);
        }
        
        // Queue System
        public static void AddQueue(string filePath) => queue.Enqueue(filePath);
        public static void ClearQueue() => queue.Clear();
        public static Queue<string> GetQueue()
        {
            Queue<string> x = queue;
            return x;
        }
        public static void NextQueue()
        {
            // If there are no more songs left
            if (queue.Count == 0) StopMusic(); // Acts the same way as the old system worked
            else PlayMusic();
        }
        public static void NextSong()
        {
            if (queue.Count == 0) StopMusic(); // Acts the same way as the old system worked
            else
            {
                avoidnextqueue = true;
                PlayMusic();
            }
        }
        // Music Playing Controls
        private static void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            if (!avoidnextqueue) NextQueue();
            else avoidnextqueue = false;
        }
        public static string PlayMusic(bool repeat=false)
        {
            if (!repeat) path = queue.Dequeue(); // Some functions want to play the same song again
            filePath = path; // This is necessary for the metadata operations everything in the program does
            songChanged?.Invoke(null, EventArgs.Empty); // Tell all other forms subscribed to this event that a new song has started playing
            void PMusic()
            {
                
                if (outputDevice == null)
                {
                    outputDevice = new WaveOutEvent();
                    outputDevice.PlaybackStopped += OnPlaybackStopped;
                }
                if (audioFile == null)
                {

                    audioFile = new AudioFileReader(path);
                    outputDevice.Init(audioFile);
                }
                outputDevice.Play();
                outputDevice.Volume = currentvolume;
                playing = true;
            }
            try
            {
                if (playing != true)
                {
                    PMusic();
                }
                else
                {
                    StopMusic();
                    PMusic();
                }
            }
            catch (System.IO.FileNotFoundException)
            {

                MessageBox.Show("Onee-Chan~~! That's not a valid file path, you BAKA! (or it's not a supported file type!)", "Incorrect file path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Onee-Chan~~! You BAKA! You're supposed to actually put something in the box!", "Nothing typed in file path box", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                MessageBox.Show("Onee-Chan~! That's not a valid audio file!", "File Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.FormatException)
            {
                MessageBox.Show("Onee-Chan~! This audio file must be corrupt! I can't play it!", "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (System.InvalidOperationException)
            {
                MessageBox.Show("Onee-Chan~! FRESHMusicPlayer doesn't support fancy VBR audio files! (or your audio file is corrupt in some way)", "VBR Files Not Supported", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return $"";
        }
        public static void StopMusic()
        {
            if (playing)
                try
                {
                    outputDevice.Dispose();
                    outputDevice = null;
                    audioFile?.Dispose();
                    audioFile = null;
                    playing = false;
                    paused = false;
                    position = 0;
                    
                }
                catch (System.NullReferenceException)
                {
                    //PlayMusic(filePath);

                }


                catch (NAudio.MmException)
                {
                    Console.WriteLine("Things are breaking!");
                    Console.WriteLine(filePath);
                    //MessageBox.Show("ok", "Format Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    outputDevice?.Dispose();
                    outputDevice = new WaveOutEvent();
                    outputDevice.PlaybackStopped += OnPlaybackStopped; // Does the same initiallization PlayMusic() does.
                    audioFile = new AudioFileReader(filePath);
                    outputDevice.Init(audioFile);
                    PlayMusic(true);
                }
            //else PlayMusic(true);
        }
        public static void PauseMusic()
        {
            if (!paused) outputDevice?.Pause();
            //playing = false;
            paused = true;
        }// Pauses the music without completely disposing it
        public static void ResumeMusic()
        {
            if (paused) outputDevice?.Play();
            //playing = true;
            paused = false;
        }// Resumes music that has been paused
        public static void UpdateSettings()
        {
            outputDevice.Volume = currentvolume;
        }
        // Other Logic Stuff
        public static string getSongPosition(bool positiononly=false)
        {
            if (playing) // Only work if music is currently playing
            if (!positiononly) position += 1; // Only tick up the position if it's being called from UserInterface
            
            string Format(int secs)
            {
                int hours = 0;
                int mins = 0;

                while (secs >= 60)
                {
                    mins++;
                    secs -= 60;
                }

                while (mins >= 60)
                {
                    hours++;
                    mins -= 60;
                }

                string hourStr = hours.ToString(); if (hourStr.Length < 2) hourStr = "0" + hourStr;
                string minStr = mins.ToString(); if (minStr.Length < 2) minStr = "0" + minStr;
                string secStr = secs.ToString(); if (secStr.Length < 2) secStr = "0" + secStr;

                string durStr = "";
                if (hourStr != "00") durStr += hourStr + ":";
                durStr = minStr + ":" + secStr;

                return durStr;
            }

            //ATL.Track theTrack = new ATL.Track(filePath);
            var length = audioFile.TotalTime;
            
            return $"{Format(position)} / {Format((int)length.TotalSeconds)}";
        }
        // Integration
        public static void InitDiscordRPC()
        {
            /*
                Create a discord client
                NOTE: 	If you are using Unity3D, you must use the full constructor and define
                         the pipe connection.
                */
            client = new DiscordRpcClient("656678380283887626");

            //Set the logger
            //client.Logger = new ConsoleLogger() { Level = Discord.LogLevel.Warning };

            //Subscribe to events
            client.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            client.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            //Connect to the RPC
            client.Initialize();

            //Set the rich presence
            //Call this as many times as you want and anywhere in your code.
        }
        public static void UpdateRPC(string Activity, string Song)
        {
            client.SetPresence(new RichPresence()
            {
                Details = Song,
                State = Activity,
            }
            );

        }
        public static void DisposeRPC() => client.Dispose();
    }
}
