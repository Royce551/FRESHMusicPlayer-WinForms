using ATL.Playlist;
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
using DiscordRPC;
namespace FRESHMusicPlayer
{
    public partial class UserInterface : Form
    {
        public static bool MiniPlayerUpdate = false;
        private List<string> SongLibrary = new List<string>();
        private List<string> ArtistLibrary = new List<string>();
        private List<string> ArtistSongLibrary = new List<string>();
        
        public UserInterface()
        {
            InitializeComponent();
            ApplySettings();
            SetCheckBoxes();
            Player.songChanged += new EventHandler(this.songChangedHandler);
        }
        // Because closing UserInterface doesn't close the main fore and therefore the application, 
        // this function does that job for us :)
        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.General_Volume = Player.currentvolume;
            Properties.Settings.Default.Save();
            Application.Exit();
            if (Properties.Settings.Default.General_DiscordIntegration) Player.client.Dispose();
        }
 // Communication with other forms
        private void UpdateGUI()
        {
            titleLabel.Text = "Nothing Playing";
            Text = "FRESHMusicPlayer";
            progressIndicator.Text = "(nothing playing)";
            Player.position = 0;
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                Update("Nothing", "Idle");
            }
        }
// BUTTONS
        private void browsemusicButton_Click(object sender, EventArgs e)
        {
            using (var selectFileDialog = new OpenFileDialog())

            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Player.AddQueue(selectFileDialog.FileName);
                    Player.PlayMusic();
                    if (AddTrackCheckBox.Checked) ImportSong(selectFileDialog.FileName);
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
        private void songChangedHandler(object sender, EventArgs e)
        {
            var metadata = Player.GetMetadata();
            titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
            Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
            getAlbumArt();
            MiniPlayerUpdate = true;
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                Update($"{metadata.Artist} - {metadata.Title}", "Playing");
            }
        }
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (Player.playing & !Player.paused)
            {
                progressIndicator.Text = Player.getSongPosition();
                if (Player.songchanged)
                {
                    Player.songchanged = false;
                    /*var metadata = Player.GetMetadata();
                    titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
                    Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
                    getAlbumArt();
                    MiniPlayerUpdate = true;
                    if (Properties.Settings.Default.General_DiscordIntegration)
                    {
                        Update($"{metadata.Artist} - {metadata.Title}", "Playing");
                    }*/
                }
            }
            else if (!Player.paused) UpdateGUI();
            else
            {
                if (Properties.Settings.Default.General_DiscordIntegration)
                {
                    Update("Nothing", "Paused");
                }
            }
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
                                if (AddTrackCheckBox.Checked) ImportSong(s);
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
        private List<string> ReadSongs()
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
 
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab == songTab)
            {
                songsListBox.Items.Clear();
                SongLibrary.Clear();
                List<string> songs = ReadSongs();
                var number = 0;
                foreach (string x in songs)
                {
                    ATL.Track theTrack = new ATL.Track(x);
                    songsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}"); // The labels people actually see
                    SongLibrary.Add(x); // References to the actual songs in the library 
                    number++;
                }
                label12.Text = $"{number.ToString()} Songs";
            }
            else if (tabControl2.SelectedTab == artistTab)
            {
                Artists_ArtistsListBox.Items.Clear();
                ArtistLibrary.Clear();
                List<string> songs = ReadSongs();
                foreach (string x in songs)
                {
                    ATL.Track theTrack = new ATL.Track(x);
                    Artists_ArtistsListBox.BeginUpdate();
                    if (!Artists_ArtistsListBox.Items.Contains(theTrack.Artist))
                    {
                        Artists_ArtistsListBox.Items.Add(theTrack.Artist);
                        ArtistLibrary.Add(x);
                    }
                    Artists_ArtistsListBox.EndUpdate();
                }
            }
        }
        private void Artists_ArtistsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Artists_SongsListBox.Items.Clear();
            ArtistSongLibrary.Clear();
            List<string> songs = ReadSongs();
            foreach (string x in songs)
            {
                ATL.Track theTrack = new ATL.Track(x);
                Artists_SongsListBox.BeginUpdate();
                if (theTrack.Artist == (string)Artists_ArtistsListBox.SelectedItem)
                {
                    Artists_SongsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}");
                    ArtistSongLibrary.Add(x);
                }
                Artists_SongsListBox.EndUpdate();
            }
        }
        private void songsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Player.playing) // If music is already playing, we don't want the user to press the "Play Song" button
            {
                Library_SongsPlayButton.Enabled = false;
                Library_SongsQueueButton.Enabled = true;
            }
            else 
            {
                Library_SongsPlayButton.Enabled = true;
                Library_SongsQueueButton.Enabled = false;
            }
        }
        private void Library_SongsPlayButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in songsListBox.SelectedIndices)
            {
                Player.AddQueue(SongLibrary[selectedItem]);
            }
            Player.PlayMusic();
            songsListBox.ClearSelected();
        }
        private void Library_SongsQueueButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in songsListBox.SelectedIndices)
            {
                Player.AddQueue(SongLibrary[selectedItem]);
            }
            songsListBox.ClearSelected();
        }
        private void Artists_PlayButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in Artists_SongsListBox.SelectedIndices)
            {
                Player.AddQueue(ArtistSongLibrary[selectedItem]);
            }
            Player.PlayMusic();
            Artists_SongsListBox.ClearSelected();
        }
        private void Artists_QueueButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in Artists_SongsListBox.SelectedIndices)
            {
                Player.AddQueue(ArtistSongLibrary[selectedItem]);
            }
            Artists_SongsListBox.ClearSelected();
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
            if (Properties.Settings.Default.General_DiscordIntegration) discordCheckBox.Checked = true; else discordCheckBox.Checked = false;
        }
        private void applychangesButton_Click(object sender, EventArgs e)
        {
            if (darkradioButton.Checked) Properties.Settings.Default.Appearance_DarkMode = true; else Properties.Settings.Default.Appearance_DarkMode = false;
            if (backgroundradioButton.Checked) Properties.Settings.Default.Appearance_ImageDefault = true; else Properties.Settings.Default.Appearance_ImageDefault = false;
            if (discordCheckBox.Checked) Properties.Settings.Default.General_DiscordIntegration = true; else Properties.Settings.Default.General_DiscordIntegration = false;
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
        //DISCORD
        public void Update(string Activity, string Song)
        {
            Player.client.SetPresence(new RichPresence()
            {
                Details = Song,
                State = Activity,
            }
            );

        }
    }
    
}
