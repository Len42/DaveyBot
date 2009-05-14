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

namespace DaveyBot
{
	/// <summary>
	/// A note-detection algorithm for Rock Band.
	/// </summary>
	/// <remarks>
	/// <para>This algorithm looks for bright pixels.</para>
	/// <para>It treats the image as two interlaced images, examining each separately.</para>
	/// </remarks>
	/// <seealso cref="NoteDef1"/>
	class NoteFinder4 : NoteFinder
	{
		// Note definitions
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 4, 0, 200, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 4, 200, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 4, 200, 170, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 4, 0, 0, 200);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 4, 200, 170, 0);

		override public int NumFramesDelay { get { return 8; } }

		override public void AnalyzeImage(IntPtr buf, int cbBuf,
										  int dyImage, int cbImageStride, int cb1Pix,
										  AnalyzeState state)
		{
			DetectNote(state.Green, m_notedefGreen,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Red, m_notedefRed,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Yellow, m_notedefYellow,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Blue, m_notedefBlue,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Orange, m_notedefOrange,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
		}

		/// <summary>
		/// Look for a particular type of note in an image.
		/// </summary>
		/// <remarks>
		/// <para>Look for a horizontal line or rectangle of (approximately) a particular colour.</para>
		/// <para>This method treats the image as two interlaced images and analyzes them separately.
		/// That makes the note shapes appear more coherent.</para>
		/// </remarks>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="buf">Bitmap video image, as a raw byte buffer</param>
		/// <param name="cbBuf">Size of image buffer</param>
		/// <param name="dyImage">Bitmap height in pixels</param>
		/// <param name="cbImageStride">Bitmap raster stride length</param>
		/// <param name="cb1Pix">Number of bytes per pixel (typically 3)</param>
		private void DetectNote(NoteState note,
								NoteDef1 notedef,
								IntPtr buf, int cbBuf,
								int dyImage, int cbImageStride, int cb1Pix)
		{
			DetectNoteInterlaced(note, notedef, buf, cbBuf, 0, dyImage, cbImageStride, cb1Pix);
			if (!note.Found)
				DetectNoteInterlaced(note, notedef, buf, cbBuf, 1, dyImage, cbImageStride, cb1Pix);
		}

		/// <summary>
		/// Check a given image region to see if a note is sitting there.
		/// </summary>
		/// <remarks>
		/// <para>This method checks one more more horizontal lines. If there are
		/// enough "on" pixels of the appropriate colour then there is something there!</para>
		/// <para>This method examines one interlaced sub-image.</para>
		/// </remarks>
		/// <seealso cref="DetectNote"/>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="buf">Bitmap video image, as a raw byte buffer</param>
		/// <param name="cbBuf">Size of image buffer</param>
		/// <param name="iInterlace">Offset (number of vertical lines) where interlaced image starts.
		/// Either 0 or 1.</param>
		/// <param name="dyImage">Bitmap height in pixels</param>
		/// <param name="cbImageStride">Bitmap raster stride length</param>
		/// <param name="cb1Pix">Number of bytes per pixel (typically 3)</param>
		private unsafe void DetectNoteInterlaced(NoteState note,
												NoteDef1 notedef,
												IntPtr buf, int cbBuf, int iInterlace,
												int dyImage, int cbImageStride, int cb1Pix)
		{

			//note.RValue = 0;
			//note.GValue = 0;
			//note.BValue = 0;

			// Scan a specified region of the bitmap.
			int xStart = notedef.xLeft;
			int yStart = 2 * notedef.yTop + iInterlace;
			yStart = dyImage - yStart - notedef.dy; // flip top-bottom
			byte* pbBuf = (byte*)buf;
			int ibStart = yStart * cbImageStride + xStart * cb1Pix;
			int cbRow = notedef.dx * cb1Pix;
			//int rTotal = 0, gTotal = 0, bTotal = 0;
			for (int yCur = yStart; yCur < yStart + notedef.dy; yCur++)
			{
				int ibEnd = ibStart + cbRow;
				for (int ib = ibStart; ib < ibEnd; ib += cb1Pix)
				{
					if (pbBuf[ib] >= notedef.bMin
						&& pbBuf[ib + 1] >= notedef.gMin
						&& pbBuf[ib + 2] >= notedef.rMin)
					{
						note.Found = true;
						// DEBUG: Mark found pixels
						pbBuf[ib] = 255;
						pbBuf[ib + 1] = 0;
						pbBuf[ib + 2] = 255;
						// DEBUG: Should quit when first pixel is found, for efficiency,
						// but let's mark all the bright pixels for testing purposes.
						//break;
					}
				}
				ibStart += 2 * cbImageStride;
			}

			// NOTE: Cannot distinuish between strummed notes and hammer-ons.
			note.Strum = note.Found;

			// DEBUG: Return various values used & calculated.
			note.RMin = notedef.rMin;
			note.GMin = notedef.gMin;
			note.BMin = notedef.bMin;
			//note.RValue = rTotal / (notedef.dx * notedef.dy);
			//note.GValue = gTotal / (notedef.dx * notedef.dy);
			//note.BValue = bTotal / (notedef.dx * notedef.dy);
		}
	}
}
