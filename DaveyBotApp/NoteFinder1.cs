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
	/// <para>This algorithm looks for rectangles with certain colour values.</para>
	/// <para>This algorithm is not very reliable at recognizing notes.</para>
	/// </remarks>
	/// <seealso cref="NoteDef1"/>
	class NoteFinder1 : NoteFinder
	{
		// Note definitions
		private NoteDef1 m_notedefGreen = new NoteDef1(252, 151, 15, 1, 0, 200, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(303, 150, 15, 1, 200, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(353, 150, 15, 1, 180, 160, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(404, 150, 15, 1, 0, 0, 200);
		private NoteDef1 m_notedefOrange = new NoteDef1(453, 151, 15, 1, 180, 140, 0);

		override public int NumFramesDelay { get { return 16; } }

		override public void AnalyzeImage(VideoImage image, AnalyzeState state)
		{
			DetectNote(state.Green, m_notedefGreen, image);
			DetectNote(state.Red, m_notedefRed, image);
			DetectNote(state.Yellow, m_notedefYellow, image);
			DetectNote(state.Blue, m_notedefBlue, image);
			DetectNote(state.Orange, m_notedefOrange, image);
		}

		/// <summary>
		/// Check a given image region to see if a note is sitting there.
		/// </summary>
		/// <remarks>
		/// <para>This method checks one more more horizontal lines. If there are
		/// enough "on" pixels of the appropriate colour then there is something there!</para>
		/// </remarks>
		/// <seealso cref="DetectNote"/>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="image">Image bitmap</param>
		private unsafe void DetectNote(NoteState note,
										NoteDef1 notedef,
										VideoImage image)
		{
			note.RValue = 0;
			note.GValue = 0;
			note.BValue = 0;

			// Scan a specified region of the bitmap.
			int xStart = notedef.xLeft;
			int yStart = notedef.yTop;
			yStart = image.Height - yStart - notedef.dy; // flip top-bottom
			byte* pbBuf = (byte*)image.ImageData;
			int cb1Pix = image.BytesPerPixel;
			int cbImageStride = image.Stride;
			int ibStart = image.Start + yStart * cbImageStride + xStart * cb1Pix;
			int cbRow = notedef.dx * cb1Pix;
			int rTotal = 0, gTotal = 0, bTotal = 0;
			for (int yCur = yStart; yCur < yStart + notedef.dy; yCur++)
			{
				int ibEnd = ibStart + cbRow;
				for (int ib = ibStart; ib < ibEnd; ib += cb1Pix)
				{
					bTotal += pbBuf[ib];
					gTotal += pbBuf[ib + 1];
					rTotal += pbBuf[ib + 2];
					// DEBUG: Mark checked pixels
					//pbBuf[ib] = 255;
				}
				ibStart += cbImageStride;
			}

			// Were enough "on" pixels found?
			note.Found = ((notedef.rMin == 0 || rTotal >= notedef.rMin * notedef.dx * notedef.dy)
				&& (notedef.gMin == 0 || gTotal >= notedef.gMin * notedef.dx * notedef.dy)
				&& (notedef.bMin == 0 || bTotal >= notedef.bMin * notedef.dx * notedef.dy));
			// NOTE: Cannot distinuish between strummed notes and hammer-ons.
			note.Strum = note.Found;

			// DEBUG: Return various values used & calculated.
			note.RMin = notedef.rMin;
			note.GMin = notedef.gMin;
			note.BMin = notedef.bMin;
			note.RValue = rTotal / (notedef.dx * notedef.dy);
			note.GValue = gTotal / (notedef.dx * notedef.dy);
			note.BValue = bTotal / (notedef.dx * notedef.dy);
		}
	}
}
