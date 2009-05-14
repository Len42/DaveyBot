/*
Copyright 2009 Len Popp

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Windows.Forms;

namespace DaveyBot
{
    static class Program
    {
		// The main components of the program
		private static Eyes s_eyes;
		private static Brain s_brain;
		private static NoteFinder s_noteFinder;
		private static Fingers s_fingers;
		private static FrameGrabber s_frameGrabber;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			// Create all the various objects we need and hook them up.
			s_eyes = new Eyes();
			s_brain = new Brain();
			s_brain.Eyes = s_eyes;
			SetNoteFinder();
			s_fingers = new Fingers();
			s_fingers.Brain = s_brain;
			s_frameGrabber = new FrameGrabber();
			s_frameGrabber.Eyes = s_eyes;
			MainForm form = new MainForm();
			form.Eyes = s_eyes;
			form.Brain = s_brain;
			form.Fingers = s_fingers;
			form.FrameGrabber = s_frameGrabber;

			Application.Run(form);
        }

		/// <summary>
		/// Create a NoteFinder object of the appropriate subclass, as defined in the Settings.
		/// </summary>
		static private void SetNoteFinder()
		{
			s_noteFinder = (NoteFinder)ObjectUtils.CreateObjectFromSettings("NoteFinder", typeof(NoteFinder));
			s_brain.NoteFinder = s_noteFinder;
		}

		/// <summary>
		/// Display a Setup dialog to edit the program's settings.
		/// </summary>
		/// <remarks>
		/// The video settings are updated while the dialog is up.
		/// Other components are updated after the dialog is closed.
		/// </remarks>
		static public void DoSetupDialog()
		{
			DlgSetup dlg = new DlgSetup();
			dlg.Eyes = s_eyes;
			dlg.CommPort = Properties.Settings.Default.ControllerPort;
			dlg.LagAdjust = Properties.Settings.Default.NoteLagAdjust;
			dlg.NoteFinder = Properties.Settings.Default.NoteFinder;
			dlg.DelayStrum = Properties.Settings.Default.DelayStrum;
			dlg.ShowDialog();
			if (dlg.DialogResult == DialogResult.OK)
			{
				// NOTE: The Video Setup dialog takes care of setting the
				// VideoSource and re-building the capture graph if necessary.
				// Save the new setup as the default.
				Properties.Settings.Default.VideoSource = s_eyes.VideoSourceName;
				Properties.Settings.Default.ControllerPort = dlg.CommPort;
				Properties.Settings.Default.NoteLagAdjust = dlg.LagAdjust;
				Properties.Settings.Default.NoteFinder = dlg.NoteFinder;
				Properties.Settings.Default.DelayStrum = dlg.DelayStrum;
				Properties.Settings.Default.Save();
				// Update the objects currently in use.
				s_brain.Stop(); // will be reinitialized when it is restarted
				SetNoteFinder();
			}
			dlg.Dispose();
		}
	}
}
