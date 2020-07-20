using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
namespace FRESHMusicPlayer.Forms
{
    public partial class Notification : Form
    {
        Stopwatch stopwatch = new Stopwatch();
        private int Lifespan;

        public Notification(string Heading, string Body, int Lifespan)
        {
            InitializeComponent();
            label1.Text = Heading;
            label2.Text = Body;
            Text = Heading;
            this.Lifespan = Lifespan;
            fadeIn.Enabled = true;
            stopwatch.Start();
        }
        public void ManualFadeOut()
        {
            fadeIn.Enabled = false;
            fadeOut.Enabled = true;
        }
        private void fadeIn_Tick(object sender, EventArgs e)
        {
            Opacity += 0.05f;
            if (Opacity == 1.00)
            {
                fadeIn.Enabled = false;
                fadeOut.Enabled = true;
            }     
        }

        private void fadeOut_Tick(object sender, EventArgs e)
        {
            if (stopwatch.ElapsedMilliseconds >= Lifespan)
            {
                Opacity -= 0.03f;
                if (Opacity == 0f)
                {
                    Close();
                    Dispose();
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
