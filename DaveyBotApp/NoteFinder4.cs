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
	/// This algorithm looks for bright pixels.
	/// </remarks>
	/// <seealso cref="NoteDef1"/>
	class NoteFinder4 : NoteFinder
	{
		// Note definitions
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 3, 0, 200, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 3, 200, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 3, 200, 170, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 3, 0, 0, 200);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 3, 200, 170, 0);

		override public int NumFramesDelay { get { return 12; } }

		/// <summary>Minimum length of a note, in frames.</summary>
		/// <remarks>This helps to avoid glitches where a note is lost then re-found.</remarks>
		private const uint m_cframeMinNoteLength = 3;

		override public void AnalyzeImage(VideoImage image, AnalyzeState state)
		{
			DetectNote(state.FrameNum, state.Green, m_notedefGreen, image);
			DetectNote(state.FrameNum, state.Red, m_notedefRed, image);
			DetectNote(state.FrameNum, state.Yellow, m_notedefYellow, image);
			DetectNote(state.FrameNum, state.Blue, m_notedefBlue, image);
			DetectNote(state.FrameNum, state.Orange, m_notedefOrange, image);
		}

		/// <summary>
		/// Check a given image region to see if a note is sitting there.
		/// </summary>
		/// <remarks>
		/// This method checks one more more horizontal lines. If there are
		/// enough "on" pixels of the appropriate colour then there is something there!
		/// </remarks>
		/// <seealso cref="DetectNote"/>
		/// <param name="iFrame">Video frame counter</param>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="image">Image bitmap</param>
		private unsafe void DetectNote(uint iFrame,
										NoteState note,
										NoteDef1 notedef,
										VideoImage image)
		{
			//note.RValue = 0;
			//note.GValue = 0;
			//note.BValue = 0;

			if (note.PrevFound && iFrame - note.FrameStart < m_cframeMinNoteLength)
			{
				// We're holding the current note until a few frames have passed.
				note.Found = true;
			}
			else
			{
				// Look for a note.
				// Scan a specified region of the bitmap.
				int xStart = notedef.xLeft;
				int yStart = notedef.yTop;
				yStart = image.Height - yStart - notedef.dy; // flip top-bottom
				byte* pbBuf = (byte*)image.ImageData;
				int cb1Pix = image.BytesPerPixel;
				int cbImageStride = image.Stride;
				int ibStart = image.Start + yStart * cbImageStride + xStart * cb1Pix;
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
					ibStart += cbImageStride;
				}

				// Keep track of when this note started so we can enforce a minimum note length.
				if (note.Found && !note.PrevFound)
					note.FrameStart = iFrame;
			}

			// NOTE: Cannot distinguish between strummed notes and hammer-ons.
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
