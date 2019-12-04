using ATL;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FRESHMusicPlayer
{
    public partial class SongInfo : Form
    {
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
            Graphics g = pictureBox1.CreateGraphics();
            toolTip1.SetToolTip(songtitleText, $"{track.Artist} - {track.Title}");
            toolTip1.SetToolTip(albumText, track.Album);
            foreach (ATL.PictureInfo pic in embeddedPictures)
            {
                pictureBox1.Image = Image.FromStream(new System.IO.MemoryStream(pic.PictureData));
            }
            if (!Properties.Settings.Default.Appearance_BoldText) Font = new Font("Segoe UI", 12, FontStyle.Regular); else Font = new Font("Segoe UI", 12, FontStyle.Bold);
        }

        private void SongInfo_FormClosing(object sender, FormClosingEventArgs e) => pictureBox1.Image?.Dispose();
    }
}
