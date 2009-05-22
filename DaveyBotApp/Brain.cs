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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DaveyBot
{
	/// <summary>
	/// Analyzes the stream of video images, finds the notes, and decides when to play them.
	/// </summary>
	/// <remarks>
	/// Receives events from Eyes and sends events to Fingers.
	/// </remarks>
	public class Brain
	{
		// Colours for superimposed rectangles that indicate notes
		private static Color clrYes = Color.FromArgb(0, 255, 0);
		private static Color clrNo = Color.Black;

		/// <summary>
		/// The video capture object supplying the images to be analyzed
		/// </summary>
		public Eyes Eyes
		{
			get { return m_eyes; }
			set
			{
				// Hook up the event to receive the video frames.
				if (m_eyes != null)
					m_eyes.VideoFrame -= eyes_VideoFrame;
				m_eyes = value;
				if (m_eyes != null)
					m_eyes.VideoFrame += eyes_VideoFrame;
			}
		}
		private Eyes m_eyes;

		/// <summary>
		/// The algorithm to use to find the notes
		/// </summary>
		/// <remarks>
		/// There are several algorithms available, for testing purposes and also to support
		/// different games (Rock Band, Guitar Hero).
		/// </remarks>
		public NoteFinder NoteFinder { get { return m_noteFinder; } set { m_noteFinder = value; } }
		private NoteFinder m_noteFinder;

		/// <summary>Is the note analysis currently running?</summary>
		public bool IsRunning { get { return m_fIsRunning; } set { m_fIsRunning = value; } }
		private bool m_fIsRunning = false;

		/// <summary>Current state of the note analysis.</summary>
		public AnalyzeState CurrentState { get { return m_state; } }
		private AnalyzeState m_state = new AnalyzeState();

		/// <summary>Delay between detecting a note and playing it.</summary>
		private TimeSpan m_dtNoteDelay;

		/// <summary>
		/// If DelayStrum is true, after a note is detected, wait until the next frame
		/// before hitting the strum button. 
		/// </summary>
		/// <remarks>
		/// Sometimes notes that are supposed to be simultaneous aren't detected at exactly
		/// the same time. In that case the strum button mustn't be hit until all the note
		/// buttons are pressed. DelayStrum introduces a one-frame delay to allow for that.
		/// </remarks>
		private bool m_fDelayStrum = false;

		/// <summary>
		/// If true, the strum button should be pressed when the next frame is processed,
		/// whether or not any new notes are detected.
		/// </summary>
		/// <seealso cref="m_fDelayStrum"/>
		private bool m_fStrumPending = false;

		/// <summary>
		/// Event that signals a note or notes are to be played.
		/// </summary>
		/// <remarks>
		/// This event indicates that notes have been detected, but they must be
		/// scheduled to be played at a later time.
		/// </remarks>
		public event EventHandler<PlayNotesArgs> PlayNotes;
		private void OnPlayNotes(bool fPressGreen, bool fPressRed, bool fPressYellow, bool fPressBlue, bool fPressOrange,
								 bool fStrum, DateTime tWhen)
		{
			if (PlayNotes != null)
				PlayNotes(this, new PlayNotesArgs(fPressGreen, fPressRed, fPressYellow, fPressBlue, fPressOrange, fStrum, tWhen));
		}

		/// <summary>
		/// Begin analyzing and playing notes.
		/// </summary>
		public void Start()
		{
			if (m_eyes == null)
			{
				throw new InvalidOperationException("Video capture is not initialized");
			}
			else if (!m_eyes.IsVideoPlaying)
			{
				throw new InvalidOperationException("Video is not running");
			}
			else if (!m_fIsRunning)
			{
				// Figure out the delay between the detection of a note and when to play it.
				// This includes the delay defined by the note detection algorithm
				// and a delay defined by the user.
				// NOTE: NoteLagAdjust is subtracted, not added, so it will appear as a
				// positive number in the Settings dialog.
				m_dtNoteDelay = new TimeSpan(m_eyes.VideoFrameInterval.Ticks * m_noteFinder.NumFramesDelay
									- Properties.Settings.Default.NoteLagAdjust * TimeSpan.TicksPerMillisecond);
				m_fDelayStrum = Properties.Settings.Default.DelayStrum;
				m_fStrumPending = false;
				m_state = new AnalyzeState();
				m_fIsRunning = true;
			}
		}

		/// <summary>
		/// Stop analyzing and playing notes.
		/// </summary>
		public void Stop()
		{
			m_fIsRunning = false;
		}

		/// <summary>
		/// Event handler that processes video frames.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="frame"></param>
		protected void eyes_VideoFrame(object sender, VideoImage frame)
		{
			if (m_fIsRunning)
			{
				VideoImage frameSub0;
				VideoImage frameSub1;
				frame.Deinterlace(Eyes.VideoFrameInterval, out frameSub0, out frameSub1);
				HandleSubFrame(frameSub0);
				HandleSubFrame(frameSub1);
			}
		}

		/// <summary>
		/// Process one de-interlaced sub-frame of video.
		/// </summary>
		/// <remarks>
		/// This method will be called twice for each interlaced video frame
		/// that is received from <see cref="DaveyBot.Eyes"/>.
		/// </remarks>
		/// <param name="frame">Video image bitmap</param>
		private void HandleSubFrame(VideoImage frame)
		{
			// Analyze the video frame to see if any notes have hit.
			AnalyzeFrame(frame, m_state);

			// DEBUG: Mark detected notes on the live video frame.
			Color clrFill;
			clrFill = m_state.Green.Found ? clrYes : clrNo;
			FillRect(234 - 12, 24, 364 + 24, 10, clrFill, frame);
			clrFill = m_state.Red.Found ? clrYes : clrNo;
			FillRect(297 - 12, 24, 364 + 24, 10, clrFill, frame);
			clrFill = m_state.Yellow.Found ? clrYes : clrNo;
			FillRect(360 - 12, 24, 364 + 24, 10, clrFill, frame);
			clrFill = m_state.Blue.Found ? clrYes : clrNo;
			FillRect(423 - 12, 24, 364 + 24, 10, clrFill, frame);
			clrFill = m_state.Orange.Found ? clrYes : clrNo;
			FillRect(482 - 12, 24, 364 + 24, 10, clrFill, frame);

			// Figure out which buttons to press based on the analysis.
			ExecuteActions(frame.SampleTime);
		}

		/// <summary>
		/// Analyze one frame (bitmap) of video from the game to find notes.
		/// </summary>
		/// <remarks>
		/// Calls a <see cref="DaveyBot.NoteFinder"/> object that implements the particular note-detection algorithm.
		/// </remarks>
		/// <param name="frame">Video image bitmap</param>
		/// <param name="state">Current analysis state</param>
		private void AnalyzeFrame(VideoImage frame, AnalyzeState state)
		{
			state.InitNextFrame();
			m_noteFinder.AnalyzeImage(frame, state);
		}

		/// <summary>
		/// Figure out which buttons to press, and raise events to do so.
		/// </summary>
		/// <remarks>
		/// <para>Decide what to do based on the current state.
		/// Signal those actions by sending events which will be handled by the Fingers class.</para>
		///<para>If new notes were found, play them. But there's a twist:
		/// If DelayStrum is true, wait one frame before playing the note.
		/// That's because sometimes there are multiple notes, but some of them
		/// are detected one frame later than others. We need to play them simultaneously.</para>
		/// <para>If no notes were detected, don't send an event. That means that after
		/// a note button is pressed, it will stay pressed until the next note appears.
		/// Thus sustained notes are held without having to explictly detect them.</para>
		/// </remarks>
		/// <todo>overdrive/star power; whammy bar</todo>
		/// <param name="tFrame">Timestamp of the video frame</param>
		private void ExecuteActions(DateTime tFrame)
		{
			bool fPlayNote = false;
			if (m_fStrumPending)
			{
				// We detected a note last time around, but delayed one frame to give a chance
				// for other notes to be detected.
				// Combine the notes detected previously with those found now, and play them.
				m_state.Green.Found |= m_state.Green.PrevFound;
				m_state.Red.Found |= m_state.Red.PrevFound;
				m_state.Yellow.Found |= m_state.Yellow.PrevFound;
				m_state.Blue.Found |= m_state.Blue.PrevFound;
				m_state.Orange.Found |= m_state.Orange.PrevFound;
				fPlayNote = true;
				m_fStrumPending = false;
			}
			else if (m_state.Green.TurnedOn
					|| m_state.Red.TurnedOn
					|| m_state.Yellow.TurnedOn
					|| m_state.Blue.TurnedOn
					|| m_state.Orange.TurnedOn)
			{
				// At least one *new* note was found. Play it/them.
				if (m_fDelayStrum)
				{
					// But not yet! We want to wait until the next frame before playing
					// this note, in case there's another one that we haven't noticed yet.
					m_fStrumPending = true;
				}
				else
				{
					// Play the note(s) now.
					fPlayNote = true;
				}
			}

			if (fPlayNote)
			{
				// It's time to strum a note or chord.
				// Raise an event, which will be handled by Fingers.
				DateTime tWhen = tFrame + m_dtNoteDelay;
				OnPlayNotes(m_state.Green.Found,
						m_state.Red.Found,
						m_state.Yellow.Found,
						m_state.Blue.Found,
						m_state.Orange.Found,
						m_state.Strum,
						tWhen);
			}
			// TODO: overdrive, whammy bar
		}

		/// <summary>
		/// Splat a coloured rectangle onto a bitmap.
		/// </summary>
		/// <remarks>
		/// This is used to flash rectangles onto the video image stream to indicate
		/// notes that are detected.
		/// </remarks>
		/// <param name="x">Rectangle left position</param>
		/// <param name="dx">Rectangle width</param>
		/// <param name="y">Rectangle top position</param>
		/// <param name="dy">Rectangle height</param>
		/// <param name="clr">Rectangle colour</param>
		/// <param name="frame">Video image bitmap</param>
		private unsafe void FillRect(int x, int dx, int y, int dy, Color clr, VideoImage frame)
		{
			byte* pbBuf = (byte*)frame.ImageData;
			byte R = clr.R; // cache these values because they're used in loops
			byte G = clr.G;
			byte B = clr.B;

			if (frame.Width <= 0 || frame.Height <= 0 || frame.BytesPerPixel <= 0 || frame.Stride <= 0)
			{
				// Error - video sizes not initialized
				// (don't throw an exception for these errors because they may repeat on every video frame)
			}
			else if (x < 0 || x + dx >= frame.Width || y < 0 || y + dy >= frame.Height)
			{
				// Error - invalid rectangle coords
			}
			else if (frame.BytesPerPixel != 3)
			{
				// Error - we can't handle this video format
			}
			else
			{
				// Flip top & bottom
				y = frame.Height - y - dy;
				int ibCur;
				int ibStart;
				int ibEnd;
				int cb1Pix = frame.BytesPerPixel;
				int cbStride = frame.Stride;
				ibStart = frame.Start + y * frame.Stride + x * cb1Pix;
				for (int i = 0; i < dy; i++)
				{
					ibEnd = ibStart + dx * cb1Pix;
					for (ibCur = ibStart; ibCur < ibEnd; ibCur += cb1Pix)
					{
						// NOTE: Pixel byte order is B, G, R
						pbBuf[ibCur] = B;
						pbBuf[ibCur + 1] = G;
						pbBuf[ibCur + 2] = R;
					}
					ibStart += cbStride;
				}
			}
		}

		/// <summary>
		/// Analyze a single image from a file.
		/// </summary>
		/// <param name="stFileBmp">Image file name</param>
		/// <returns>Object describing what was found in the image</returns>
		public AnalyzeState AnalyzeFrameFile(string stFileBmp)
		{

			// NOTE: Use a new, blank state.
			AnalyzeState state = new AnalyzeState();
			AnalyzeFrameFileHelper(stFileBmp, state);
			return state;
		}

		/// <summary>
		/// Analyze all image files in a directory and write out a summary of the results.
		/// </summary>
		/// <remarks>
		/// This method looks for numbered files that were created by FrameGrabber - 0.bmp, 1.bmp, etc.
		/// It outputs the results to the file analysis.txt in the same directory.
		/// </remarks>
		/// <seealso cref="FrameGrabber"/>
		/// <param name="stDir">Directory containing the image files</param>
		public void AnalyzeFrameFiles(string stDir)
		{
			// Analyze a bunch of frames that were saved in files by FrameGrabber.
			// Iterate through numbered files until we run out.
			// Write state info for all the frames to a summary file.
			AnalyzeState state = new AnalyzeState();
			string stFile = "analysis.txt";
			stFile = Path.Combine(stDir, stFile);
			StreamWriter fileResults = new StreamWriter(stFile);
			fileResults.Write("Num,Strum,");
			fileResults.Write("GreenFound,GreenPrevFound,GreenRValue,GreenGValue,GreenBValue,");
			fileResults.Write("RedFound,RedPrevFound,RedRValue,RedGValue,RedBValue,");
			fileResults.Write("YellowFound,YellowPrevFound,YellowRValue,YellowGValue,YellowBValue,");
			fileResults.Write("BlueFound,BluePrevFound,BlueRValue,BlueGValue,BlueBValue,");
			fileResults.Write("OrangeFound,OrangePrevFound,OrangeRValue,OrangeGValue,OrangeBValue");
			fileResults.WriteLine();
			for (int i = 0; ; i++)
			{
				// Analyze the next image file.
				stFile = i.ToString() + ".bmp";
				stFile = Path.Combine(stDir, stFile);
				if (!File.Exists(stFile))
					break;
				AnalyzeFrameFileHelper(stFile, state);
				// Write info to summary file.
				fileResults.Write(i);
				fileResults.Write(',');
				fileResults.Write(state.Strum ? "True" : "");
				fileResults.Write(',');
				fileResults.Write(state.Green.AsCSVString());
				fileResults.Write(',');
				fileResults.Write(state.Red.AsCSVString());
				fileResults.Write(',');
				fileResults.Write(state.Yellow.AsCSVString());
				fileResults.Write(',');
				fileResults.Write(state.Blue.AsCSVString());
				fileResults.Write(',');
				fileResults.Write(state.Orange.AsCSVString());
				fileResults.WriteLine();
			}
			fileResults.Close();
		}

		/// <summary>
		/// Subroutine that calls <see cref="AnalyzeFrame"/> using an image file.
		/// </summary>
		/// <param name="stFileBmp">Image file</param>
		/// <param name="state">Analysis state (input/output)</param>
		private void AnalyzeFrameFileHelper(string stFileBmp, AnalyzeState state)
		{
			Bitmap bmp = new Bitmap(stFileBmp);
			// Bitmap needs to be flipped top/bottom because we flipped it
			// when we wrote it out.
			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
			Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
			BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			// NOTE: we only support Format24bppRgb
			if (data.PixelFormat != PixelFormat.Format24bppRgb)
				throw new ApplicationException("Unsupported bitmap file format");
			VideoImage image = new VideoImage(DateTime.Now, data.Width, data.Height, 3, data.Stride, 0, data.Scan0,
											  data.Height * data.Stride);
			// Analyze the image.
			AnalyzeFrame(image, state);
			bmp.UnlockBits(data);
			bmp.Dispose();
		}
	}
}
