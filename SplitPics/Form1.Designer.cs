namespace SplitPics
{
	partial class Form1
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
			this.btnSplitPicFile = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnSplitAllPicFiles = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnSplitPicFile
			// 
			this.btnSplitPicFile.Location = new System.Drawing.Point(12, 12);
			this.btnSplitPicFile.Name = "btnSplitPicFile";
			this.btnSplitPicFile.Size = new System.Drawing.Size(90, 23);
			this.btnSplitPicFile.TabIndex = 0;
			this.btnSplitPicFile.Text = "&Split Pic File...";
			this.btnSplitPicFile.UseVisualStyleBackColor = true;
			this.btnSplitPicFile.Click += new System.EventHandler(this.btnSplitPicFile_Click);
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(12, 101);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 2;
			this.btnClose.Text = "&Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// btnSplitAllPicFiles
			// 
			this.btnSplitAllPicFiles.Location = new System.Drawing.Point(12, 56);
			this.btnSplitAllPicFiles.Name = "btnSplitAllPicFiles";
			this.btnSplitAllPicFiles.Size = new System.Drawing.Size(90, 23);
			this.btnSplitAllPicFiles.TabIndex = 1;
			this.btnSplitAllPicFiles.Text = "Split Pic &Files...";
			this.btnSplitAllPicFiles.UseVisualStyleBackColor = true;
			this.btnSplitAllPicFiles.Click += new System.EventHandler(this.btnSplitAllPicFiles_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(219, 202);
			this.Controls.Add(this.btnSplitAllPicFiles);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnSplitPicFile);
			this.Name = "Form1";
			this.Text = "Split Pic Files";
			this.Click += new System.EventHandler(this.btnSplitAllPicFiles_Click);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSplitPicFile;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.Button btnSplitAllPicFiles;
	}
}

