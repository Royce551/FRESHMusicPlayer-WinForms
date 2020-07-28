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
using FRESHMusicPlayer.Handlers;
namespace FRESHMusicPlayer
{
    public partial class QueueManagement : Form
    {
        public QueueManagement()
        {
            InitializeComponent();
            PopulateList();
            Player.songChanged += new EventHandler(this.songChangedHandler);
            if (Properties.Settings.Default.Appearance_DarkMode) ThemeHandler.SetColors(this, (44, 47, 51), (255, 255, 255), Color.Black, Color.White);
        }   
        public void PopulateList()
        {
            var list = Player.GetQueue();
            var nextlength = 0;
            int number = 1;
            foreach (var song in list)
            {
                string place;
                if (Player.QueuePosition == number) place = "NOW PLAYING: ";
                else if (Player.QueuePosition == number - 1) place = "UP NEXT: ";
                else place = (number - Player.QueuePosition).ToString();
                Track theTrack = new Track(song);
                listBox1.Items.Add($"{place} {theTrack.Artist} - {theTrack.Title}");
                if (Player.QueuePosition < number) nextlength += theTrack.Duration;
                number++;
            }
            label2.Text = $"Remaining Time - {Format(nextlength)}";
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
            Player.NextSong();
            listBox1.Items.Clear();
            PopulateList();
        }
        private void previous_Click(object sender, EventArgs e)
        {
            Player.PreviousSong();
            listBox1.Items.Clear();
            PopulateList();
        }

        string Format(int secs)
        {
            int days = 0;
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
            
            while (hours >= 24)
            {
                days++;
                hours -= 24;
            }
            string dayStr = days.ToString(); if (dayStr.Length < 2) dayStr = "0" + dayStr;
            string hourStr = hours.ToString(); if (hourStr.Length < 2) hourStr = "0" + hourStr;
            string minStr = mins.ToString(); if (minStr.Length < 2) minStr = "0" + minStr;
            string secStr = secs.ToString(); if (secStr.Length < 2) secStr = "0" + secStr;

            string durStr = "";
            if (dayStr != "00") durStr += dayStr + ":";
            durStr = hourStr + ":" + minStr + ":" + secStr;

            return durStr;
        }

        private void QueueManagement_LocationChanged(object sender, EventArgs e)
        {
            //Owner.Location = Location; // This will be needed later, but not now.
        }

        private void QueueManagement_FormClosing(object sender, FormClosingEventArgs e)
        {
            Player.songChanged -= this.songChangedHandler; // Make sure we subscribe from the song changed event before the form closes (otherwise we'd have resource
                                                           // leaking issues and errors from trying to call things that don't exist)
        }
        private void songChangedHandler(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            PopulateList();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Player.QueuePosition = listBox1.SelectedIndex;
            Player.PlayMusic();
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e) => contextMenuStrip1.Show(listBox1, e.Location);
    }
}
