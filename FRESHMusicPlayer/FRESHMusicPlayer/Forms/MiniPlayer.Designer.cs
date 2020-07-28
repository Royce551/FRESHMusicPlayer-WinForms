﻿namespace FRESHMusicPlayer
{
    partial class MiniPlayer
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.progressIndicator = new System.Windows.Forms.Label();
            this.pauseplayButton = new System.Windows.Forms.Button();
            this.albumartBox = new System.Windows.Forms.PictureBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.infoButton = new System.Windows.Forms.Button();
            this.fullscreenButton = new System.Windows.Forms.Button();
            this.progressTimer = new System.Windows.Forms.Timer(this.components);
            this.nextButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.fadeIn = new System.Windows.Forms.Timer(this.components);
            this.fadeOut = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.albumartBox)).BeginInit();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.titleLabel.Location = new System.Drawing.Point(87, 6);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(136, 21);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Nothing Playing";
            this.titleLabel.UseMnemonic = false;
            // 
            // progressIndicator
            // 
            this.progressIndicator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.progressIndicator.AutoSize = true;
            this.progressIndicator.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressIndicator.ForeColor = System.Drawing.SystemColors.ControlText;
            this.progressIndicator.Location = new System.Drawing.Point(88, 27);
            this.progressIndicator.Name = "progressIndicator";
            this.progressIndicator.Size = new System.Drawing.Size(106, 17);
            this.progressIndicator.TabIndex = 4;
            this.progressIndicator.Text = "(nothing playing)";
            // 
            // pauseplayButton
            // 
            this.pauseplayButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseplayButton.Image = global::FRESHMusicPlayer.Properties.Resources.baseline_pause_black_18dp;
            this.pauseplayButton.Location = new System.Drawing.Point(223, 70);
            this.pauseplayButton.Name = "pauseplayButton";
            this.pauseplayButton.Size = new System.Drawing.Size(41, 43);
            this.pauseplayButton.TabIndex = 9;
            this.pauseplayButton.UseVisualStyleBackColor = true;
            this.pauseplayButton.Click += new System.EventHandler(this.pauseplayButton_Click);
            // 
            // albumartBox
            // 
            this.albumartBox.Location = new System.Drawing.Point(11, 6);
            this.albumartBox.Name = "albumartBox";
            this.albumartBox.Size = new System.Drawing.Size(70, 70);
            this.albumartBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.albumartBox.TabIndex = 8;
            this.albumartBox.TabStop = false;
            // 
            // stopButton
            // 
            this.stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.stopButton.Image = global::FRESHMusicPlayer.Properties.Resources.baseline_stop_black_18dp;
            this.stopButton.Location = new System.Drawing.Point(270, 70);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(41, 43);
            this.stopButton.TabIndex = 10;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // infoButton
            // 
            this.infoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.infoButton.Image = global::FRESHMusicPlayer.Properties.Resources.baseline_info_black_18dp;
            this.infoButton.Location = new System.Drawing.Point(317, 70);
            this.infoButton.Name = "infoButton";
            this.infoButton.Size = new System.Drawing.Size(41, 43);
            this.infoButton.TabIndex = 11;
            this.infoButton.UseVisualStyleBackColor = true;
            this.infoButton.Click += new System.EventHandler(this.infoButton_Click);
            // 
            // fullscreenButton
            // 
            this.fullscreenButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.fullscreenButton.Location = new System.Drawing.Point(11, 83);
            this.fullscreenButton.Name = "fullscreenButton";
            this.fullscreenButton.Size = new System.Drawing.Size(70, 30);
            this.fullscreenButton.TabIndex = 12;
            this.fullscreenButton.Text = "Exit";
            this.fullscreenButton.UseVisualStyleBackColor = true;
            this.fullscreenButton.Click += new System.EventHandler(this.fullscreenButton_Click);
            // 
            // progressTimer
            // 
            this.progressTimer.Enabled = true;
            this.progressTimer.Interval = 1000;
            this.progressTimer.Tick += new System.EventHandler(this.progressTimer_Tick);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(160, 83);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(55, 30);
            this.nextButton.TabIndex = 13;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // backButton
            // 
            this.backButton.Location = new System.Drawing.Point(99, 83);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(55, 30);
            this.backButton.TabIndex = 14;
            this.backButton.Text = "Prev.";
            this.backButton.UseVisualStyleBackColor = true;
            // 
            // fadeIn
            // 
            this.fadeIn.Interval = 5;
            this.fadeIn.Tick += new System.EventHandler(this.fadeIn_Tick);
            // 
            // fadeOut
            // 
            this.fadeOut.Interval = 5;
            this.fadeOut.Tick += new System.EventHandler(this.fadeOut_Tick);
            // 
            // MiniPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.fullscreenButton;
            this.ClientSize = new System.Drawing.Size(360, 117);
            this.Controls.Add(this.backButton);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.fullscreenButton);
            this.Controls.Add(this.infoButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.pauseplayButton);
            this.Controls.Add(this.albumartBox);
            this.Controls.Add(this.progressIndicator);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MiniPlayer";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MiniPlayer";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.MiniPlayer_Activated);
            this.Deactivate += new System.EventHandler(this.MiniPlayer_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MiniPlayer_FormClosing);
            this.Load += new System.EventHandler(this.MiniPlayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.albumartBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label progressIndicator;
        private System.Windows.Forms.PictureBox albumartBox;
        private System.Windows.Forms.Button pauseplayButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button infoButton;
        private System.Windows.Forms.Button fullscreenButton;
        private System.Windows.Forms.Timer progressTimer;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Timer fadeIn;
        private System.Windows.Forms.Timer fadeOut;
    }
}