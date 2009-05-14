namespace DaveyBot
{
    partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.panelVideo = new System.Windows.Forms.Panel();
			this.btnSetup = new System.Windows.Forms.Button();
			this.btnQuit = new System.Windows.Forms.Button();
			this.btnStartVideo = new System.Windows.Forms.Button();
			this.timerInit = new System.Windows.Forms.Timer(this.components);
			this.btnPlay = new System.Windows.Forms.Button();
			this.btnGrabFrames = new System.Windows.Forms.Button();
			this.btnAnalyzeFrame = new System.Windows.Forms.Button();
			this.bgFrameWriter = new System.ComponentModel.BackgroundWorker();
			this.btnAnalyzeAllFiles = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// panelVideo
			// 
			this.panelVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelVideo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panelVideo.BackColor = System.Drawing.SystemColors.Control;
			this.panelVideo.Location = new System.Drawing.Point(0, 64);
			this.panelVideo.Name = "panelVideo";
			this.panelVideo.Size = new System.Drawing.Size(632, 367);
			this.panelVideo.TabIndex = 7;
			this.panelVideo.Layout += new System.Windows.Forms.LayoutEventHandler(this.panelVideo_Layout);
			// 
			// btnSetup
			// 
			this.btnSetup.Location = new System.Drawing.Point(12, 12);
			this.btnSetup.Name = "btnSetup";
			this.btnSetup.Size = new System.Drawing.Size(75, 23);
			this.btnSetup.TabIndex = 0;
			this.btnSetup.Text = "&Setup";
			this.btnSetup.UseVisualStyleBackColor = true;
			this.btnSetup.Click += new System.EventHandler(this.btnSetup_Click);
			// 
			// btnQuit
			// 
			this.btnQuit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQuit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnQuit.Location = new System.Drawing.Point(545, 12);
			this.btnQuit.Name = "btnQuit";
			this.btnQuit.Size = new System.Drawing.Size(75, 23);
			this.btnQuit.TabIndex = 6;
			this.btnQuit.Text = "&Quit";
			this.btnQuit.UseVisualStyleBackColor = true;
			this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
			// 
			// btnStartVideo
			// 
			this.btnStartVideo.Location = new System.Drawing.Point(93, 12);
			this.btnStartVideo.Name = "btnStartVideo";
			this.btnStartVideo.Size = new System.Drawing.Size(75, 23);
			this.btnStartVideo.TabIndex = 1;
			this.btnStartVideo.Text = "Start &Video";
			this.btnStartVideo.UseVisualStyleBackColor = true;
			this.btnStartVideo.Click += new System.EventHandler(this.btnStartVideo_Click);
			// 
			// timerInit
			// 
			this.timerInit.Tick += new System.EventHandler(this.timerInit_Tick);
			// 
			// btnPlay
			// 
			this.btnPlay.Location = new System.Drawing.Point(174, 12);
			this.btnPlay.Name = "btnPlay";
			this.btnPlay.Size = new System.Drawing.Size(75, 23);
			this.btnPlay.TabIndex = 2;
			this.btnPlay.Text = "&Play Game";
			this.btnPlay.UseVisualStyleBackColor = true;
			this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
			// 
			// btnGrabFrames
			// 
			this.btnGrabFrames.Location = new System.Drawing.Point(278, 12);
			this.btnGrabFrames.Name = "btnGrabFrames";
			this.btnGrabFrames.Size = new System.Drawing.Size(75, 23);
			this.btnGrabFrames.TabIndex = 3;
			this.btnGrabFrames.Text = "&Grab Frames";
			this.btnGrabFrames.UseVisualStyleBackColor = true;
			this.btnGrabFrames.Click += new System.EventHandler(this.btnGrabFrames_Click);
			// 
			// btnAnalyzeFrame
			// 
			this.btnAnalyzeFrame.Location = new System.Drawing.Point(359, 12);
			this.btnAnalyzeFrame.Name = "btnAnalyzeFrame";
			this.btnAnalyzeFrame.Size = new System.Drawing.Size(81, 23);
			this.btnAnalyzeFrame.TabIndex = 4;
			this.btnAnalyzeFrame.Text = "Check &Frame";
			this.btnAnalyzeFrame.UseVisualStyleBackColor = true;
			this.btnAnalyzeFrame.Click += new System.EventHandler(this.btnAnalyzeFrame_Click);
			// 
			// bgFrameWriter
			// 
			this.bgFrameWriter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgFrameWriter_DoWork);
			this.bgFrameWriter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgFrameWriter_RunWorkerCompleted);
			// 
			// btnAnalyzeAllFiles
			// 
			this.btnAnalyzeAllFiles.Location = new System.Drawing.Point(446, 12);
			this.btnAnalyzeAllFiles.Name = "btnAnalyzeAllFiles";
			this.btnAnalyzeAllFiles.Size = new System.Drawing.Size(75, 23);
			this.btnAnalyzeAllFiles.TabIndex = 5;
			this.btnAnalyzeAllFiles.Text = "&Analyze Files";
			this.btnAnalyzeAllFiles.UseVisualStyleBackColor = true;
			this.btnAnalyzeAllFiles.Click += new System.EventHandler(this.btnAnalyzeAllFiles_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnQuit;
			this.ClientSize = new System.Drawing.Size(632, 431);
			this.Controls.Add(this.btnAnalyzeAllFiles);
			this.Controls.Add(this.btnAnalyzeFrame);
			this.Controls.Add(this.btnGrabFrames);
			this.Controls.Add(this.btnPlay);
			this.Controls.Add(this.btnStartVideo);
			this.Controls.Add(this.btnQuit);
			this.Controls.Add(this.btnSetup);
			this.Controls.Add(this.panelVideo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "DaveyBot";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVideo;
        private System.Windows.Forms.Button btnSetup;
        private System.Windows.Forms.Button btnQuit;
        private System.Windows.Forms.Button btnStartVideo;
		private System.Windows.Forms.Timer timerInit;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnGrabFrames;
		private System.Windows.Forms.Button btnAnalyzeFrame;
		private System.ComponentModel.BackgroundWorker bgFrameWriter;
		private System.Windows.Forms.Button btnAnalyzeAllFiles;
    }
}

