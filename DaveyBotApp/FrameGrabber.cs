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
using System.IO;
using System.Runtime.InteropServices;

namespace DaveyBot
{
	/// <summary>
	/// Captures a sequence of video frames from the <see cref="DaveyBot.Eyes"/>
	/// and writes them out as bitmap files, for diagnostic purposes.
	/// </summary>
	public class FrameGrabber
	{
		/// <summary>The video capture object supplying the images</summary>
		public Eyes Eyes { get { return m_eyes; } set { m_eyes = value; } }
		private Eyes m_eyes = null;

		/// <summary>The number of video frames to be grabbed</summary>
		public int NumFramesToGrab { get { return m_cframeAlloc; } set { m_cframeAlloc = value; } }
		private int m_cframeAlloc = 30;

		/// <summary>The number of video frames that have been grabbed so far</summary>
		public int NumFramesGrabbed { get { return m_cframe; } }
		private int m_cframe = 0;

		/// <summary>The grabbed video frames</summary>
		public VideoFrameGrab[] Frames { get { return m_rgframe; } }
		private VideoFrameGrab[] m_rgframe = null;

		/// <summary>Lock object to protect the collection of video frames</summary>
		private object oLockFrames = new Object();

		/// <summary>Are we currently grabbing video frames?</summary>
		public bool IsCapturing { get { return m_fIsCapturing; } }
		private bool m_fIsCapturing = false;

		/// <summary>Event raised when frame grabbing ends.</summary>
		public event EventHandler Stopped;
		private void OnStopped()
		{
			if (Stopped != null)
				Stopped(this, new EventArgs());
		}

		/// <summary>
		/// Start grabbing a consecutive sequence of video frames.
		/// </summary>
		/// <remarks>
		/// The video frames are saved in an array. They will be written to files
		/// later with a separate method call.
		/// </remarks>
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
			else if (!m_fIsCapturing)
			{
				lock (oLockFrames)
				{
					m_rgframe = new VideoFrameGrab[m_cframeAlloc];
					m_cframe = 0;
				}
				// Listen for Eyes events.
				m_eyes.VideoFrame += eyes_VideoFrame;
				m_eyes.VideoStopped += eyes_VideoStopped;
				m_fIsCapturing = true;
			}
		}

		/// <summary>
		/// Stop grabbing video frames.
		/// </summary>
		public void Stop()
		{
			if (m_fIsCapturing)
			{
				m_fIsCapturing = false;
				m_eyes.VideoFrame -= eyes_VideoFrame;
				m_eyes.VideoStopped -= eyes_VideoStopped;
				OnStopped();
			}
		}

		/// <summary>
		/// Discard the saved video frames.
		/// </summary>
		public void Clear()
		{
			lock (oLockFrames)
			{
				m_rgframe = null;
				m_cframe = 0;
			}
		}

		/// <summary>
		/// Event handler to grab video frames as they come in.
		/// </summary>
		/// <remarks>
		/// The images are saved in an array and will be written to files later.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void eyes_VideoFrame(object sender, VideoImage args)
		{
			if (m_fIsCapturing)
			{
				if (m_cframe >= m_cframeAlloc)
				{
					Stop();
				}
				else
				{
					VideoFrameGrab frame = new VideoFrameGrab(args);
					lock (oLockFrames)
						m_rgframe[m_cframe++] = frame;
				}
			}
		}

		/// <summary>
		/// Event handler to stop frame grabber if video capture stops.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void eyes_VideoStopped(object sender, EventArgs args)
		{
			Stop();
		}

		/// <summary>
		/// Write the previously-saved video frames to bitmap files in the given directory.
		/// </summary>
		/// <param name="stSubDir">Name of sub-directory under "My Pictures" in which
		/// the image files will be written</param>
		public void WriteFiles(string stSubDir)
		{
			// This is time-consuming, so we will not lock m_rgframe for the whole operation.
			// Instead we will grab m_rgframe and set it to null, then release the lock.
			VideoFrameGrab[] rgframe = null;
			int cframe = 0;
			lock (oLockFrames)
			{
				rgframe = m_rgframe;
				cframe = m_cframe;
				Clear();
			}
			if (cframe > 0 && rgframe != null)
			{
				// Create a sub-directory under "My Pictures" for the image files.
				string stDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				stDir = Path.Combine(stDir, stSubDir);
				Directory.CreateDirectory(stDir);
				// Create a summary text file.
				string stPath = Path.Combine(stDir, "frames.txt");
				StreamWriter fileInfo = File.CreateText(stPath);
				// Write out all captured images to separate files.
				for (int i = 0; i < cframe; i++)
				{
					VideoFrameGrab frame = rgframe[i];
					// Convert the raw pixels to a proper bitmap.
					// TODO: PixelFormat - for now, it's assumed
					IntPtr hPixels = Marshal.AllocHGlobal(frame.NumBytes);
					Marshal.Copy(frame.Pixels, 0, hPixels, frame.NumBytes);
					Bitmap bmp = new Bitmap(frame.Width, frame.Height, frame.Stride,
											System.Drawing.Imaging.PixelFormat.Format24bppRgb,
											hPixels);
					// Bitmap needs to be flipped top/bottom.
					bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
					// Write the bitmap to a file.
					string stFile = i.ToString() + ".bmp";
					stPath = Path.Combine(stDir, stFile);
					bmp.Save(stPath, System.Drawing.Imaging.ImageFormat.Bmp);
					Marshal.FreeHGlobal(hPixels);
					bmp.Dispose();
					// Write info about the video frame.
					fileInfo.Write(i);
					fileInfo.Write(',');
					fileInfo.Write(frame.SampleTime);
					fileInfo.Write(',');
					fileInfo.WriteLine(stFile);
				}
				fileInfo.Close();
			}
		}
	}
}
