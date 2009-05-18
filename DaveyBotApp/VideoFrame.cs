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
	/// A video image bitmap that was captured by <see cref="Eyes"/>.
	/// </summary>
	/// <remarks>
	/// <para>The bitmap data buffer is represented by an IntPtr which is NOT freed
	/// when this object is destroyed.</para>
	/// <para>This class can represent either sub-frame of an interlaced bitmap,
	/// by setting Start and Stride appropriately.</para>
	/// </remarks>
	public struct VideoFrame
	{
		public VideoFrame(double tSample, int dx, int dy, int cb1Pix, int cbStride, int ibStart, IntPtr buf, int cbBuf)
		{
			m_tSample = tSample;
			m_dx = dx;
			m_dy = dy;
			m_cb1Pix = cb1Pix;
			m_cbStride = cbStride;
			m_ibStart = ibStart;
			System.Diagnostics.Debug.Assert(dx * dy * 3 == cbBuf);
			m_pbPixels = new byte[cbBuf];
			Marshal.Copy(buf, m_pbPixels, 0, cbBuf);
		}

		/// <summary>Timestamp of the video image</summary>
		public double SampleTime { get { return m_tSample; } }
		private double m_tSample;

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

		/// <summary>The image pixels, as an array of bytes</summary>
		public byte[] Pixels { get { return m_pbPixels; } }
		private byte[] m_pbPixels;
	}
}
