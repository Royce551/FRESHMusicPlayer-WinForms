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
using ATL;
using ATL.AudioData;
using ATL.CatalogDataReaders;
using ATL.Logging;
using ATL.Playlist;
using ATL.PlaylistReaders;
using System.IO;

namespace FRESHMusicPlayer.Forms
{
    
    public partial class TagEditor : Form
    {
        public List<string> filePaths = new List<string>();
        public Image albumArt;
        public List<(Image coverArt, int width, int height, string format, string type)> coverArt = new List<(Image coverArt, int width, int height, string format, string type)>();
        private List<string> FilePathsToSave = new List<string>();
        public bool UnsavedChanges = false;
        public TagEditor(List<string> filePaths)
        {
            InitializeComponent();
            this.filePaths = filePaths;
            InitFields();
            Player.songChanged += new EventHandler(SongChangedHandler);
            
        }
 
        public void InitFields()
        {                                       // this code probably needs some cleanup
            int iterations = 1;
            foreach (string path in filePaths)
            {
                Track track = new Track(path);
                if (Artist_Box.Text == "") Artist_Box.Text = track.Artist; else Artist_Box.Text = "";
                if (Title_Box.Text == "") Title_Box.Text = track.Title; else Title_Box.Text = "";
                if (Album_Box.Text == "") Album_Box.Text = track.Album; else Album_Box.Text = "";
                if (Genre_Box.Text == "") Genre_Box.Text = track.Genre; else Genre_Box.Text = "";
                if (Year_Box.Text == "") Year_Box.Text = track.Year.ToString(); else Year_Box.Text = "";
                if (AlbumArtist_Box.Text == "") AlbumArtist_Box.Text = track.AlbumArtist; else AlbumArtist_Box.Text = "";
                if (Composer_Box.Text == "") Composer_Box.Text = track.Composer; else Composer_Box.Text = "";
                if (TrackNum_Box.Text == "") TrackNum_Box.Text = track.TrackNumber.ToString(); else TrackNum_Box.Text = "";
                if (DiscNum_Box.Text == "") DiscNum_Box.Text = track.DiscNumber.ToString(); else DiscNum_Box.Text = "";
                IList<PictureInfo> embeddedPictures = track.EmbeddedPictures;
                var pages = 1;
                if (iterations == 1) 
                foreach (PictureInfo picture in embeddedPictures)
                {
                    if (picture == null) continue;
                    Image image = Image.FromStream(new MemoryStream(picture.PictureData));
                    coverArt.Add((image, image.Width, image.Height, new ImageFormatConverter().ConvertToString(image.RawFormat).ToUpper(), picture.PicType.ToString()));
                    PageBox.Items.Add(pages.ToString());
                    PageBox.SelectedIndex = 0;
                    ChangeCoverArt();
                    pages++;
                }
                if (iterations > 1) label4.Visible = true;
                iterations++;
            }
            Editing_Label.Text = $"Editing {string.Join(", ", filePaths)}";
            UnsavedChanges = true;
        }
        public void SaveChanges(List<string> filePaths)
        {
            foreach (string path in filePaths)
            {
                Track track = new Track(path);
                track.Artist = Artist_Box.Text;
                track.Title = Title_Box.Text;
                track.Album = Album_Box.Text;
                track.Genre = Genre_Box.Text;
                track.Year = Convert.ToInt32(Year_Box.Text);
                track.AlbumArtist = AlbumArtist_Box.Text;
                track.Composer = Composer_Box.Text;
                track.TrackNumber = Convert.ToInt32(TrackNum_Box.Text);
                track.DiscNumber = Convert.ToInt32(DiscNum_Box.Text);
                track.Save();
            }
            UnsavedChanges = false;
            UserInterface.LibraryNeedsUpdating = true;
        }
        private void SongChangedHandler(object sender, EventArgs e)
        {
            foreach (string path in FilePathsToSave)
            {
                if (path == Player.filePath) break;
            }
            SaveChanges(FilePathsToSave);
            FilePathsToSave.Clear();
            if (!Visible) Close();
        }
        private void TagEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UnsavedChanges == true)
            {
                var dialog = MessageBox.Show("Do you want to save your changes?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (dialog == DialogResult.Yes) SaveChanges(filePaths);
                else if (dialog == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else return;
            }
            if (FilePathsToSave.Count != 0)
            {
                Hide(); // This will allow the SongChanged handler to fire and actually save the changes
                e.Cancel = true;
            }
            else
            {
                Player.songChanged -= SongChangedHandler;               
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (string path in filePaths)
            {
                if (path != Player.filePath) continue;              // We're good
                else
                {
                    FilePathsToSave.AddRange(filePaths);            // SongChanged event handler will handle this (can't save changes if player is currently playing it)
                    return;                             
                }
            }
            SaveChanges(filePaths);
        }
        private void ChangeCoverArt()
        {
            int SelectedIndex = PageBox.SelectedIndex;
            CoverArt_Box.Image = coverArt[SelectedIndex].coverArt;
            CoverArt_Label.Text = $"{coverArt[SelectedIndex].width}x{coverArt[SelectedIndex].height}\n{new ImageFormatConverter().ConvertToString(coverArt[SelectedIndex].coverArt.RawFormat).ToUpper()} Image\n{coverArt[SelectedIndex].type}";
        }
        private void PageBox_SelectedIndexChanged(object sender, EventArgs e) => ChangeCoverArt();

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var SelectFileDialog = new OpenFileDialog())
            {
                if (SelectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePaths = SelectFileDialog.FileNames.ToList();
                    InitFields();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (string path in filePaths)
            {
                if (path != Player.filePath) continue;              // We're good
                else
                {
                    FilePathsToSave.AddRange(filePaths);            // SongChanged event handler will handle this (can't save changes if player is currently playing it)
                    return;
                }
            }
            SaveChanges(filePaths);
        }

        private void exitTagEditorToolStripMenuItem_Click(object sender, EventArgs e) => Close();

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TagEditor tagEditor = new TagEditor(filePaths);
            tagEditor.Show();
        }
    }
}