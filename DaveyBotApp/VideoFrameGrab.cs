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
using System.Runtime.InteropServices;

namespace DaveyBot
{
	/// <summary>
	/// Stores a video image bitmap for the <see cref="FrameGrabber"/>.
	/// </summary>
	/// <remarks>
	/// Unlike <see cref="VideoImage"/>, this class copies the bitmap data into
	/// its own byte[].
	/// </remarks>
	public struct VideoFrameGrab
	{
		public VideoFrameGrab(VideoImage image)
			: this(image.SampleTime, image.Width, image.Height,
					image.BytesPerPixel, image.Stride, image.ImageData, image.NumBytes)
		{
		}

		public VideoFrameGrab(DateTime tSample, int dx, int dy, int cb1Pix, int cbStride, IntPtr buf, int cbBuf)
		{
			m_tSample = tSample;
			m_dx = dx;
			m_dy = dy;
			m_cb1Pix = cb1Pix;
			m_cbStride = cbStride;
			System.Diagnostics.Debug.Assert(dx * dy * 3 == cbBuf);
			m_pbPixels = new byte[cbBuf];
			Marshal.Copy(buf, m_pbPixels, 0, cbBuf);
		}

		/// <summary>
		/// Create a VideoFrameGrab bitmap from a <see cref="VideoImage"/> bitmap,
		/// which may represent half of an interlaced video frame.
		/// </summary>
		/// <todo></todo>
		/// <param name="image"></param>
		/// <returns></returns>
		public static unsafe VideoFrameGrab CreateDeInterlaced(VideoImage image)
		{
			VideoFrameGrab frameGrab = new VideoFrameGrab();
			frameGrab.m_tSample = image.SampleTime;
			frameGrab.m_dx = image.Width;
			frameGrab.m_dy = image.Height;
			frameGrab.m_cb1Pix = image.BytesPerPixel;
			int cbRow = frameGrab.m_dx * frameGrab.m_cb1Pix;
			frameGrab.m_cbStride = cbRow; // de-interlacing
			int cbPixels = frameGrab.m_dx * frameGrab.m_dy * frameGrab.m_cb1Pix;
			frameGrab.m_pbPixels = new byte[cbPixels];

			int ibDst;
			int ibDstStart;
			int ibDstEnd;
			byte* pbSrc = (byte*)image.ImageData;
			int ibSrc;
			int ibSrcStart;
			int ibSrcEnd;
			ibSrcStart = image.Start;
			ibDstStart = 0;
			for (int i = 0; i < frameGrab.m_dy; i++)
			{
				ibSrcEnd = ibSrcStart + cbRow;
				ibDstEnd = ibDstStart + cbRow;
				ibSrc = ibSrcStart;
				ibDst = ibDstStart;
				while (ibDst < ibDstEnd)
				{
					frameGrab.m_pbPixels[ibDst++] = pbSrc[ibSrc++];
				}
				ibSrcStart += image.Stride;
				ibDstStart += frameGrab.m_cbStride;
			}
	
			return frameGrab;
		}

		/// <summary>Timestamp of the video image</summary>
		public DateTime SampleTime { get { return m_tSample; } }
		private DateTime m_tSample;

		/// <summary>Width of the image in pixels</summary>
		public int Width { get { return m_dx; } }
		private int m_dx;

		/// <summary>Height of the image in pixels</summary>
		public int Height { get { return m_dy; } }
		private int m_dy;

		/// <summary>Number of bytes per pixel (typically 3)</summary>
		public int BytesPerPixel { get { return m_cb1Pix; } }
		private int m_cb1Pix;

		/// <summary>
		/// Bitmap image stride, i.e. the number of bytes from the start of
		/// one horizontal line to the next.
		/// </summary>
		public int Stride { get { return m_cbStride; } }
		private int m_cbStride;

		/// <summary>Number of bytes in the image bitmap</summary>
		public int NumBytes { get { return m_dx * m_dy * m_cb1Pix; } }

		/// <summary>The image pixels, as an array of bytes</summary>
		public byte[] Pixels { get { return m_pbPixels; } }
		private byte[] m_pbPixels;
	}
}
