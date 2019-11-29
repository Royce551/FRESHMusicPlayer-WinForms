using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ATL;
using ATL.Playlist;

namespace FRESHMusicPlayer
{
    public partial class QueueManagement : Form
    {
        public QueueManagement()
        {
            InitializeComponent();
            PopulateList();
        }
        public void PopulateList()
        {
            var list = Player.GetQueue();
            int number = 1;
            foreach (var song in list)
            {
                Track theTrack = new Track(song);
                listBox1.Items.Add($"{number.ToString()} {theTrack.Artist} - {theTrack.Title}");
                number++;
            }
        }

        private void clearQueue_Click(object sender, EventArgs e)
        {
            Player.ClearQueue();
            listBox1.Items.Clear();
            PopulateList();
        }

        private void addSong_Click(object sender, EventArgs e)
        {
            using (var selectFileDialog = new OpenFileDialog())

            {
                if (selectFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Player.AddQueue(selectFileDialog.FileName);
                    listBox1.Items.Clear();
                    PopulateList();
                }

            }
        }

        private void addPlaylist_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectFileDialog = new OpenFileDialog();
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
                        listBox1.Items.Clear();
                        PopulateList();
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        MessageBox.Show("This playlist file cannot be played because one or more of the songs could not be found.", "Songs not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Player.ClearQueue();
                    }

                }

            }
        }

        private void next_Click(object sender, EventArgs e)
        {

        }
    }
}
