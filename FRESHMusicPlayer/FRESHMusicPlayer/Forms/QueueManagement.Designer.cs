namespace FRESHMusicPlayer
{
    partial class QueueManagement
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.addPlaylist = new System.Windows.Forms.Button();
            this.addSong = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.clearQueue = new System.Windows.Forms.Button();
            this.next = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 21;
            this.listBox1.Location = new System.Drawing.Point(12, 40);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(460, 256);
            this.listBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Your song queue";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.addPlaylist);
            this.groupBox1.Controls.Add(this.addSong);
            this.groupBox1.Location = new System.Drawing.Point(478, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(158, 117);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add...";
            // 
            // addPlaylist
            // 
            this.addPlaylist.Location = new System.Drawing.Point(6, 68);
            this.addPlaylist.Name = "addPlaylist";
            this.addPlaylist.Size = new System.Drawing.Size(147, 34);
            this.addPlaylist.TabIndex = 1;
            this.addPlaylist.Text = "Playlist File";
            this.addPlaylist.UseVisualStyleBackColor = true;
            this.addPlaylist.Click += new System.EventHandler(this.addPlaylist_Click);
            // 
            // addSong
            // 
            this.addSong.Location = new System.Drawing.Point(6, 28);
            this.addSong.Name = "addSong";
            this.addSong.Size = new System.Drawing.Size(147, 34);
            this.addSong.TabIndex = 0;
            this.addSong.Text = "Song";
            this.addSong.UseVisualStyleBackColor = true;
            this.addSong.Click += new System.EventHandler(this.addSong_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.clearQueue);
            this.groupBox2.Location = new System.Drawing.Point(478, 163);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(158, 72);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remove";
            // 
            // clearQueue
            // 
            this.clearQueue.Location = new System.Drawing.Point(6, 28);
            this.clearQueue.Name = "clearQueue";
            this.clearQueue.Size = new System.Drawing.Size(147, 34);
            this.clearQueue.TabIndex = 0;
            this.clearQueue.Text = "Clear Queue";
            this.clearQueue.UseVisualStyleBackColor = true;
            this.clearQueue.Click += new System.EventHandler(this.clearQueue_Click);
            // 
            // next
            // 
            this.next.Location = new System.Drawing.Point(484, 241);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(147, 34);
            this.next.TabIndex = 1;
            this.next.Text = "Next Song";
            this.next.UseVisualStyleBackColor = true;
            this.next.Click += new System.EventHandler(this.next_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "label2";
            // 
            // QueueManagement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(648, 327);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.next);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "QueueManagement";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Queue Management";
            this.LocationChanged += new System.EventHandler(this.QueueManagement_LocationChanged);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button addPlaylist;
        private System.Windows.Forms.Button addSong;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button clearQueue;
        private System.Windows.Forms.Button next;
        private System.Windows.Forms.Label label2;
    }
}