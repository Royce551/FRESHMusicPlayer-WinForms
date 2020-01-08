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
using FRESHMusicPlayer.Handlers;
namespace FRESHMusicPlayer
{
    public partial class UserInterface : Form
    {
        private List<string> SongLibrary = new List<string>();
        private List<string> ArtistLibrary = new List<string>();
        private List<string> ArtistSongLibrary = new List<string>();
        private List<string> AlbumLibrary = new List<string>();
        private List<string> AlbumSongLibrary = new List<string>();

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
            if (Properties.Settings.Default.General_DiscordIntegration) Player.DisposeRPC();
            Application.Exit();

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
                Player.UpdateRPC("Nothing", "Idle");
            }
        }
        // BUTTONS
        #region buttons
        private void browsemusicButton_Click(object sender, EventArgs e)
        {
            using (var selectFileDialog = new OpenFileDialog())

            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Player.AddQueue(selectFileDialog.FileName);
                    Player.PlayMusic();
                    if (AddTrackCheckBox.Checked) DatabaseHandler.ImportSong(selectFileDialog.FileName);
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
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                Player.UpdateRPC($"{metadata.Artist} - {metadata.Title}", "Playing");
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
                    Player.UpdateRPC("Nothing", "Paused");
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
                                if (AddTrackCheckBox.Checked) DatabaseHandler.ImportSong(s);
                            }

                            Player.PlayMusic();
                            Player.playing = true;
                            getAlbumArt();
                        }
                        catch (DirectoryNotFoundException)
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
        private void AccentColorButton_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.CustomColors = new int[] { 4160219 };
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.Appearance_AccentColorRed = colorDialog.Color.R;
                    Properties.Settings.Default.Appearance_AccentColorGreen = colorDialog.Color.G;
                    Properties.Settings.Default.Appearance_AccentColorBlue = colorDialog.Color.B;
                }
            }
        }

        private void SortLibraryButton_Click(object sender, EventArgs e)
        {
            List<string> songs = DatabaseHandler.ReadSongs();
            List<(string song, string path)> sort = new List<(string song, string path)>();

            foreach (string x in songs)
            {
                ATL.Track track = new ATL.Track(x);
                sort.Add(($"{track.Artist} - {track.Title}", x));
            }
            sort.Sort();
            DatabaseHandler.ClearLibrary();
            foreach ((string song, string path) x in sort)
            {
                DatabaseHandler.ImportSong(x.path);
            }
        }

        private void ReverseLibraryButton_Click(object sender, EventArgs e)
        {
            List<string> songs = DatabaseHandler.ReadSongs();
            List<(string song, string path)> sort = new List<(string song, string path)>();

            foreach (string x in songs)
            {
                ATL.Track track = new ATL.Track(x);
                sort.Add(($"{track.Artist} - {track.Title}", x));
            }
            sort.Sort();
            sort.Reverse();
            DatabaseHandler.ClearLibrary();
            foreach ((string song, string path) x in sort)
            {
                DatabaseHandler.ImportSong(x.path);
            }
        }
        private void Library_SongsDeleteButton_Click(object sender, EventArgs e)
        {
            foreach (int item in songsListBox.SelectedIndices) DatabaseHandler.DeleteSong(SongLibrary[item]);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (int item in Artists_SongsListBox.SelectedIndices) DatabaseHandler.DeleteSong(ArtistSongLibrary[item]);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            foreach (int item in Albums_SongsListBox.SelectedIndices) DatabaseHandler.DeleteSong(AlbumSongLibrary[item]);
        }
        #endregion buttons
        // MENU BAR
        #region menubar
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
        #endregion menubar
        // LIBRARY
        #region library

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab == songTab)
            {
                songsListBox.Items.Clear();
                SongLibrary.Clear();
                List<string> songs = DatabaseHandler.ReadSongs();
                var number = 0;
                songsListBox.BeginUpdate();
                foreach (string x in songs)
                {
                    ATL.Track theTrack = new ATL.Track(x);
                    songsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}"); // The labels people actually see
                    SongLibrary.Add(x); // References to the actual songs in the library 
                    number++;
                }
                songsListBox.EndUpdate();
                label12.Text = $"{number.ToString()} Songs";
            }
            else if (tabControl2.SelectedTab == artistTab)
            {
                Artists_ArtistsListBox.Items.Clear();
                ArtistLibrary.Clear();
                List<string> songs = DatabaseHandler.ReadSongs();
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
            else if (tabControl2.SelectedTab == albumTab)
            {
                Albums_AlbumsListBox.Items.Clear();
                AlbumLibrary.Clear();
                List<string> songs = DatabaseHandler.ReadSongs();
                foreach (string x in songs)
                {
                    ATL.Track theTrack = new ATL.Track(x);
                    Albums_AlbumsListBox.BeginUpdate();
                    if (!Albums_AlbumsListBox.Items.Contains(theTrack.Album))
                    {
                        Albums_AlbumsListBox.Items.Add(theTrack.Album);
                        AlbumLibrary.Add(x);
                    }
                    Albums_AlbumsListBox.EndUpdate();
                }
            }
        }
        private void Artists_ArtistsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Artists_SongsListBox.Items.Clear();
            ArtistSongLibrary.Clear();
            List<string> songs = DatabaseHandler.ReadSongs(); ;
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
        private void Albums_AlbumsListBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Albums_SongsListBox.Items.Clear();
            AlbumSongLibrary.Clear();
            List<string> songs = DatabaseHandler.ReadSongs();
            foreach (string x in songs)
            {
                ATL.Track theTrack = new ATL.Track(x);
                Albums_SongsListBox.BeginUpdate();
                if (theTrack.Album == (string)Albums_AlbumsListBox.SelectedItem)
                {
                    Albums_SongsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}");
                    AlbumSongLibrary.Add(x);
                }
                Albums_SongsListBox.EndUpdate();
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
        private void Albums_QueueButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in Albums_SongsListBox.SelectedIndices)
            {
                Player.AddQueue(AlbumSongLibrary[selectedItem]);
            }
            Albums_SongsListBox.ClearSelected();
        }

        private void Albums_PlayButton_Click(object sender, EventArgs e)
        {
            foreach (int selectedItem in Albums_SongsListBox.SelectedIndices)
            {
                Player.AddQueue(AlbumSongLibrary[selectedItem]);
            }
            Player.PlayMusic();
            Albums_SongsListBox.ClearSelected();
        }

        #endregion library
        // LOGIC
        private void getAlbumArt()
        {
            ATL.Track theTrack = new ATL.Track(Player.filePath);
            IList<ATL.PictureInfo> embeddedPictures = theTrack.EmbeddedPictures;
            Graphics g = albumartBox.CreateGraphics();
            g.Clear(Color.FromArgb(Properties.Settings.Default.Appearance_AccentColorRed, Properties.Settings.Default.Appearance_AccentColorGreen, Properties.Settings.Default.Appearance_AccentColorBlue)); // The background color of the volume bar should be the same as the highlight color of the UI
            albumartBox.Image?.Dispose(); // Clear resources used by the previous image
            foreach (ATL.PictureInfo pic in embeddedPictures)
            {
                albumartBox.Image = Image.FromStream(new System.IO.MemoryStream(pic.PictureData));
            }
        }
        private void UserInterface_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void UserInterface_DragDrop(object sender, DragEventArgs e)
        {
            string[] tracks = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string track in tracks)
            {
                Player.AddQueue(track);
                
                if (AddTrackCheckBox.Checked) DatabaseHandler.ImportSong(track);
            }
            Player.PlayMusic();
        }
        private void volumeBar_MouseHover(object sender, EventArgs e) => toolTip1.SetToolTip(volumeBar, $"{volumeBar.Value.ToString()}%");
        // SETTINGS
        #region settings
        public void ApplySettings()
        {
            if (Properties.Settings.Default.Appearance_DarkMode) ThemeHandler.SetColors(this,
                                                                                        (Properties.Settings.Default.Appearance_AccentColorRed, Properties.Settings.Default.Appearance_AccentColorGreen, Properties.Settings.Default.Appearance_AccentColorBlue),
                                                                                        (255, 255, 255),
                                                                                        Color.Black,
                                                                                        Color.White);
            else ThemeHandler.SetColors(this, (Properties.Settings.Default.Appearance_AccentColorRed, Properties.Settings.Default.Appearance_AccentColorGreen, Properties.Settings.Default.Appearance_AccentColorBlue), (255, 255, 255), Color.White, Color.Black);
            if (Properties.Settings.Default.General_DiscordIntegration) Player.InitDiscordRPC(); else Player.DisposeRPC();
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
            //if (backgroundradioButton.Checked) Properties.Settings.Default.Appearance_ImageDefault = true; else Properties.Settings.Default.Appearance_ImageDefault = false;
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







        #endregion settings

        
    }

}
