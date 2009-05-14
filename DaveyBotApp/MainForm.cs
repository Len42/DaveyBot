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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DaveyBot
{
	/// <summary>
	/// The main application window, with controls and live video display.
	/// </summary>
    public partial class MainForm : Form
    {
		/// <summary>The video capture object</summary>
		public Eyes Eyes { get { return m_eyes; } set { m_eyes = value; } }
		private Eyes m_eyes;

		/// <summary>
		/// The object performing note detection and analysis
		/// </summary>
		public Brain Brain
		{
			get { return m_brain; }
			set
			{
				if (m_brain != null)
					m_brain.PlayNotes -= brain_PlayNotes;
				m_brain = value;
				if (m_brain != null)
					m_brain.PlayNotes += brain_PlayNotes;
			}
		}
		private Brain m_brain;

		/// <summary>
		/// The object that plays the notes and interfaces with the Xbox 360 controller
		/// </summary>
		public Fingers Fingers
		{
			get { return m_fingers; }
			set
			{
				if (m_fingers != null)
					m_fingers.PlayNotes -= fingers_PlayNotes;
				m_fingers = value;
				if (m_fingers != null)
					m_fingers.PlayNotes += fingers_PlayNotes;
			}

		}
		private Fingers m_fingers;

		/// <summary>
		/// Object to write a sequence of video frames to files, for diagnostic purposes.
		/// </summary>
		public FrameGrabber FrameGrabber
		{
			get { return m_frameGrabber; }
			set
			{
				if (m_frameGrabber != null)
					m_frameGrabber.Stopped -= frameGrabber_Stopped;
				m_frameGrabber = value;
				if (m_frameGrabber != null)
					m_frameGrabber.Stopped += frameGrabber_Stopped;
			}
		}
		private FrameGrabber m_frameGrabber;

		/// <summary>Are the Brain and Fingers playing the game?</summary>
		private bool m_fPlayingGame = false;

		// "Lights" to indicate button presses & strums
		// These ones are from the Brain.
		// They light up when notes are detected.
		bool m_fGreen = false;
		bool m_fRed = false;
		bool m_fYellow = false;
		bool m_fBlue = false;
		bool m_fOrange = false;
		bool m_fStrum = false;
		// These ones are from the Fingers.
		// They light up when the notes are actually played.
		bool m_fGreen2 = false;
		bool m_fRed2 = false;
		bool m_fYellow2 = false;
		bool m_fBlue2 = false;
		bool m_fOrange2 = false;
		bool m_fStrum2 = false;
		// These are the positions of the various lights.
		const int xPaintLeft = 12;
		const int yPaintTop = 40;
		const int dxPaint = 18;
		const int dyPaint = 18 / 2;
		const int yPaintTop2 = yPaintTop + dyPaint;
		const int dxSpace = 8;
		const int xPaintGreen = xPaintLeft;
		const int xPaintRed = xPaintGreen + dxPaint + dxSpace;
		const int xPaintYellow = xPaintRed + dxPaint + dxSpace;
		const int xPaintBlue = xPaintYellow + dxPaint + dxSpace;
		const int xPaintOrange = xPaintBlue + dxPaint + dxSpace;
		const int xPaintStrum = xPaintOrange + dxPaint + dxSpace;
		const int xPaintRight = xPaintStrum + dxPaint;
		Rectangle rectPaint = new Rectangle(xPaintLeft, yPaintTop, xPaintRight - xPaintLeft, dyPaint);
		Rectangle rectPaint2 = new Rectangle(xPaintLeft, yPaintTop2, xPaintRight - xPaintLeft, dyPaint);
		// Coloured brushes.
		Brush brGreen = new SolidBrush(Color.Green);
		Brush brRed = new SolidBrush(Color.Red);
		Brush brYellow = new SolidBrush(Color.Yellow);
		Brush brBlue = new SolidBrush(Color.Blue);
		Brush brOrange = new SolidBrush(Color.Orange);
		Brush brWhite = new SolidBrush(Color.White);
		Brush brBlack = new SolidBrush(Color.Black);

        public MainForm()
        {
            InitializeComponent();
		}

		/// <summary>
		/// Event handler for form initialization.
		/// </summary>
		/// <remarks>
		/// Time-consuming video initialization is not done now.
		/// It's delayed by a one-shot timer, so the form can display itself first.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_Load(object sender, EventArgs e)
		{
			SetAllButtonStates();
			timerInit.Start();
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			m_fingers.Stop();
			m_brain.Stop();
			m_eyes.Shutdown();
		}

		/// <summary>
		/// One-shot timer to defer slow initialization until the form has appeared.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerInit_Tick(object sender, EventArgs e)
		{
			timerInit.Stop();
			Initialize();
		}

		/// <summary>
		/// Start the video capture when the form opens.
		/// </summary>
		private void Initialize()
		{
			m_eyes.VideoParentWindow = panelVideo;
			// Start video using the saved settings if any.
			if (m_eyes.LoadVideoSetup())
			{
				m_eyes.PlayVideo();
			}
			else
			{
				// No (valid) saved video settings, so bring up the Setup dialog.
				// (Actually, no, don't.)
				//DoVideoSetupDialog();
			}
			SetAllButtonStates();
		}

		/// <summary>
		/// Paint event handler to draw activity monitor lights.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MainForm_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (m_brain.IsRunning)
			{
				if (m_fGreen)
					g.FillRectangle(brGreen, xPaintGreen, yPaintTop, dxPaint, dyPaint);
				if (m_fRed)
					g.FillRectangle(brRed, xPaintRed, yPaintTop, dxPaint, dyPaint);
				if (m_fYellow)
					g.FillRectangle(brYellow, xPaintYellow, yPaintTop, dxPaint, dyPaint);
				if (m_fBlue)
					g.FillRectangle(brBlue, xPaintBlue, yPaintTop, dxPaint, dyPaint);
				if (m_fOrange)
					g.FillRectangle(brOrange, xPaintOrange, yPaintTop, dxPaint, dyPaint);
				g.FillRectangle(m_fStrum ? brWhite : brBlack, xPaintStrum, yPaintTop, dxPaint, dyPaint);
			}
			if (m_fingers.IsRunning)
			{
				if (m_fGreen2)
					g.FillRectangle(brGreen, xPaintGreen, yPaintTop2, dxPaint, dyPaint);
				if (m_fRed2)
					g.FillRectangle(brRed, xPaintRed, yPaintTop2, dxPaint, dyPaint);
				if (m_fYellow2)
					g.FillRectangle(brYellow, xPaintYellow, yPaintTop2, dxPaint, dyPaint);
				if (m_fBlue2)
					g.FillRectangle(brBlue, xPaintBlue, yPaintTop2, dxPaint, dyPaint);
				if (m_fOrange2)
					g.FillRectangle(brOrange, xPaintOrange, yPaintTop2, dxPaint, dyPaint);
				g.FillRectangle(m_fStrum2 ? brWhite : brBlack, xPaintStrum, yPaintTop2, dxPaint, dyPaint);
			}
		}

		/// <summary>
		/// Event handler to display notes when they are detected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void brain_PlayNotes(object sender, PlayNotesArgs args)
		{
			m_fGreen = args.PressGreen;
			m_fRed = args.PressRed;
			m_fYellow = args.PressYellow;
			m_fBlue = args.PressBlue;
			m_fOrange = args.PressOrange;
			// Toggle the "strum" box only if the strum bar is pressed.
			if (args.Strum)
				m_fStrum = !m_fStrum;
			Invalidate(rectPaint);
		}

		/// <summary>
		/// Event handler to display notes when they are played.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void fingers_PlayNotes(object sender, PlayNotesArgs args)
		{
			m_fGreen2 = args.PressGreen;
			m_fRed2 = args.PressRed;
			m_fYellow2 = args.PressYellow;
			m_fBlue2 = args.PressBlue;
			m_fOrange2 = args.PressOrange;
			// Toggle the "strum" box only if the strum bar is pressed.
			if (args.Strum)
				m_fStrum2 = !m_fStrum2;
			Invalidate(rectPaint2);
		}

		/// <summary>
		/// When the video display control is resized, resize the live video display to match.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void panelVideo_Layout(object sender, LayoutEventArgs e)
		{
			m_eyes.SetVideoWindowPos();
		}

		/// <summary>
		/// Update button states and labels to reflect the program's current state.
		/// </summary>
		private void SetAllButtonStates()
		{
			SetVideoButtonState();
			SetPlayButtonState();
			SetGrabButtonState();
		}

		/// <summary>
		/// Update the Start/Stop Video button.
		/// </summary>
		private void SetVideoButtonState()
		{
			bool fStart;
			bool fEnabled;
			if (!m_eyes.IsInitialized)
			{
				fEnabled = false;
				fStart = true;
			}
			else
			{
				fEnabled = true;
				fStart = !m_eyes.IsVideoPlaying;
			}
			btnStartVideo.Enabled = fEnabled;
			btnStartVideo.Text = fStart ? "Start &Video" : "Stop &Video";
		}

		/// <summary>
		/// Update the Play Game button.
		/// </summary>
		private void SetPlayButtonState()
		{
			bool fPlay;
			bool fEnabled;
			if (!m_eyes.IsVideoPlaying)
			{
				fPlay = true;
				fEnabled = false;
			}
			else if (!m_brain.IsRunning)
			{
				fPlay = true;
				fEnabled = true;
			}
			else
			{
				fPlay = false;
				fEnabled = true;
			}
			btnPlay.Enabled = fEnabled;
			btnPlay.Text = fPlay ? "&Play Game" : "Stop &Playing";
		}

		/// <summary>
		/// Update the Grab Frames button.
		/// </summary>
		private void SetGrabButtonState()
		{
			bool fGrab;
			bool fEnabled;
			if (!m_frameGrabber.IsCapturing)
			{
				fGrab = true;
				fEnabled = true;
			}
			else
			{
				fGrab = false;
				fEnabled = true;
			}
			btnGrabFrames.Enabled = fEnabled;
			btnGrabFrames.Text = fGrab ? "&Grab Frames" : "Stop &Grabbing";
		}

		/// <summary>
		/// Do I really have to explain what this does?
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnQuit_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Button handler to display the Setup dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnSetup_Click(object sender, EventArgs e)
		{
			Program.DoSetupDialog();
			SetAllButtonStates();
		}

		/// <summary>
		/// Button handler to start or stop the video capture.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnStartVideo_Click(object sender, EventArgs e)
		{
			if (!m_eyes.IsVideoPlaying)
			{
				m_eyes.PlayVideo();
			}
			else
			{
				if (m_fPlayingGame)
					StopPlaying();
				m_eyes.StopVideo();
			}
			SetAllButtonStates();
		}

		/// <summary>
		/// Button handler to start or stop playing the game.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnPlay_Click(object sender, EventArgs e)
		{
			if (!m_fPlayingGame)
			{
				if (m_eyes.IsVideoPlaying)
				{
					m_fingers.Start();
					m_brain.Start();
					m_fPlayingGame = true;
				}
			}
			else
			{
				StopPlaying();
			}
			Invalidate(rectPaint);
			Invalidate(rectPaint2);
			SetAllButtonStates();
		}

		/// <summary>
		/// Stop playing the game.
		/// </summary>
		private void StopPlaying()
		{
			if (m_fPlayingGame)
			{
				m_fingers.Stop();
				m_brain.Stop();
				// Erase the activity lights.
				m_fGreen = false;
				m_fRed = false;
				m_fYellow = false;
				m_fBlue = false;
				m_fOrange = false;
				m_fStrum = false;
				m_fGreen2 = false;
				m_fRed2 = false;
				m_fYellow2 = false;
				m_fBlue2 = false;
				m_fOrange2 = false;
				m_fStrum2 = false;
				m_fPlayingGame = false;
			}
			Invalidate(rectPaint);
			Invalidate(rectPaint2);
			SetAllButtonStates();
		}

		/// <summary>
		/// Button handler to start/stop the frame grabber.
		/// </summary>
		/// <remarks>
		/// A sequence of video frames will be captured, and when the
		/// <see cref="DaveyBot.FrameGrabber"/> is done they will be written to files.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnGrabFrames_Click(object sender, EventArgs e)
		{
			if (!m_frameGrabber.IsCapturing)
			{
				// TODO: Make the number of frames configurable.
				m_frameGrabber.NumFramesToGrab = 300;
				m_frameGrabber.Start();
			}
			else
			{
				m_frameGrabber.Stop();
			}
			SetAllButtonStates();
		}

		/// <summary>
		/// Event handler - When the <see cref="DaveyBot.FrameGrabber"/> stops,
		/// write the captured images to files.
		/// </summary>
		/// <remarks>
		/// The files are written in a background thread (using a BackgroundWorker)
		/// because it's a slow process and would interfere with the video capture.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void frameGrabber_Stopped(object sender, EventArgs args)
		{
			// Dump the captured frames to files.
			// Do this in the background because it's slow and would interfere
			// with the video capture.
			SetGrabButtonState();
			bgFrameWriter.RunWorkerAsync();
		}

		/// <summary>
		/// See <see cref="frameGrabber_Stopped"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bgFrameWriter_DoWork(object sender, DoWorkEventArgs e)
		{
			m_frameGrabber.WriteFiles("DaveyBot");
		}

		/// <summary>
		/// See <see cref="frameGrabber_Stopped"/>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bgFrameWriter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			MessageBox.Show("Image files have been written.");
		}

		/// <summary>
		/// Button handler to analyze one image from a file and display the result.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAnalyzeFrame_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			//dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			dlg.Filter = "Bitmap files|*.bmp|All files|*.*";
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				AnalyzeState state = m_brain.AnalyzeFrameFile(dlg.FileName);
				state.ShowDialog(null);
			}
		}

		/// <summary>
		/// Button handler to analyze a set of image files and write the results to a file.
		/// </summary>
		/// <remarks>
		/// This will analyze a set of files that were written by the <see cref="DaveyBot.FrameGrabber"/>.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnAnalyzeAllFiles_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
				m_brain.AnalyzeFrameFiles(dlg.SelectedPath);
			}
		}
    }
}
