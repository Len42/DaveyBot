using System;
using System.Windows.Forms;

namespace SplitPics
{
	/// <summary>
	/// Program that splits a bitmap of an interlaced video image into two non-interlaced images.
	/// </summary>
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
