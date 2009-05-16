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
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DirectShowLib;

namespace DaveyBot
{
	/// <summary>
	/// Captures video from an input device and outputs a stream of images as events.
	/// </summary>
	/// <remarks>
	/// <para>The video capture configuration is saved in the program's Settings. Not all
	/// device configurations are supported, but typical video turner crossbar setups
	/// should work.</para>
	/// <para>Uses DirectShow via the DirectShowNET library.</para>
	/// </remarks>
	public class Eyes : ISampleGrabberCB
    {
		/// <summary>Name of the video capture device</summary>
		public string VideoSourceName { get { return m_stVideoSourceName; } set { m_stVideoSourceName = value; } }
		private string m_stVideoSourceName = null;

		/// <summary>The window in which to display the video</summary>
		public Control VideoParentWindow { get { return m_ctrlVideoParent; } set { m_ctrlVideoParent = value; } }
		private Control m_ctrlVideoParent = null;

		/// <summary>True if video capture has been initialized (but not necessarily running)</summary>
		public bool IsInitialized { get { return m_graph != null; } }
		private IGraphBuilder m_graph = null;

		// other DirectShow capture objects
		private IMediaControl m_mediaControl = null;
        private IAMCrossbar m_crossbar = null;
        private IVideoWindow m_videoWindow = null;

		/// <summary>Width of the video image in pixels</summary>
		public int VideoWidth { get { return m_dxVideo; } }
		private int m_dxVideo = 0;

		/// <summary>Height of the video image in pixels</summary>
		public int VideoHeight { get { return m_dyVideo; } }
		private int m_dyVideo = 0;

		/// <summary>Number of bytes per pixel (typically 3)</summary>
		public int PixelDepth { get { return m_cb1Pix; } }
		private int m_cb1Pix = 0;

		/// <summary>
		/// Video image stride, i.e. the number of bytes from the start of
		/// one scan line to the next.
		/// </summary>
		public int VideoStride { get { return m_cbVideoStride; } }
		private int m_cbVideoStride = 0;

		/// <summary>Time interval between video frames, in seconds</summary>
		public double VideoFrameInterval { get { return m_dtVideoFrameSecs; } }
		private double m_dtVideoFrameSecs = 0;

		/// <summary>Is live video currently running?</summary>
		public bool IsVideoPlaying { get { return m_fPlaying; } }
		private bool m_fPlaying = false;

		/// <summary>
		/// Event containing a video frame that has been captured
		/// </summary>
		public event EventHandler<VideoFrameArgs> VideoFrame;
		private void OnVideoFrame(VideoFrameArgs args)
		{
			if (VideoFrame != null)
				VideoFrame(this, args);
		}

		/// <summary>
		/// Event raised when video capture starts
		/// </summary>
		public event EventHandler VideoStarted;
		private void OnVideoStarted()
		{
			if (VideoStarted != null)
				VideoStarted(this, new EventArgs());
		}

		/// <summary>
		/// Event raised when video capture stops
		/// </summary>
		public event EventHandler VideoStopped;
		private void OnVideoStopped()
		{
			if (VideoStopped != null)
				VideoStopped(this, new EventArgs());
		}

		/// <summary>
		/// Convenience function to check for a COM error and throw an exception for it.
		/// </summary>
		/// <param name="hr">COM error code (HRESULT)</param>
        private void Check(int hr)
        {
            DsError.ThrowExceptionForHR(hr);
        }

		/// <summary>
		/// Convenience function for releasing COM objects.
		/// </summary>
		/// <param name="o">COM object</param>
		private void ReleaseCOM(object o)
		{
			if (o != null)
			{
				int cRef = Marshal.ReleaseComObject(o);
				Debug.Assert(cRef == 0, "COM object not fully released " + cRef);
			}
		}

		/// <summary>
		/// Stop video capture and release the DirectShow objects.
		/// </summary>
		public void Shutdown()
		{
			StopVideo();
			DestroyCaptureGraph();
		}

		/// <summary>
		/// Initialize the video capture objects from the saved Settings.
		/// </summary>
		/// <returns>True if the capture settings were loaded,
		/// false if there are no saved settings.</returns>
		public bool LoadVideoSetup()
		{
			bool fDone = false;
			DestroyCaptureGraph();
			m_stVideoSourceName = Properties.Settings.Default.VideoSource;
			if (m_stVideoSourceName == String.Empty)
				m_stVideoSourceName = null;
			if (m_stVideoSourceName != null)
			{
				BuildCaptureGraph();
				// Load crossbar settings if available, to select the video input.
				if (m_crossbar != null)
				{
					int cPinOut;
					int cPinIn;
					int iPinIn;
					int iPinOut;
					string stProp;
					Check(m_crossbar.get_PinCounts(out cPinOut, out cPinIn));
					for (iPinOut = 0; iPinOut < cPinOut; iPinOut++)
					{
						stProp = "VideoInput" + iPinOut;
						iPinIn = (int)Properties.Settings.Default[stProp];
						if (iPinIn >= 0)
						{
							Check(m_crossbar.Route(iPinOut, iPinIn));
						}
					}
				}
				fDone = true;
			}
			return fDone;
		}

