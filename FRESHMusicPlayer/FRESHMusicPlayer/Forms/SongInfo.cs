using ATL;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using FRESHMusicPlayer.Handlers;
using System.IO;

namespace FRESHMusicPlayer
{
    public partial class SongInfo : Form
    {
        Image albumArt;
        public SongInfo()
        {
            InitializeComponent();
            Track track = new Track(Player.filePath);
            songtitleText.Text = $"{track.Artist} - {track.Title}";
            albumText.Text = $"Album - {track.Album}";
            genreText.Text = $"Genre - {track.Genre}";
            yearText.Text = $"Year - {track.Year.ToString()}";
            trackText.Text = $"Track #{track.TrackNumber}";
            discText.Text = $"Disc #{track.DiscNumber}";
            bitrateText.Text = $"Bitrate - {track.Bitrate.ToString()}kbps";
            IList<ATL.PictureInfo> embeddedPictures = track.EmbeddedPictures;       
            toolTip1.SetToolTip(songtitleText, $"{track.Artist} - {track.Title}");
            toolTip1.SetToolTip(albumText, track.Album);
            if (embeddedPictures.Count != 0)
            {
                albumArt = Image.FromStream(new MemoryStream(embeddedPictures[0].PictureData));
                pictureBox1.Image = albumArt;
            }
            else
            {
                albumArt = null;
                pictureBox1.Image = albumArt;
            }
            if (Properties.Settings.Default.Appearance_DarkMode) ThemeHandler.SetColors(this, (44, 47, 51), (255, 255, 255), Color.Black, Color.White); else ThemeHandler.SetColors(this, (4, 160, 219), (255, 255, 255), Color.White, Color.Black);
        }

        private void SongInfo_FormClosing(object sender, FormClosingEventArgs e) => pictureBox1.Dispose();

        private void pictureBox1_Click(object sender, System.EventArgs e)
        {
            Track track = new Track(Player.filePath);
            IList<ATL.PictureInfo> embeddedPictures = track.EmbeddedPictures;
            foreach (PictureInfo pic in embeddedPictures)
            {
                Image x = Image.FromStream(new MemoryStream(pic.PictureData));
                x.Save(Path.GetTempPath() + "FMPalbumart.png", ImageFormat.Png);
                System.Diagnostics.Process.Start(Path.GetTempPath() + "FMPalbumart.png");
            }

              
        }
    }
}
1