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
            int length = 0;
            foreach (var song in list)
            {
                string place;
                if (number != 1) place = number.ToString(); // If it isn't the first song, put its place in the queue
                else place = "UP NEXT:"; // If it is, put "UP NEXT"
                Track theTrack = new Track(song);
                listBox1.Items.Add($"{place} {theTrack.Artist} - {theTrack.Title}");
                length += theTrack.Duration;
                number++;
            }
            label2.Text = $"Queue Length - {Format(length)}";
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
    }
}
