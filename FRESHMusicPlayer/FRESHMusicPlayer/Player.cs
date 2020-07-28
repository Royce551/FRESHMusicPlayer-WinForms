﻿using DiscordRPC;
using NAudio.Wave;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRESHMusicPlayer.Handlers;
using FRESHMusicPlayer.Utilities;
using System.Net.Http;
namespace FRESHMusicPlayer
{
    public partial class Player : Form
    {
        private static WaveOutEvent outputDevice;
        public static AudioFileReader audioFile;
        public static bool avoidnextqueue = false;
        public static DiscordRpcClient client;

        //public static int position;
        public static float currentvolume = 1;
        public static string filePath = "";
        public static bool playing = false;
        public static bool paused = false;

        public static bool RepeatOnce = false;
        public static bool Shuffle = false;

        private static List<string> Queue = new List<string>();
        public static int QueuePosition = 0;

        public static DateTime lastUpdateCheck;
        /// <summary>
        /// Raised whenever a new track is being played.
        /// </summary>
        public static event EventHandler songChanged;
        public static event EventHandler songStopped;
        public static event EventHandler<PlaybackExceptionEventArgs> songException;
        public Player()
        {
            InitializeComponent();
            UserInterface userInterface = new UserInterface();  // Show your UI form here.
            userInterface.Show();                               // Note: The UI form must close the application when it closes, else the program will stay opened.
        }
        #region CoreFMP
        // Queue System
        /// <summary>
        /// Adds a track to the <see cref="queue"/>.
        /// </summary>
        /// <param name="filePath">The file path to the track to add.</param>
        public static void AddQueue(string filePath)
        {
            Queue.Add(filePath);
        }
        public static void AddQueue(string[] filePaths)
        {
            Queue.AddRange(filePaths);
        }
        public static void ClearQueue() => Queue.Clear();
        public static List<string> GetQueue()
        {
            return Queue;
        }
        /// <summary>
        /// Skips to the previous track in the queue. If there are no tracks for the player to go back to, nothing will happen.
        /// </summary>
        public static void PreviousSong()
        {
            if (QueuePosition <= 1) return;
            if (Shuffle) Queue = PlayerUtils.ShuffleQueue(Queue);
            QueuePosition -= 2;
            PlayMusic();
        }
        /// <summary>
        /// Skips to the next track in the queue. If there are no more tracks, the player will stop.
        /// </summary>
        /// <param name="avoidnext">Intended to be used only by the player</param>
        public static void NextSong(bool avoidnext=false)
        {
            avoidnextqueue = avoidnext;
            if (QueuePosition >= Queue.Count)
            {
                Queue.Clear();
                QueuePosition = 0;
                StopMusic();
                return;
            }
            if (RepeatOnce) QueuePosition--; // Don't advance queue, play the same thing again
            if (Shuffle) Queue = PlayerUtils.ShuffleQueue(Queue);
            PlayMusic();    
        }
        // Music Playing Controls
        private static void OnPlaybackStopped(object sender, StoppedEventArgs args)
        {
            if (!avoidnextqueue) NextSong(false);
            else
            {
                avoidnextqueue = false;
            }
        }
        /// <summary>
        /// Repositions the playback position of the player.
        /// </summary>
        /// <param name="seconds">The position in to the track to skip in, in seconds.</param>
        public static void RepositionMusic(int seconds)
        {
            audioFile.CurrentTime = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Starts playing the queue. In order to play a track, you must first add it to the queue using <see cref="AddQueue(string)"/>.
        /// </summary>
        /// <param name="repeat">If true, avoids dequeuing the next track. Not to be used for anything other than the player.</param>
        public static void PlayMusic(bool repeat=false)
        {           
            if (!repeat && Queue.Count != 0) filePath = Queue[QueuePosition]; // Some functions want to play the same song again
            QueuePosition++;
            void PMusic()
            {
                if (outputDevice == null)
                {
                    outputDevice = new WaveOutEvent();
                    outputDevice.PlaybackStopped += OnPlaybackStopped;
                }
                if (audioFile == null)
                {
                    audioFile = new AudioFileReader(filePath);
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
                    avoidnextqueue = true;
                    StopMusic();
                    PMusic();
                }
                songChanged?.Invoke(null, EventArgs.Empty); // Now that playback has started without any issues, fire the song changed event.
            }
            catch (System.IO.FileNotFoundException)
            {
                PlaybackExceptionEventArgs args = new PlaybackExceptionEventArgs();
                args.Details = "That's not a valid file path!";
                songException.Invoke(null, args);
            }
            catch (System.ArgumentException)
            {
                PlaybackExceptionEventArgs args = new PlaybackExceptionEventArgs();
                args.Details = "That's not a valid file path!";
                songException.Invoke(null, args);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                PlaybackExceptionEventArgs args = new PlaybackExceptionEventArgs();
                args.Details = "This isn't a valid audio file!";
                songException.Invoke(null, args);
            }
            catch (System.FormatException)
            {
                PlaybackExceptionEventArgs args = new PlaybackExceptionEventArgs();
                args.Details = "This audio file might be corrupt!";
                songException.Invoke(null, args);
            }
            catch (System.InvalidOperationException)
            {
                PlaybackExceptionEventArgs args = new PlaybackExceptionEventArgs();
                args.Details = "This audio file uses VBR \nor might be corrupt!";
                songException.Invoke(null, args);
            }

            
        }
        /// <summary>
        /// Completely stops and disposes the player and resets all playback related variables to their defaults.
        /// </summary>
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
                    //position = 0;
                    songStopped?.Invoke(null, EventArgs.Empty);
                    if (Properties.Settings.Default.General_DiscordIntegration)
                    {
                        UpdateRPC("idle", "Nobody", "Idle");
                    }
                }
                catch (NAudio.MmException)  // This is an old workaround from the original FMP days. Shouldn't be needed anymore, but is kept anyway for the sake of
                {                           // stability.
                    Console.WriteLine("Things are breaking!");
                    Console.WriteLine(filePath);
                    outputDevice?.Dispose();
                    outputDevice = new WaveOutEvent();
                    outputDevice.PlaybackStopped += OnPlaybackStopped; // Does the same initiallization PlayMusic() does.
                    audioFile = new AudioFileReader(filePath);
                    outputDevice.Init(audioFile);
                    PlayMusic(true);
                }
            
        }
        /// <summary>
        /// Pauses playback without disposing. Can later be resumed with <see cref="ResumeMusic()"/>.
        /// </summary>
        public static void PauseMusic()
        {
            if (!paused) outputDevice?.Pause();
            //playing = false;
            paused = true;
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                UpdateRPC("pause", "Nobody", "Paused");
            }
        }// Pauses the music without completely disposing it
        /// <summary>
        /// Resumes playback.
        /// </summary>
        public static void ResumeMusic()
        {
            if (paused) outputDevice?.Play();
            //playing = true;
            paused = false;
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                ATL.Track metadata = new ATL.Track(filePath);
                UpdateRPC("play", metadata.Artist, metadata.Title);
            }
        }// Resumes music that has been paused
        /// <summary>
        /// Updates the volume of the player during playback to the value of <see cref="currentvolume"/>.
        /// Even if you don't call this, the volume of the player will update whenever the next track plays.
        /// </summary>
        public static void UpdateSettings()
        {
            outputDevice.Volume = currentvolume;
        }
        // Other Logic Stuff
        /// <summary>
        /// Returns a formatted string of the current playback position.
        /// </summary>
        /// <returns></returns>
        public static string getSongPosition()
        {
            //if (playing) // Only work if music is currently playing
            //if (!positiononly) position += 1; // Only tick up the position if it's being called from UserInterface
            
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
            
            return $"{Format((int)audioFile.CurrentTime.TotalSeconds)} / {Format((int)length.TotalSeconds)}";
        }
        #endregion
        // Integration
        #region DiscordRPC
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
        public static void UpdateRPC(string Activity, string Artist = null, string Title = null)
        {
            client?.SetPresence(new RichPresence()
            {
                Details = Title,
                State = $"by {Artist}",
                Assets = new Assets()
                {
                    LargeImageKey = "icon",
                    SmallImageKey = Activity
                },
                Timestamps = Timestamps.Now
            }
            );

        }
        public static void DisposeRPC() => client?.Dispose();
        #endregion
        #region Squirrel
        public static async Task UpdateIfAvailable()
        {
            
            updateInProgress = RealUpdateIfAvailable();
            await updateInProgress;
        }

        public static async Task WaitForUpdatesOnShutdown()
        {
            // We don't actually care about errors here, only completion
            await updateInProgress.ContinueWith(ex => { });
        }

        public static Task updateInProgress = Task.FromResult(true);
        private static async Task RealUpdateIfAvailable(bool useDeltaPatching = true)
        {
            Properties.Settings.Default.General_LastUpdate = DateTime.Now;
            Properties.Settings.Default.Save();
            var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/Royce551/FRESHMusicPlayer", prerelease:Properties.Settings.Default.General_PreRelease);
            try
            {
                UpdateInfo updateInfo = await mgr.CheckForUpdate(!useDeltaPatching);
                if (updateInfo.CurrentlyInstalledVersion == null) return; // Standalone version of FMP, don't bother
                if (updateInfo.ReleasesToApply.Count == 0) return; // No updates to apply, don't bother
                DialogResult dialogResult = MessageBox.Show("A new update for FMP is available. Do you want to update now?", "Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    await mgr.DownloadReleases(updateInfo.ReleasesToApply);
                    await mgr.ApplyReleases(updateInfo);
                    ShutdownTheApp();
                }
                else return; 
            }
            catch (Exception e)
            { 
                if (useDeltaPatching)
                {
                    await RealUpdateIfAvailable(false);
                }
                else MessageBox.Show($"An error occured during the update process \n (Info - {e.Message}). \n If you are using a pre-release version, no need to worry. If not, let me know.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                mgr.Dispose();        
            }
        }
        

        /// Now, in your shutdown code

        public static async void ShutdownTheApp()
        {

            await WaitForUpdatesOnShutdown();

            Application.Exit();
        }
        #endregion
        private void Player_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
