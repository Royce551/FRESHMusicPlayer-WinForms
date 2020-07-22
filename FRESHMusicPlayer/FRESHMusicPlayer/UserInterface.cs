﻿using ATL.Playlist;
using ATL;
using FRESHMusicPlayer.Handlers;
using FRESHMusicPlayer.Handlers.Integrations;
using FRESHMusicPlayer.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Diagnostics;
namespace FRESHMusicPlayer
{
    public partial class UserInterface : Form
    {
        private List<string> SongLibrary = new List<string>();
        private List<string> ArtistLibrary = new List<string>();
        private List<string> ArtistSongLibrary = new List<string>();
        private List<string> AlbumLibrary = new List<string>();
        private List<string> AlbumSongLibrary = new List<string>();
        private List<string> SearchSongLibrary = new List<string>();
        private Image albumArt;
        
        private List<Form> overlays = new List<Form>();
        private int VolumeTimer = 0;
        private int SearchTasksRunning = 0;
        private bool TaskIsRunning = false;
        public static bool LibraryNeedsUpdating = true;
        public UserInterface()
        {
            InitializeComponent();
            ApplySettings();

            Player.songChanged += new EventHandler(songChangedHandler);
            Player.songStopped += new EventHandler(songStoppedHandler);
            Player.songException += new EventHandler<PlaybackExceptionEventArgs>(songExceptionHandler);
            if (Properties.Settings.Default.General_AutoCheckForUpdates)
            {
                Task task = Task.Run(Player.UpdateIfAvailable);
            }
            SetCheckBoxes();

        }

        private void Form1_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.General_Volume = Player.currentvolume;
            Properties.Settings.Default.General_LastUpdate = Player.lastUpdateCheck;
            Properties.Settings.Default.Save();
            if (Properties.Settings.Default.General_DiscordIntegration) Player.DisposeRPC();
            //Application.Exit();
            Task.Run(Player.ShutdownTheApp);
        }
        #region Controls
        private void PlayButton()
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
        private static void StopButton()
        {
            Player.ClearQueue();
            Player.StopMusic();
        }
        private void pauseplayButton_Click(object sender, EventArgs e) => PlayButton();
        private void stopButton_Click(object sender, EventArgs e) => StopButton();

        private void volumeBar_Scroll(object sender, EventArgs e)
        {
            Player.currentvolume = (float)volumeBar.Value / 100.0f;
            if (Player.playing) Player.UpdateSettings();
            VolumeTimer = 100;
        }
        private void infoButton_Click(object sender, EventArgs e) => infobuttonContextMenu.Show(infoButton, infoButton.Location);
        private void nextButton_Click(object sender, EventArgs e) => Player.NextSong();
        private void queuemanagementMenuItem_Click(object sender, EventArgs e)
        {
            QueueManagement queueManagement = new QueueManagement();
            queueManagement.Show();
        }

        private void miniplayerMenuItem_Click(object sender, EventArgs e)
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