		/// <summary>
		/// Add the names of all the available video capture devices
		/// to the given list.
		/// </summary>
		/// <param name="list">List of names to be filled</param>
		public void FillCaptureSourceList(IList list)
		{
			list.Clear();
			foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
				list.Add(ds.Name);
		}

		/// <summary>
		/// Display a DirectShow crossbar dialog to select a video input.
		/// </summary>
		/// <param name="wndParent">Parent window for the dialog (optional)</param>
		public void DoCrossbarDialog(Form wndParent)
		{
			// If there's a crossbar object, display its property page.
			// That's where the user can select the correct video input.
			if (m_crossbar != null)
			{
				DisplayPropertyPage(m_crossbar, wndParent);

				// After the dialog, get the crossbar routings and save them.
				int cpinOut;
				int cpinIn;
				int iPinOut;
				int iPinIn;
				string stProp;
				Check(m_crossbar.get_PinCounts(out cpinOut, out cpinIn));
				for (iPinOut = 0; iPinOut < cpinOut; iPinOut++)
				{
					Check(m_crossbar.get_IsRoutedTo(iPinOut, out iPinIn));
					stProp = "VideoInput" + iPinOut;
					Properties.Settings.Default[stProp] = iPinIn;
				}
			}
		}

		/// <summary>
		/// Display a DirectShow object's properties dialog.
		/// </summary>
		/// <param name="oFilter">DirectShow object</param>
		/// <param name="wndParent">Parent window for the dialog (optional)</param>
		private void DisplayPropertyPage(object oFilter, Form wndParent)
		{
			IBaseFilter bfFilter = null;
			ISpecifyPropertyPages pPropPages = null;
			IAMVfwCompressDialogs dlgCompress = null;
			FilterInfo filterInfo = new FilterInfo { achName = null, pGraph = null };
			DsCAUUID caGUID = new DsCAUUID { cElems = 0, pElems = IntPtr.Zero };
			IntPtr hwndParent;
			try
			{
				pPropPages = oFilter as ISpecifyPropertyPages;
				if (pPropPages != null)
				{
					bfFilter = (IBaseFilter)oFilter;
					Check(bfFilter.QueryFilterInfo(out filterInfo));
					Check(pPropPages.GetPages(out caGUID));
					hwndParent = (wndParent == null) ? IntPtr.Zero : wndParent.Handle;
					Check(OleCreatePropertyFrame(hwndParent, 0, 0, filterInfo.achName,
												 1, ref oFilter, caGUID.cElems, caGUID.pElems,
												 0, 0, IntPtr.Zero));
				}
				else
				{
					// If the filter doesn't implement ISpecifyPropertyPages, look for IAMVfwCompressDialogs instead.
					dlgCompress = oFilter as IAMVfwCompressDialogs;
					if (dlgCompress != null)
					{

						Check(dlgCompress.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero));
					}
					else
					{
						MessageBox.Show("Item has no property page", "No Property Page", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}
			finally
			{
				//ReleaseCOM(bfFilter); - no, this messes up oFilter
				ReleaseCOM(pPropPages);
				ReleaseCOM(dlgCompress);
				ReleaseCOM(filterInfo.pGraph);
				if (caGUID.pElems != IntPtr.Zero)
					Marshal.FreeCoTaskMem(caGUID.pElems);
			}
		}

		// OleCreatePropertyFrame declaration
		[DllImport("olepro32.dll")]
		internal static extern int OleCreatePropertyFrame(
			IntPtr hwndOwner,
			int x,
			int y,
			[MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
			int cObjects,
			[MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
            ref object ppUnk,
			int cPages,
			IntPtr lpPageClsID,
			int lcid,
			int dwReserved,
			IntPtr lpvReserved);

		/// <summary>
		/// Create the DirectShow capture graph for video capture.
		/// </summary>
		public void BuildCaptureGraph()
		{
            object o = null;
			ICaptureGraphBuilder2 captureGraphBuilder = null;
            IBasicVideo2 basicVideo2 = null;
			IBaseFilter bfVideoSource = null;
            AMMediaType mediaType = null;
            ISampleGrabber grabber = null;
            IBaseFilter bfGrabber = null;
			//IBaseFilter bfOutputFile = null;
			//IFileSinkFilter fsfOutputFile = null;

			if (m_stVideoSourceName == null || m_stVideoSourceName.Length == 0)
				throw new InvalidOperationException("No capture device has been selected.");

			Cursor.Current = Cursors.WaitCursor;

			DestroyCaptureGraph();

            try
            {
                // Create the capture graph.
                m_graph = (IGraphBuilder)new FilterGraph();
                captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                Check(captureGraphBuilder.SetFiltergraph(m_graph));
                basicVideo2 = (IBasicVideo2)m_graph;

                // Create the video input device and add it to the graph.
                bfVideoSource = CreateFilter(FilterCategory.VideoInputDevice, m_stVideoSourceName);
                Check(m_graph.AddFilter(bfVideoSource, "video source"));

                // Create the sample grabber and add it to the graph.
                grabber = (ISampleGrabber)new SampleGrabber();
                bfGrabber = (IBaseFilter)grabber;
                Check(grabber.SetCallback(this, 1));
                // Set the media type to Video/RBG24
                mediaType = new AMMediaType();
                mediaType.majorType = MediaType.Video;
                mediaType.subType = MediaSubType.RGB24;
                mediaType.formatType = FormatType.VideoInfo;
                Check(grabber.SetMediaType(mediaType));
                DsUtils.FreeAMMediaType(mediaType);
                mediaType = null;
                Check(m_graph.AddFilter(bfGrabber, "sample grabber"));

/*
				// TODO: make this optional
				// Set up the video output file and hook it up to the Capture pin of the video source.
				string stFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				stFile = System.IO.Path.Combine(stFile, "DaveyBot.avi");
				Check(captureGraphBuilder.SetOutputFileName(MediaSubType.Avi, stFile, out bfOutputFile, out fsfOutputFile));
				Check(captureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, bfVideoSource, null, bfOutputFile));
*/

                // Render any preview pin of the device through the SampleGrabber.
                // NOTE: Use PinCategory.Preview instead of PinCategory.Capture
                // for *both* preview & capturing because preview video is choppy
                // if we use PinCategory.Capture for previewing; and we want to
                // display the images that we are processing.
				// The images seen by the sample grabber are the same quality in either case
				// (at least with the devices tested so far).
                Check(captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, bfVideoSource, bfGrabber, null));

                // Now that the grabber is hooked up, get the size of the captured images.
                Debug.Assert(mediaType == null, "mediaType was not freed");
                mediaType = new AMMediaType();
                Check(grabber.GetConnectedMediaType(mediaType));
                if ((mediaType.formatType != FormatType.VideoInfo) || (mediaType.formatPtr == IntPtr.Zero))
                {
                    throw new NotSupportedException("Unknown media type from sample grabber");
                }
                VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
                m_dxVideo = videoInfoHeader.BmiHeader.Width;
                m_dyVideo = videoInfoHeader.BmiHeader.Height;
                m_cb1Pix = videoInfoHeader.BmiHeader.BitCount / 8;
                m_cbVideoStride = m_dxVideo * m_cb1Pix;
				Check(basicVideo2.get_AvgTimePerFrame(out m_dtVideoFrameSecs));

                // Also, we can now get the crossbar, which will be needed to select the video input.
                int hr = captureGraphBuilder.FindInterface(null, null, bfVideoSource, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    m_crossbar = (IAMCrossbar)o;
                    o = null;
                }

                // Get the video display window.
                // It won't be visible until ShowVideoWindow is called.
                m_videoWindow = (IVideoWindow)m_graph;

                // Get the media control for controlling the graph.
                m_mediaControl = (IMediaControl)m_graph;

            }
            catch
            {
                // In case of error shut everything down
                try
                {
                    DestroyCaptureGraph();
                }
                catch
                {
                }
                throw;
            }
			finally
			{
				// DirectShowNET objects have to be explicitly released.
				ReleaseCOM(captureGraphBuilder);
                //ReleaseCOM(basicVideo2); - no, this messes up m_graph
				ReleaseCOM(bfVideoSource);
				ReleaseCOM(grabber);
				//ReleaseCOM(bfGrabber);
				if (mediaType != null)
					DsUtils.FreeAMMediaType(mediaType);
				Cursor.Current = Cursors.Default;
			}
		}

		/// <summary>
		/// Destroy the DirectShow video capture graph.
		/// </summary>
		private void DestroyCaptureGraph()
		{
            StopVideo();
			ReleaseCOM(m_graph);
			m_graph = null;
			ReleaseCOM(m_crossbar);
            m_crossbar = null;
			//ReleaseCOM(m_videoWindow);
            m_videoWindow = null;
			//ReleaseCOM(m_mediaControl);
            m_mediaControl = null;
            m_dxVideo = 0;
            m_dyVideo = 0;
            m_cb1Pix = 0;
            m_cbVideoStride = 0;
		}

		/// <summary>
		/// Function to create a DirectShow filter object.
		/// </summary>
		/// <param name="idCategory">Type of filter</param>
		/// <param name="stName">Name of filter</param>
		/// <returns>DirectShow filter</returns>
		private IBaseFilter CreateFilter(Guid idCategory, string stName)
		{
			object oFilter = null;
			Guid iidBF = typeof(IBaseFilter).GUID;
			foreach (DsDevice device in DsDevice.GetDevicesOfCat(idCategory))
			{
				if (device.Name == stName)
				{
					device.Mon.BindToObject(null, null, ref iidBF, out oFilter);
					return (IBaseFilter)oFilter;
				}
			}
			// not found
			throw new ArgumentException("Device not found: \"" + stName + '"');
		}

		/// <summary>
		/// Display the live video in the VideoParent window.
		/// </summary>
        private void SetupVideoWindow()
		{
			if (m_videoWindow == null)
			{
				throw new InvalidOperationException("Video window does not exist");
			}
			else if (m_ctrlVideoParent == null)
			{
				throw new InvalidOperationException("Video parent window does not exist");
			}
			else
			{
				Check(m_videoWindow.put_Owner(m_ctrlVideoParent.Handle));
				Check(m_videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren));
				SetVideoWindowPos();
				Check(m_videoWindow.put_Visible(OABool.True));
			}
		}

		/// <summary>
		/// Set the position and size of the DirectShow video window so
		/// the video image fits in the VideoParent window.
		/// </summary>
		public void SetVideoWindowPos()
		{
			if (m_videoWindow != null)
			{
				int dx = m_ctrlVideoParent.Width;
				int dy = m_ctrlVideoParent.Height;
				if (dx > m_dxVideo && dy > m_dyVideo)
				{
					// Full-size video image fits in the window.
					dx = m_dxVideo;
					dy = m_dyVideo;
				}
				else if (dx > 0 && dy > 0 && m_dxVideo > 0 && m_dyVideo > 0)
				{
					// Preserve the aspect ratio of the video image.
					if ((long)m_dxVideo * dy > (long)dx * m_dyVideo)
						dy = (int)(((long)dx * m_dyVideo) / m_dxVideo);
					else
						dx = (int)(((long)m_dxVideo * dy) / m_dyVideo);
				}

				// Size the video window to fill its parent window.
				Check(m_videoWindow.SetWindowPosition(0, 0, dx, dy));
			}
		}

		/// <summary>
		/// Start capturing and displaying live video.
		/// </summary>
		public void PlayVideo()
		{
			if (m_mediaControl == null)
			{
				throw new InvalidOperationException("Video capture is not initialized");
			}
			else if (!m_fPlaying)
			{
				SetupVideoWindow();
				m_mediaControl.Run();
				if (m_videoWindow != null)
					Check(m_videoWindow.put_Visible(OABool.True));
				m_fPlaying = true;
				OnVideoStarted();
			}
		}

		/// <summary>
		/// Stop video capture and display.
		/// </summary>
		public void StopVideo()
		{
			if (m_fPlaying)
			{
				m_fPlaying = false;
				if (m_videoWindow != null)
					Check(m_videoWindow.put_Visible(OABool.False));
				if (m_mediaControl != null)
					m_mediaControl.Stop();
				OnVideoStopped();
			}
		}

		//
		// ISampleGrabberCB interface
		//

		/// <summary>
		/// Not used.
		/// </summary>
		/// <param name="SampleTime"></param>
		/// <param name="pSample"></param>
		/// <returns></returns>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
			ReleaseCOM(pSample);
            return 0;
        }

		/// <summary>
		/// Process each video frame that is captured.
		/// See <see cref="ISampleGrabberCB"/>.
		/// </summary>
		/// <param name="tSample">Timestamp of the video frame.
		/// NOTE: This value appears to be garbage.</param>
		/// <param name="buf">Bitmap video image, as a raw byte buffer</param>
		/// <param name="cbBuf">Size of image buffer</param>
		/// <returns>S_OK</returns>
        public int BufferCB(double tSample, IntPtr buf, int cbBuf)
        {
			// Raise an event for others to process the frame.
			VideoFrameArgs args = new VideoFrameArgs(tSample, buf, cbBuf);
			OnVideoFrame(args);
			return 0;
        }
    }
}
