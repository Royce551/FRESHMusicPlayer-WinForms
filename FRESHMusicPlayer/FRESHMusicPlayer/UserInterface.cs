﻿using ATL.Playlist;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using DatabaseFormat;
namespace FRESHMusicPlayer
{
    public partial class UserInterface : Form
    {
        public static bool MiniPlayerUpdate = false;
        public UserInterface()
        {
            InitializeComponent();
            ApplySettings();
            SetCheckBoxes();
        }
        // Because closing UserInterface doesn't close the main fore and therefore the application, 
        // this function does that job for us :)
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.General_Volume = Player.currentvolume;
            Properties.Settings.Default.Save();
            Application.Exit();
        }
 // Communication with other forms
        private void UpdateGUI()
        {
            titleLabel.Text = "Nothing Playing";
            Text = "FRESHMusicPlayer";
            progressIndicator.Text = "(nothing playing)";
            Player.position = 0;
        }
// BUTTONS
        private void browsemusicButton_Click(object sender, EventArgs e)
        {
            using (var selectFileDialog = new OpenFileDialog())

            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {

                    //Player.filePath = selectFileDialog.FileName;
                    Player.AddQueue(selectFileDialog.FileName);
                    Player.PlayMusic();
                    var metadata = Player.GetMetadata();
                    titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
                    Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
                    Player.playing = true;
                    getAlbumArt();
                }
                
            }
        }

        private void pauseplayButton_Click(object sender, EventArgs e)
        {
            if (!Player.paused)
            {
                
                pauseplayButton.Image = Properties.Resources.baseline_play_arrow_black_18dp;
                Player.PauseMusic();
            }
            else
            {
                
                pauseplayButton.Image = Properties.Resources.baseline_pause_black_18dp;
                Player.ResumeMusic();
            }
        }
        private void stopButton_Click(object sender, EventArgs e)
        {
            Player.ClearQueue();
            Player.StopMusic();
            
        }
        private void volumeBar_Scroll(object sender, EventArgs e)
        {
            Player.currentvolume = (float)volumeBar.Value / 100.0f;
            if (Player.playing) Player.UpdateSettings();
            //
        }
        private void infoButton_Click(object sender, EventArgs e)
        {
            SongInfo songInfo = new SongInfo();
            songInfo.ShowDialog();
        }
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (Player.playing & !Player.paused)
            {
                progressIndicator.Text = Player.getSongPosition();
                if (Player.songchanged)
                {
                    Player.songchanged = false;
                    var metadata = Player.GetMetadata();
                    titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
                    Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
                    getAlbumArt();
                    MiniPlayerUpdate = true;
                }
            }
            else if (!Player.paused) UpdateGUI();
        }
        private void importplaylistButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog selectFileDialog = new OpenFileDialog())
            {
                selectFileDialog.Filter = "Playlist Files|*.xspf;*.asx;*.wax;*.wvx;*.b4s;*.m3u;*.m3u8;*.pls;*.smil;*.smi;*.zpl;";
                {
                    if (selectFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        IPlaylistIO theReader = PlaylistIOFactory.GetInstance().GetPlaylistIO(selectFileDialog.FileName);
                        try
                        {
                            foreach (string s in theReader.FilePaths)
                            {
                                Player.AddQueue(s);
                            }

                            Player.PlayMusic();
                            Player.playing = true;
                            getAlbumArt();
                        }
                        catch (System.IO.DirectoryNotFoundException)
                        {
                            MessageBox.Show("This playlist file cannot be played because one or more of the songs could not be found.", "Songs not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Player.ClearQueue();
                        }

                    }

                }
            }
        }
        private void queueButton_Click(object sender, EventArgs e)
        {
            QueueManagement queueManagement = new QueueManagement();
            queueManagement.ShowDialog();
        }
        private void nextButton_Click(object sender, EventArgs e)
        {
            Player.NextSong();
        }
        private void MiniPlayerButton_Click(object sender, EventArgs e)
        {
            using (MiniPlayer miniPlayer = new MiniPlayer())
            {
                Hide(); // Hide the main UI
                if (miniPlayer.ShowDialog() == DialogResult.Cancel)
                {
                    Show(); // If the fullscreen button on the miniplayer is pressed, unhide the main UI
                    miniPlayer.Dispose();
                }  
            }
        }
 // MENU BAR
        // MUSIC
        private void moreSongInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SongInfo songInfo = new SongInfo();
            songInfo.ShowDialog();
        }
        // HELP
        private void aboutFRESHMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        // LIBRARY
        private void ImportSong(string filepath)
        { 
            List<string> ExistingSongs = new List<string>();
            
            using (StreamReader file = File.OpenText("C:\\Users\\poohw\\OneDrive\\Desktop\\database.json")) // Read json file
            {
                JsonSerializer serializer = new JsonSerializer();
                Format database = (Format)serializer.Deserialize(file, typeof(Format));
                ExistingSongs = database.Songs; // Add the existing songs to a list to use later
            }     
            ExistingSongs.Add(filepath); // Add the new song in
            Format format = new Format();
            format.Version = 1;
            format.Songs = new List<string>();
            format.Songs = ExistingSongs;
            
            using (StreamWriter file = File.CreateText("C:\\Users\\poohw\\OneDrive\\Desktop\\database.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, format);
            }

        }
        private void library_importsongButton_Click(object sender, EventArgs e)
        {
            using (var selectFileDialog = new OpenFileDialog())
            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK) ImportSong(selectFileDialog.FileName);
            }
        }
 // LOGIC
        private void getAlbumArt()
        {
            ATL.Track theTrack = new ATL.Track(Player.filePath);
            IList<ATL.PictureInfo> embeddedPictures = theTrack.EmbeddedPictures;
            Graphics g = albumartBox.CreateGraphics();
            g.Clear(volumeBar.BackColor); // The background color of the volume bar should be the same as the highlight color of the UI
            albumartBox.Image?.Dispose(); // Clear resources used by the previous image
            foreach (ATL.PictureInfo pic in embeddedPictures)
            {
                albumartBox.Image = Image.FromStream(new System.IO.MemoryStream(pic.PictureData));
            }
        }
        private void volumeBar_MouseHover(object sender, EventArgs e) => toolTip1.SetToolTip(volumeBar, $"{volumeBar.Value.ToString()}%");
 // SETTINGS
        public void ApplySettings()
        {
            if (Properties.Settings.Default.Appearance_DarkMode)
            {
                BackColor = Color.FromArgb(44, 47, 51);
                volumeBar.BackColor = Color.FromArgb(44, 47, 51); // Handles highlight colors
                ForeColor = Color.White;
                // Welcome to the point where we get to repetitive code that would make any experienced programmer want to die.
                tabPage1.BackColor = Color.Black;tabPage2.BackColor = Color.Black;tabPage3.BackColor = Color.Black;tabPage4.BackColor = Color.Black; tabPage5.BackColor = Color.Black; tabPage6.BackColor = Color.Black;
                browsemusicButton.ForeColor = Color.Black;importplaylistButton.ForeColor = Color.Black;applychangesButton.ForeColor = Color.Black;browsebackButton.ForeColor = Color.Black;
                groupBox1.ForeColor = Color.White;groupBox2.ForeColor = Color.White;groupBox3.ForeColor = Color.White;
                menuBar.BackColor = Color.Black;menuBar.ForeColor = Color.White;
            }
           
        } 
        public void SetCheckBoxes()
        {
            Player.currentvolume = Properties.Settings.Default.General_Volume;
            volumeBar.Value = (int)(Properties.Settings.Default.General_Volume * 100.0f);
            MiniPlayerOpacityTrackBar.Value = (int)(Properties.Settings.Default.MiniPlayer_UnfocusedOpacity * 100.0f);
            if (Properties.Settings.Default.Appearance_DarkMode) darkradioButton.Checked = true; else lightradioButton.Checked = true;
        }
        private void applychangesButton_Click(object sender, EventArgs e)
        {
            if (darkradioButton.Checked) Properties.Settings.Default.Appearance_DarkMode = true; else Properties.Settings.Default.Appearance_DarkMode = false;
            if (backgroundradioButton.Checked) Properties.Settings.Default.Appearance_ImageDefault = true; else Properties.Settings.Default.Appearance_ImageDefault = false;
            Properties.Settings.Default.MiniPlayer_UnfocusedOpacity = MiniPlayerOpacityTrackBar.Value / 100.0f;
            Properties.Settings.Default.Save();
            ApplySettings();
        }

        private void ResetSettingsButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            ApplySettings();
            SetCheckBoxes();
        }

        
    }
    
}