        private void trackInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SongInfo songInfo = new SongInfo())
                songInfo.ShowDialog();
        }
        private void infoButton_MouseClick(object sender, MouseEventArgs e) => infobuttonContextMenu.Show(infoButton, e.Location);
        #endregion
        #region LibraryTab
        private void browsemusicButton_Click(object sender, EventArgs e) => BrowseMusic();
        private void openAudioFileToolStripMenuItem_Click(object sender, EventArgs e) => BrowseMusic();
        private async void BrowseMusic()
        {
            using (var selectFileDialog = new OpenFileDialog())
            {
                selectFileDialog.Filter = "Audio Files|*.wav;*.aiff;*.mp3;*.wma;*.3g2;*.3gp;*.3gp2;*.3gpp;*.asf;*.wmv;*.aac;*.adts;*.avi;*.m4a;*.m4a;*.m4v;*.mov;*.mp4;*.sami;*.smi;*.flac|Other|*";
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Player.AddQueue(selectFileDialog.FileName);
                    Player.PlayMusic();
                    if (AddTrackCheckBox.Checked) DatabaseHandler.ImportSong(selectFileDialog.FileNames);
                    LibraryNeedsUpdating = true;
                    await UpdateLibrary();
                }

            }
        }
        private async void BrowsePlaylist()
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
                            if (AddTrackCheckBox.Checked) DatabaseHandler.ImportSong(theReader.FilePaths);
                            foreach (string s in theReader.FilePaths)
                            {
                                Player.AddQueue(s);

                            }
                            LibraryNeedsUpdating = true;
                            Player.PlayMusic();
                            await UpdateLibrary();
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
        private void importplaylistButton_Click(object sender, EventArgs e) => BrowsePlaylist();
        private void openPlaylistFileToolStripMenuItem_Click(object sender, EventArgs e) => BrowsePlaylist();
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Task.Run(Player.ShutdownTheApp);
        private void UserInterface_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
            Notification notification = new Notification("", "Hold CTRL to play without importing\nHold SHIFT to queue only", 3000);
            notification.Location = Location;
            notification.Show();
        }

        private async void UserInterface_DragDrop(object sender, DragEventArgs e)
        {
            if (!TaskIsRunning)
            {
                await Task.Run(() =>
                {
                    TaskIsRunning = true;
                    
                    string[] tracks = (string[])e.Data.GetData(DataFormats.FileDrop);
                    Player.AddQueue(tracks);

                    if (AddTrackCheckBox.Checked && (e.KeyState & 8) != 8 /*CTRL key*/) DatabaseHandler.ImportSong(tracks);

                });
                TaskIsRunning = false;
                LibraryNeedsUpdating = true;
                if ((e.KeyState & 4) != 4 /*SHIFT key*/) Player.PlayMusic();
                await UpdateLibrary();
            }
        }
        private async void tabControl2_SelectedIndexChanged(object sender, EventArgs e) => await UpdateLibrary();
        public async Task UpdateLibrary()
        {
            if (LibraryNeedsUpdating && !TaskIsRunning)
            {
                TaskIsRunning = true;
                var songs = DatabaseHandler.ReadSongs();
                int tracknumber = 0;
                var task1 = Task.Run(() =>
                {
                    songsListBox.Invoke(new Action(() => songsListBox.BeginUpdate()));
                    songsListBox.Invoke(new Action(() => songsListBox.Items.Clear()));
                    SongLibrary.Clear();
                    foreach (string x in songs)
                    {
                        Track theTrack = new Track(x);
                        songsListBox.Invoke(new Action(() => songsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}"))); // The labels people actually see
                        SongLibrary.Add(x); // References to the actual songs in the library 
                        tracknumber++;
                    }
                    songsListBox.Invoke(new Action(() => songsListBox.EndUpdate()));
                });
                var task2 = Task.Run(() =>
                {
                    Artists_ArtistsListBox.Invoke(new Action(() => Artists_ArtistsListBox.BeginUpdate()));
                    Artists_ArtistsListBox.Invoke(new Action(() => Artists_ArtistsListBox.Items.Clear()));
                    ArtistLibrary.Clear();
                    foreach (string x in songs)
                    {
                        Track theTrack = new Track(x);
                        if (!Artists_ArtistsListBox.Items.Contains(theTrack.Artist))
                        {
                            Artists_ArtistsListBox.Invoke(new Action(() => Artists_ArtistsListBox.Items.Add(theTrack.Artist)));
                            ArtistLibrary.Add(x);
                        }
                    }
                    Artists_ArtistsListBox.Invoke(new Action(() => Artists_ArtistsListBox.EndUpdate()));
                });
                var task3 = Task.Run(() =>
                {
                    Albums_AlbumsListBox.Invoke(new Action(() => Albums_AlbumsListBox.BeginUpdate()));
                    Albums_AlbumsListBox.Invoke(new Action(() => Albums_AlbumsListBox.Items.Clear()));
                    AlbumLibrary.Clear();
                    foreach (string x in songs)
                    {
                        Track theTrack = new Track(x);
                        if (!Albums_AlbumsListBox.Items.Contains(theTrack.Album))
                        {
                            Albums_AlbumsListBox.Invoke(new Action(() => Albums_AlbumsListBox.Items.Add(theTrack.Album)));
                            AlbumLibrary.Add(x);
                        }
                    }
                    Albums_AlbumsListBox.Invoke(new Action(() => Albums_AlbumsListBox.EndUpdate()));
                });

                await Task.WhenAll(task1, task2, task3);

                label12.Text = $"{tracknumber} Tracks";
                TaskIsRunning = false;
                LibraryNeedsUpdating = false;
            }
            else if (TaskIsRunning)
            {
                Notification notification = new Notification("Hold up!", "The library is still loading.\nWait for it to finish first.", 2500);
                notification.Location = Location;
                notification.Show();
            }
        }

        private async void Artists_ArtistsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!TaskIsRunning)
            {
                Artists_SongsListBox.BeginUpdate();
                var SelectedItem = (string)Artists_ArtistsListBox.SelectedItem;
                await Task.Run(() =>
                {
                    Artists_SongsListBox.Invoke(new Action(() => Artists_SongsListBox.Items.Clear()));
                    ArtistSongLibrary.Clear();
                    List<string> songs = DatabaseHandler.ReadSongs();
                    foreach (string x in songs)
                    {
                        ATL.Track theTrack = new ATL.Track(x);

                        if (theTrack.Artist == SelectedItem)
                        {
                            Artists_SongsListBox.Invoke(new Action(() => Artists_SongsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}")));
                            ArtistSongLibrary.Add(x);
                        }

                    }
                });
                Artists_SongsListBox.EndUpdate();
            }
            else
            {
                Notification notification = new Notification("Hold up!", "Can't do this now because a background\ntask is working with the library.", 2500);
                notification.Show();
            }
        }
        private async void searchBox_KeyUp(object sender, KeyEventArgs e)
        {

            if (SearchTasksRunning == 0) // Prevent multiple of these tasks happening at once (else this will use massive amounts of resources)
            {
                SearchTasksRunning++;
                Search_SongsListBox.BeginUpdate();
                await Task.Run(() =>
                {
                    Search_SongsListBox.Invoke(new Action(() => Search_SongsListBox.Items.Clear()));
                    SearchSongLibrary.Clear();
                    List<string> songs = DatabaseHandler.ReadSongs();
                    foreach (string x in songs)
                    {
                        //if (SearchTasksRunning > 1) break;
                        ATL.Track theTrack = new ATL.Track(x);
                        if (theTrack.Artist.Contains(searchBox.Text) || theTrack.Title.Contains(searchBox.Text))
                        {
                            Search_SongsListBox.Invoke(new Action(() => Search_SongsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}")));
                            SearchSongLibrary.Add(x);

                        }
                    }
                });
                Search_SongsListBox.EndUpdate();
                SearchTasksRunning--;
            }
        }

        private async void Albums_AlbumsListBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

            if (!TaskIsRunning)
            {
                Albums_SongsListBox.BeginUpdate();
                var SelectedItem = (string)Albums_AlbumsListBox.SelectedItem;
                await Task.Run(() =>
                {
                    Albums_SongsListBox.Invoke(new Action(() => Albums_SongsListBox.Items.Clear()));
                    AlbumSongLibrary.Clear();
                    List<string> songs = DatabaseHandler.ReadSongs();
                    foreach (string x in songs)
                    {
                        ATL.Track theTrack = new ATL.Track(x);

                        if (theTrack.Album == SelectedItem)
                        {
                            Albums_SongsListBox.Invoke(new Action(() => Albums_SongsListBox.Items.Add($"{theTrack.Artist} - {theTrack.Title}")));
                            AlbumSongLibrary.Add(x);
                        }

                    }
                });
                Albums_SongsListBox.EndUpdate();
            }
            else
            {
                Notification notification = new Notification("Hold up!", "Can't do this now because a background\ntask is working with the library.", 2500);
                notification.Show();
            }

        }
        private void songsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (Player.playing) // If music is already playing, we don't want the user to press the "Play Song" button
            {
                Library_SongsPlayButton.Enabled = false;
                Library_SongsQueueButton.Enabled = true;
            }
            else
            {
                Library_SongsPlayButton.Enabled = true;
                Library_SongsQueueButton.Enabled = false;
            }*/
        }
        #region LibraryButtons
        // B U T T O N.
        private void Library_SongsDeleteButton_Click(object sender, EventArgs e) => LibraryDeleteButton(songsListBox, SongLibrary);
        private void button4_Click(object sender, EventArgs e) => LibraryDeleteButton(Artists_SongsListBox, ArtistSongLibrary);
        private void button5_Click(object sender, EventArgs e) => LibraryDeleteButton(Albums_SongsListBox, AlbumSongLibrary);
        private void Library_SongsPlayButton_Click(object sender, EventArgs e) => LibraryPlayButton(songsListBox, SongLibrary);
        private void Library_SongsQueueButton_Click(object sender, EventArgs e) => LibraryQueueButton(songsListBox, SongLibrary);
        private void Artists_PlayButton_Click(object sender, EventArgs e) => LibraryPlayButton(Artists_SongsListBox, ArtistSongLibrary);
        private void Artists_QueueButton_Click(object sender, EventArgs e) => LibraryQueueButton(Artists_SongsListBox, ArtistSongLibrary);
        private void Albums_QueueButton_Click(object sender, EventArgs e) => LibraryQueueButton(Albums_SongsListBox, AlbumSongLibrary);
        private void Albums_PlayButton_Click(object sender, EventArgs e) => LibraryPlayButton(Albums_SongsListBox, AlbumSongLibrary);
        private void Search_PlayButton_Click(object sender, EventArgs e) => LibraryPlayButton(Search_SongsListBox, SearchSongLibrary);
        private void Search_QueueButton_Click(object sender, EventArgs e) => LibraryQueueButton(Search_SongsListBox, SearchSongLibrary);
        private void Search_DeleteButton_Click(object sender, EventArgs e) => LibraryDeleteButton(Search_SongsListBox, SearchSongLibrary);
        private void LibraryPlayButton(ListBox listBox, List<string> list)
        {
            foreach (int selectedItem in listBox.SelectedIndices)
            {
                Player.AddQueue(list[selectedItem]);
            }
            Player.PlayMusic();
            listBox.ClearSelected();
        }
        private void LibraryQueueButton(ListBox listBox, List<string> list)
        {
            foreach (int selectedItem in listBox.SelectedIndices)
            {
                Player.AddQueue(list[selectedItem]);
            }
            listBox.ClearSelected();
        }
        private async void LibraryDeleteButton(ListBox listBox, List<string> list)
        {
            foreach (int item in listBox.SelectedIndices) DatabaseHandler.DeleteSong(list[item]);
            LibraryNeedsUpdating = true;
            await UpdateLibrary();
        }
        #endregion
        private void searchBox_Enter(object sender, EventArgs e) => searchBox.Text = ""; // Get rid of the placeholder text
        #endregion
        #region SettingsTab
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
                SetCheckBoxes();
                ApplySettings();
            }
        }

        private async void SortLibraryButton_Click(object sender, EventArgs e)
        {
            if (!TaskIsRunning) await Task.Run(() =>
            {
                TaskIsRunning = true;
                List<string> songs = DatabaseHandler.ReadSongs();
                List<(string song, string path)> sort = new List<(string song, string path)>();

                foreach (string x in songs)
                {
                    Track track = new Track(x);
                    sort.Add(($"{track.Artist} - {track.Title}", x));
                }
                sort.Sort();
                DatabaseHandler.ClearLibrary();
                foreach ((string song, string path) x in sort)
                {
                    DatabaseHandler.ImportSong(x.path);
                }
            });
            TaskIsRunning = false;
            LibraryNeedsUpdating = true;
            Notification notification = new Notification("Success!", "Your database was sorted successfully.", 5000);
            notification.Location = Location;
            notification.Show();
        }

        private async void ReverseLibraryButton_Click(object sender, EventArgs e)
        {
            if (!TaskIsRunning) await Task.Run(() =>
            {
                TaskIsRunning = true;
                List<string> songs = DatabaseHandler.ReadSongs();
                List<(string song, string path)> sort = new List<(string song, string path)>();

                foreach (string x in songs)
                {
                    Track track = new Track(x);
                    sort.Add(($"{track.Artist} - {track.Title}", x));
                }
                sort.Sort();
                sort.Reverse();
                DatabaseHandler.ClearLibrary();
                foreach ((string song, string path) x in sort)
                {
                    DatabaseHandler.ImportSong(x.path);
                }
            });
            TaskIsRunning = false;
            LibraryNeedsUpdating = true;
            Notification notification = new Notification("Success!", "Your database was sorted successfully.", 5000);
            notification.Location = Location;
            notification.Show();
        }
        private void CheckNowButton_Click(object sender, EventArgs e)
        {
            UpdateStatusLabel.Text = "Checking for updates - The window might hang for a bit.";
            Task task = Task.Run(Player.UpdateIfAvailable);
            while (!task.IsCompleted) { }
            task.Dispose();
            SetCheckBoxes();
        }

        private void ColorResetButton_Click(object sender, EventArgs e)
        {
            (Properties.Settings.Default.Appearance_AccentColorRed, Properties.Settings.Default.Appearance_AccentColorGreen, Properties.Settings.Default.Appearance_AccentColorBlue) = (4, 160, 219);
            Properties.Settings.Default.Save();
            SetCheckBoxes();
            ApplySettings();
        }
        private async void NukeLibraryButton_Click(object sender, EventArgs e)
        {
            var dialog = MessageBox.Show("You are about to irreversibly clear your entire library.", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (dialog == DialogResult.OK) 
            {
                DatabaseHandler.ClearLibrary();
                await UpdateLibrary();
            }
        }
        #endregion
        #region Logic
        private void songChangedHandler(object sender, EventArgs e)
        {
            progressTimer.Enabled = true;
            ATL.Track metadata = new ATL.Track(Player.filePath);
            titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
            Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
            getAlbumArt();
            ProgressBar.Maximum = (int)Player.audioFile.TotalTime.TotalSeconds;
            if (Properties.Settings.Default.General_DiscordIntegration)
            {
                Player.UpdateRPC("play", metadata.Artist, metadata.Title);
            }
        }
        private void songStoppedHandler(object sender, EventArgs e)
        {
            titleLabel.Text = "Nothing Playing";
            Text = "FRESHMusicPlayer";
            progressIndicator.Text = "(nothing playing)";
            progressTimer.Enabled = false;
        }
        private void songExceptionHandler(object sender, PlaybackExceptionEventArgs e)
        {
            Notification notification = new Notification("An error occured.", $"{e.Details}\nWe'll skip to the next track for you.", 2500);
            notification.Location = Location;
            notification.Show();
            Player.NextSong();
        }
        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (Player.playing & !Player.paused)
            {
                Player.avoidnextqueue = false;
                progressIndicator.Text = Player.getSongPosition();
                if ((int)Player.audioFile.CurrentTime.TotalSeconds <= ProgressBar.Maximum) ProgressBar.Value = (int)Player.audioFile.CurrentTime.TotalSeconds;
            }
        }
        private void getAlbumArt()
        {
            ATL.Track theTrack = new ATL.Track(Player.filePath);
            IList<ATL.PictureInfo> embeddedPictures = theTrack.EmbeddedPictures;
            if (embeddedPictures.Count != 0)
            {
                albumArt = Image.FromStream(new MemoryStream(embeddedPictures[0].PictureData));
                albumartBox.Image = albumArt;
            }
            else
            {
                albumArt = null;
                albumartBox.Image = albumArt;
            }
        }

        private void VolumeToggleButton_MouseEnter(object sender, EventArgs e)
        {
            label3.Visible = true;
            volumeBar.Visible = true;
            VolumeTimer = 15;
            VolumeBarTimer.Enabled = true;
        }

        private void VolumeBarTimer_Tick(object sender, EventArgs e)
        {
            if (VolumeTimer > 0) VolumeTimer -= 1;
            if (VolumeTimer == 0)
            {
                label3.Visible = false;
                volumeBar.Visible = false;
                VolumeBarTimer.Enabled = false;
            }

        }

        private void ProgressBar_Scroll(object sender, EventArgs e)
        {
            if (Player.playing) Player.RepositionMusic(ProgressBar.Value);
        }

        private void volumeBar_MouseEnter(object sender, EventArgs e) => VolumeBarTimer.Enabled = false;

        private void volumeBar_MouseLeave(object sender, EventArgs e) => VolumeBarTimer.Enabled = true;

        private void albumartBox_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void UserInterface_Load(object sender, EventArgs e) => await UpdateLibrary();
        private void volumeBar_MouseHover(object sender, EventArgs e) => toolTip1.SetToolTip(volumeBar, $"{volumeBar.Value}%");

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!searchBox.Focused && Properties.Settings.Default.General_KeyboardNavigation)
            {
                switch (keyData)        // TODO: In the future, I wanna make these keys rebindable
                {
                    case Keys.A:                            // Tab controls
                        tabControl2.SelectTab(songTab);
                        return true;
                    case Keys.S:
                        tabControl2.SelectTab(artistTab);
                        return true;
                    case Keys.D:
                        tabControl2.SelectTab(albumTab);
                        return true;
                    case Keys.F:
                        tabControl2.SelectTab(searchTab);
                        return true;
                    case Keys.G:
                        tabControl2.SelectTab(importTab);
                        return true;
                    case Keys.Q:
                        tabControl1.SelectTab(tabPage3);
                        return true;
                    case Keys.MediaPlayPause:               // Playback controls
                    case Keys.C:
                        PlayButton();
                        return true;
                    case Keys.V:
                    case Keys.MediaNextTrack:
                        Player.NextSong();
                        return true;
                    case Keys.X:
                    case Keys.MediaStop:
                        StopButton();
                        return true;
                    case Keys.P:
                        TagEditor tagEditor = new TagEditor(new List<string> { Player.filePath });
                        tagEditor.Show();
                        return true;
                    case Keys.O:
                        
                        return true;
                    default:
                        break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion
        #region OverlaySystem
        private void AddOverlay(Form form)
        {
            if (overlays.Count != 0)
            {
                foreach (Form overlay in overlays) overlay.Dispose();
                overlays.Clear();
            }
            overlays.Add(form);
            overlays[0].Owner = this;
            overlays[0].Location = Location;
            overlays[0].Size = new Size(Width, Height - controlsBox.Height);
            overlays[0].ShowDialog();
        }
        #endregion
        #region menubar
        // MUSIC

        // HELP
        private void aboutFRESHMusicPlayerToolStripMenuItem_Click(object sender, EventArgs e) => Process.Start("https://imgur.com/sozTGTK");
        private void aboutFRESHMusicPlayerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{Application.ProductVersion}\nBy Squid Grill & Open source contributors :)\nLicensed under the GPL-3.0 license", "About FRESHMusicPlayer", MessageBoxButtons.OK);
        }
        #endregion menubar
        #region settings
        public void ApplySettings()
        {
            if (Properties.Settings.Default.Appearance_DarkMode) ThemeHandler.SetColors(this,
                                                                                        (44, 47, 51),
                                                                                        (255, 255, 255),
                                                                                        Color.Black,
                                                                                        Color.White);
            else ThemeHandler.SetColors(this, (Properties.Settings.Default.Appearance_AccentColorRed, Properties.Settings.Default.Appearance_AccentColorGreen, Properties.Settings.Default.Appearance_AccentColorBlue), (255, 255, 255), Color.White, Color.Black);
            if (Properties.Settings.Default.General_DiscordIntegration) Player.InitDiscordRPC(); else Player.DisposeRPC();
        
        }
        public void SetCheckBoxes()
        {
            SettingsVersionText.Text = $"Current Version - {Application.ProductVersion}";

            Player.currentvolume = Properties.Settings.Default.General_Volume;
            volumeBar.Value = (int)(Properties.Settings.Default.General_Volume * 100.0f);
            MiniPlayerOpacityTrackBar.Value = (int)(Properties.Settings.Default.MiniPlayer_UnfocusedOpacity * 100.0f);
            if (Properties.Settings.Default.Appearance_DarkMode) darkradioButton.Checked = true; else lightradioButton.Checked = true;
            if (Properties.Settings.Default.General_DiscordIntegration) discordCheckBox.Checked = true; else discordCheckBox.Checked = false;
            if (Properties.Settings.Default.General_AutoCheckForUpdates) CheckUpdatesAutoCheckBox.Checked = true; else CheckUpdatesAutoCheckBox.Checked = false;
            if (Properties.Settings.Default.General_PreRelease) BlueprintCheckBox.Checked = true; else BlueprintCheckBox.Checked = false;
            if (Properties.Settings.Default.General_KeyboardNavigation) KeyboardNavCheckBox.Checked = true; else KeyboardNavCheckBox.Checked = false;

            var UpdateCheck = Properties.Settings.Default.General_LastUpdate;
            if (UpdateCheck.Year < 1500)
            {
                UpdateStatusLabel.Text = "Never checked for updates";
                return;
            }

            UpdateStatusLabel.Text = $"Last Checked {UpdateCheck}";
        }
        private void applychangesButton_Click(object sender, EventArgs e)
        {
            if (darkradioButton.Checked) Properties.Settings.Default.Appearance_DarkMode = true; else Properties.Settings.Default.Appearance_DarkMode = false;
            if (BlueprintCheckBox.Checked) Properties.Settings.Default.General_PreRelease = true; else Properties.Settings.Default.General_PreRelease = false;
            if (discordCheckBox.Checked) Properties.Settings.Default.General_DiscordIntegration = true; else Properties.Settings.Default.General_DiscordIntegration = false;
            if (CheckUpdatesAutoCheckBox.Checked) Properties.Settings.Default.General_AutoCheckForUpdates = true; else Properties.Settings.Default.General_AutoCheckForUpdates = false;
            if (KeyboardNavCheckBox.Checked) Properties.Settings.Default.General_KeyboardNavigation = true; else Properties.Settings.Default.General_KeyboardNavigation = false;
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

        private void UserInterface_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            //Font = new Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        }

        private void previousTrackToolStripMenuItem_Click(object sender, EventArgs e) => Player.PreviousSong();

        private void repeatOnceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Player.RepeatOnce)
            {
                repeatOnceToolStripMenuItem.Checked = true;
                Player.RepeatOnce = true;
            }
            else
            {
                repeatOnceToolStripMenuItem.Checked = false;
                Player.RepeatOnce = false;
            }
        }

        private void shuffleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Player.Shuffle)
            {
                shuffleToolStripMenuItem.Checked = true;
                Player.Shuffle = true;
            }
            else
            {
                shuffleToolStripMenuItem.Checked = false;
                Player.Shuffle = false;
            }
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TagEditor tagEditor = new TagEditor(new List<string> { Player.filePath });
            tagEditor.Owner = this;
            tagEditor.Show();
        }

        private void editSelectedInSongsTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> x = new List<string>();
            foreach (int y in songsListBox.SelectedIndices)
            {
                x.Add(SongLibrary[y]);
            }
            TagEditor tagEditor = new TagEditor(x);
            tagEditor.Owner = this;
            tagEditor.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox3.SelectedIndex)
            {
                case 0: // Sky Blue
                    Properties.Settings.Default.Appearance_AccentColorRed = 51;
                    Properties.Settings.Default.Appearance_AccentColorGreen = 139;
                    Properties.Settings.Default.Appearance_AccentColorBlue = 193;
                    break;
                case 1: // Sea Green
                    Properties.Settings.Default.Appearance_AccentColorRed = 105;
                    Properties.Settings.Default.Appearance_AccentColorGreen = 181;
                    Properties.Settings.Default.Appearance_AccentColorBlue = 120;
                    break;
                case 2: // Soft Red
                    Properties.Settings.Default.Appearance_AccentColorRed = 213;
                    Properties.Settings.Default.Appearance_AccentColorGreen = 70;
                    Properties.Settings.Default.Appearance_AccentColorBlue = 63;
                    break;
                case 3: // Fuschia Purple
                    Properties.Settings.Default.Appearance_AccentColorRed = 193;
                    Properties.Settings.Default.Appearance_AccentColorGreen = 96;
                    Properties.Settings.Default.Appearance_AccentColorBlue = 195;
                    break;
                case 4: // Classic Blue
                    Properties.Settings.Default.Appearance_AccentColorRed = 4;
                    Properties.Settings.Default.Appearance_AccentColorGreen = 160;
                    Properties.Settings.Default.Appearance_AccentColorBlue = 219;
                    break;
            }
            SetCheckBoxes();
            ApplySettings();
        }
    }

}
