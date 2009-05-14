using System;
using System.Windows.Forms;

namespace SplitPics
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btnSplitPicFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Bitmap files|*.bmp|All files|*.*";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				SplitPic.Split(dlg.FileName);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnSplitAllPicFiles_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				SplitPic.SplitFiles(dlg.SelectedPath);
			}
		}
	}
}
