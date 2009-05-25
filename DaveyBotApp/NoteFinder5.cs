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
	/// <para>This algorithm looks for bright pixels in a small area.</para>
	/// <para>Also, when a note "turns off", it checks the immediately following area
	/// to see if the note is actually finished. This avoids erroneous on-offs caused
	/// by dark spots in the notes.</para>
	/// </remarks>
	/// <seealso cref="NoteDef1"/>
	class NoteFinder5 : NoteFinder
	{
		// Note definitions
		// lower thresholds, note height = 3
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 3, 0, 170, 0);
		private NoteDef1 m_notedefGreenFollow = new NoteDef1(253, 157, 7, 3, 0, 170, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 3, 170, 0, 0);
		private NoteDef1 m_notedefRedFollow = new NoteDef1(303, 156, 7, 3, 170, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 3, 170, 150, 0);
		private NoteDef1 m_notedefYellowFollow = new NoteDef1(357, 155, 7, 3, 170, 150, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 3, 0, 0, 170);
		private NoteDef1 m_notedefBlueFollow = new NoteDef1(411, 156, 7, 3, 0, 0, 170);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 3, 170, 150, 0);
		private NoteDef1 m_notedefOrangeFollow = new NoteDef1(461, 157, 7, 3, 170, 150, 0);

#if false
		// higher thresholds, note height = 3
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 3, 0, 220, 0);
		private NoteDef1 m_notedefGreenFollow = new NoteDef1(253, 157, 7, 3, 0, 220, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 3, 220, 0, 0);
		private NoteDef1 m_notedefRedFollow = new NoteDef1(303, 156, 7, 3, 220, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 3, 220, 170, 0);
		private NoteDef1 m_notedefYellowFollow = new NoteDef1(357, 155, 7, 3, 220, 170, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 3, 0, 0, 220);
		private NoteDef1 m_notedefBlueFollow = new NoteDef1(411, 156, 7, 3, 0, 0, 220);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 3, 220, 170, 0);
		private NoteDef1 m_notedefOrangeFollow = new NoteDef1(461, 157, 7, 3, 220, 170, 0);
#endif

#if false
		// lower thresholds, note height = 3
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 3, 0, 184, 0);
		private NoteDef1 m_notedefGreenFollow = new NoteDef1(253, 157, 7, 3, 0, 184, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 3, 184, 0, 0);
		private NoteDef1 m_notedefRedFollow = new NoteDef1(303, 156, 7, 3, 184, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 3, 184, 165, 0);
		private NoteDef1 m_notedefYellowFollow = new NoteDef1(357, 155, 7, 3, 184, 165, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 3, 0, 0, 184);
		private NoteDef1 m_notedefBlueFollow = new NoteDef1(411, 156, 7, 3, 0, 0, 184);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 3, 184, 165, 0);
		private NoteDef1 m_notedefOrangeFollow = new NoteDef1(461, 157, 7, 3, 184, 165, 0);
#endif

#if false
		// first try
		private NoteDef1 m_notedefGreen = new NoteDef1(250, 160, 7, 2, 0, 200, 0);
		private NoteDef1 m_notedefGreenFollow = new NoteDef1(252, 158, 7, 2, 0, 200, 0);
		private NoteDef1 m_notedefRed = new NoteDef1(302, 159, 7, 2, 200, 0, 0);
		private NoteDef1 m_notedefRedFollow = new NoteDef1(303, 157, 7, 2, 200, 0, 0);
		private NoteDef1 m_notedefYellow = new NoteDef1(357, 158, 7, 2, 200, 170, 0);
		private NoteDef1 m_notedefYellowFollow = new NoteDef1(357, 156, 7, 2, 200, 170, 0);
		private NoteDef1 m_notedefBlue = new NoteDef1(412, 159, 7, 2, 0, 0, 200);
		private NoteDef1 m_notedefBlueFollow = new NoteDef1(411, 157, 7, 2, 0, 0, 200);
		private NoteDef1 m_notedefOrange = new NoteDef1(464, 160, 7, 2, 200, 170, 0);
		private NoteDef1 m_notedefOrangeFollow = new NoteDef1(462, 158, 7, 2, 200, 170, 0);
#endif
		override public int NumFramesDelay { get { return 12; } }

		override public void AnalyzeImage(VideoImage image, AnalyzeState state)
		{
			DetectNote(state.FrameNum, state.Green, m_notedefGreen, m_notedefGreenFollow, image);
			DetectNote(state.FrameNum, state.Red, m_notedefRed, m_notedefRedFollow, image);
			DetectNote(state.FrameNum, state.Yellow, m_notedefYellow, m_notedefYellowFollow, image);
			DetectNote(state.FrameNum, state.Blue, m_notedefBlue, m_notedefBlueFollow, image);
			DetectNote(state.FrameNum, state.Orange, m_notedefOrange, m_notedefOrangeFollow, image);
		}

		private void DetectNote(uint iFrame,
								NoteState note,
								NoteDef1 notedef,
								NoteDef1 notedefFollow,
								VideoImage image)
		{
			DetectNoteHelper(iFrame, note, notedef, image);
			// If the note seems to have ended, check the immediately following area to make sure.
			if (!note.Found && note.PrevFound)
				DetectNoteHelper(iFrame, note, notedefFollow, image);
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
		private unsafe void DetectNoteHelper(uint iFrame,
											NoteState note,
											NoteDef1 notedef,
											VideoImage image)
		{
			//note.RValue = 0;
			//note.GValue = 0;
			//note.BValue = 0;

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
						//pbBuf[ib] = 255;
						//pbBuf[ib + 1] = 0;
						//pbBuf[ib + 2] = 255;
						// DEBUG: Should quit when first pixel is found, for efficiency,
						// but let's mark all the bright pixels for testing purposes.
						//break;
					}
				}
				ibStart += cbImageStride;
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
