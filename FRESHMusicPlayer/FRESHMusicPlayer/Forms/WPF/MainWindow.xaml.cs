using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using FRESHMusicPlayer;
using ATL;
namespace FRESHMusicPlayer.Forms.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class WPFUserInterface : Window
    {
        //DispatcherTimer dispatcherTimer;
        Timer timer;
        public WPFUserInterface()
        {
            InitializeComponent();
            Player.songChanged += Player_songChanged;
            Player.songStopped += Player_songStopped;
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += DispatcherTimer_Tick;
            //dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            //dispatcherTimer.Tick += DispatcherTimer_Tick;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            ProgressIndicator.Text = Player.getSongPosition();
            ProgressBar.Value = Player.audioFile.CurrentTime.TotalSeconds;
        }

        private void Player_songStopped(object sender, EventArgs e)
        {
            Title = "FRESHMusicPlayer WPF Test";
            TitleLabel.Text = "Nothing Playing";
            timer.Stop();
        }

        private void Player_songChanged(object sender, EventArgs e)
        {
            Track track = new Track(Player.filePath);
            Title = $"{track.Artist} - {track.Title} | FRESHMusicPlayer WPF Test";
            TitleLabel.Text = $"{track.Artist} - {track.Title}";
            ProgressBar.Maximum = Player.audioFile.TotalTime.TotalSeconds;
            try { CoverArtBox.Source = BitmapFrame.Create(new System.IO.MemoryStream(track.EmbeddedPictures[0].PictureData), BitmapCreateOptions.None, BitmapCacheOption.OnLoad); }
            catch { CoverArtBox.Source = null; }
            timer.Start();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            Player.AddQueue(FilePathBox.Text);
            Player.PlayMusic();
            Player.currentvolume = .3f;
            Player.UpdateSettings();
        }
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Player.StopMusic();
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.audioFile.CurrentTime = TimeSpan.FromSeconds(ProgressBar.Value);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
