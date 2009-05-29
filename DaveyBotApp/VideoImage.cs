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
using System.Drawing;

namespace DaveyBot
{
	/// <summary>
	/// An interlaced video image bitmap that was captured by <see cref="Eyes"/>.
	/// </summary>
	/// <remarks>
	/// <para>The bitmap data buffer is represented by an IntPtr which is NOT freed
	/// when this object is destroyed.</para>
	/// <para>This class can represent either sub-frame of an interlaced bitmap,
	/// by setting Start and Stride appropriately.</para>
	/// <para>Derived from EventArgs so it can be used in events.</para>
	/// </remarks>
	public class VideoImage : EventArgs
	{
		public VideoImage(DateTime tSample, int dx, int dy, int cb1Pix, int cbStride, int ibStart, IntPtr buf, int cbBuf)
		{
			m_tSample = tSample;
			m_dx = dx;
			m_dy = dy;
			m_cb1Pix = cb1Pix;
			m_cbStride = cbStride;
			m_ibStart = ibStart;
			m_buf = buf;
			m_cbBuf = cbBuf;
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
		/// <remarks>
		/// Start and Stride can be used to select one sub-image in an interlaced image.
		/// </remarks>
		public int Stride { get { return m_cbStride; } }
		private int m_cbStride;

		/// <summary>
		/// Byte offset to start of the image.
		/// </summary>
		/// <remarks>
		/// Start and Stride can be used to select one sub-image in an interlaced image.
		/// </remarks>
		public int Start { get { return m_ibStart; } }
		private int m_ibStart;

		/// <summary>Number of bytes in the image bitmap</summary>
		public int NumBytes { get { return m_dx * m_dy * m_cb1Pix; } }
		private int m_cbBuf;

		/// <summary>The image pixels, as an array of bytes</summary>
		public IntPtr ImageData { get { return m_buf; } }
		private IntPtr m_buf;

		/// <summary>
		/// Split an interlaced video image into its two component sub-images.
		/// </summary>
		/// <param name="dtFrameInterval">Time interval between video sub-frames</param>
		/// <param name="imageOut0"></param>
		/// <param name="imageOut1"></param>
		public void Deinterlace(TimeSpan dtFrameInterval, out VideoImage imageOut0, out VideoImage imageOut1)
		{
			TimeSpan dtSubInterval = new TimeSpan(dtFrameInterval.Ticks / 2);
			// NOTE: The first image starts on the second scan line, and
			// the second image starts on the first scan line!
			imageOut0 = new VideoImage(SampleTime, Width, Height / 2, BytesPerPixel, 2 * Stride, Stride, ImageData, NumBytes);
			imageOut1 = new VideoImage(SampleTime + dtSubInterval, Width, Height - imageOut0.Height, BytesPerPixel, 2 * Stride, 0, ImageData, NumBytes);
		}

		/// <summary>
		/// Splat a coloured rectangle onto a bitmap.
		/// </summary>
		/// <remarks>
		/// This is used to flash rectangles onto the video image stream to indicate
		/// notes that are detected.
		/// </remarks>
		/// <param name="rect">Rectangular to draw</param>
		/// <param name="clr">Rectangle colour</param>
		public unsafe void FillRect(Rectangle rect, Color clr)
		{
			byte* pbBuf = (byte*)ImageData;
			byte R = clr.R; // cache these values because they're used in loops
			byte G = clr.G;
			byte B = clr.B;

			if (Width <= 0 || Height <= 0 || BytesPerPixel <= 0 || Stride <= 0)
			{
				// Error - video sizes not initialized
				// (don't throw an exception for these errors because they may repeat on every video frame)
			}
			else if (rect.Left < 0 || rect.Right >= Width || rect.Top < 0 || rect.Bottom >= Height)
			{
				// Error - invalid rectangle coords
			}
			else if (BytesPerPixel != 3)
			{
				// Error - we can't handle this video format
			}
			else
			{
				// Flip top & bottom
				int y = Height - rect.Bottom;
				int ibCur;
				int ibStart;
				int ibEnd;
				ibStart = Start + y * Stride + rect.Left * BytesPerPixel;
				for (int i = 0; i < rect.Height; i++)
				{
					ibEnd = ibStart + rect.Width * BytesPerPixel;
					for (ibCur = ibStart; ibCur < ibEnd; ibCur += BytesPerPixel)
					{
						// NOTE: Pixel byte order is B, G, R
						pbBuf[ibCur] = B;
						pbBuf[ibCur + 1] = G;
						pbBuf[ibCur + 2] = R;
					}
					ibStart += Stride;
				}
			}
		}
	}
}
