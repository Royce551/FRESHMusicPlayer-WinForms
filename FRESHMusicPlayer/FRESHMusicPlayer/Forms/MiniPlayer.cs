using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRESHMusicPlayer.Handlers;
namespace FRESHMusicPlayer
{
    public partial class MiniPlayer : Form
    {
        private float UnfocusedOpacity = Properties.Settings.Default.MiniPlayer_UnfocusedOpacity;
        public MiniPlayer()
        {
            InitializeComponent();
            Player.songChanged += new EventHandler(this.songChangedHandler);
            if (Properties.Settings.Default.Appearance_DarkMode) ThemeHandler.SetColors(this, (44, 47, 51), (255, 255, 255), Color.Black, Color.White); else ThemeHandler.SetColors(this, (4, 160, 219), (255, 255, 255), Color.White, Color.Black);
        }
        private void UpdateGUI()
        {
            titleLabel.Text = "Nothing Playing";
            Text = "FRESHMusicPlayer";
            progressIndicator.Text = "(nothing playing)";
        }
        private void getAlbumArt()
        {
            ATL.Track theTrack = new ATL.Track(Player.filePath);
            IList<ATL.PictureInfo> embeddedPictures = theTrack.EmbeddedPictures;
            Graphics g = albumartBox?.CreateGraphics();
            g?.Clear(pauseplayButton.BackColor); // The background color of the volume bar should be the same as the highlight color of the UI
            albumartBox.Image?.Dispose(); // Clear resources used by the previous image
            foreach (ATL.PictureInfo pic in embeddedPictures)
            {
                albumartBox.Image = Image.FromStream(new System.IO.MemoryStream(pic.PictureData));
            }
        }
        private void fullscreenButton_Click(object sender, EventArgs e)
        {
            
        }
        private void songChangedHandler(object sender, EventArgs e) => UpdateMetadata();

        private void UpdateMetadata()
        {
            ATL.Track metadata = new ATL.Track(Player.filePath);
            titleLabel.Text = $"{metadata.Artist} - {metadata.Title}";
            Text = $"{metadata.Artist} - {metadata.Title} | FRESHMusicPlayer";
            getAlbumArt();
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

        private void infoButton_Click(object sender, EventArgs e)
        {
            SongInfo songInfo = new SongInfo();
            songInfo.ShowDialog();
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (Player.playing & !Player.paused)
            {
                progressIndicator.Text = Player.getSongPosition(true);
            }
            else if (!Player.paused) UpdateGUI();
        }

        private void MiniPlayer_Load(object sender, EventArgs e) => UpdateMetadata();

        private void MiniPlayer_Deactivate(object sender, EventArgs e)
        {
            Opacity = UnfocusedOpacity;
        }

        private void MiniPlayer_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            Player.NextSong();
        }

        private void MiniPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.songChanged -= this.songChangedHandler; // Make sure we subscribe from the song changed event before the form closes (otherwise we'd have resource
                                                           // leaking issues and errors from trying to call things that don't exist)
        }
    }
}
