namespace FRESHMusicPlayer
{
    partial class SongInfo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.songtitleText = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.albumText = new System.Windows.Forms.Label();
            this.genreText = new System.Windows.Forms.Label();
            this.yearText = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.discText = new System.Windows.Forms.Label();
            this.trackText = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bitrateText = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // songtitleText
            // 
            this.songtitleText.AutoSize = true;
            this.songtitleText.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.songtitleText.Location = new System.Drawing.Point(249, 16);
            this.songtitleText.Name = "songtitleText";
            this.songtitleText.Size = new System.Drawing.Size(96, 37);
            this.songtitleText.TabIndex = 1;
            this.songtitleText.Text = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.yearText);
            this.groupBox1.Controls.Add(this.genreText);
            this.groupBox1.Controls.Add(this.albumText);
            this.groupBox1.Location = new System.Drawing.Point(256, 56);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(380, 101);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Song Info";
            // 
            // albumText
            // 
            this.albumText.AutoSize = true;
            this.albumText.Location = new System.Drawing.Point(21, 29);
            this.albumText.Name = "albumText";
            this.albumText.Size = new System.Drawing.Size(57, 21);
            this.albumText.TabIndex = 0;
            this.albumText.Text = "label1";
            // 
            // genreText
            // 
            this.genreText.AutoSize = true;
            this.genreText.Location = new System.Drawing.Point(21, 50);
            this.genreText.Name = "genreText";
            this.genreText.Size = new System.Drawing.Size(57, 21);
            this.genreText.TabIndex = 1;
            this.genreText.Text = "label1";
            // 
            // yearText
            // 
            this.yearText.AutoSize = true;
            this.yearText.Location = new System.Drawing.Point(21, 71);
            this.yearText.Name = "yearText";
            this.yearText.Size = new System.Drawing.Size(57, 21);
            this.yearText.TabIndex = 2;
            this.yearText.Text = "label1";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.discText);
            this.groupBox2.Controls.Add(this.trackText);
            this.groupBox2.Location = new System.Drawing.Point(256, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(380, 81);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Album Info";
            // 
            // discText
            // 
            this.discText.AutoSize = true;
            this.discText.Location = new System.Drawing.Point(21, 50);
            this.discText.Name = "discText";
            this.discText.Size = new System.Drawing.Size(57, 21);
            this.discText.TabIndex = 1;
            this.discText.Text = "label1";
            // 
            // trackText
            // 
            this.trackText.AutoSize = true;
            this.trackText.Location = new System.Drawing.Point(21, 29);
            this.trackText.Name = "trackText";
            this.trackText.Size = new System.Drawing.Size(57, 21);
            this.trackText.TabIndex = 0;
            this.trackText.Text = "label1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.bitrateText);
            this.groupBox3.Location = new System.Drawing.Point(256, 250);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(380, 60);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Technical Info";
            // 
            // bitrateText
            // 
            this.bitrateText.AutoSize = true;
            this.bitrateText.Location = new System.Drawing.Point(21, 29);
            this.bitrateText.Name = "bitrateText";
            this.bitrateText.Size = new System.Drawing.Size(57, 21);
            this.bitrateText.TabIndex = 0;
            this.bitrateText.Text = "label1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 23);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(225, 225);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // SongInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 323);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.songtitleText);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "SongInfo";
            this.Text = "About this song";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SongInfo_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label songtitleText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label yearText;
        private System.Windows.Forms.Label genreText;
        private System.Windows.Forms.Label albumText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label discText;
        private System.Windows.Forms.Label trackText;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label bitrateText;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}