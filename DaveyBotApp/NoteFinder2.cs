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
	/// <para>This algorithm looks for the small white areas on each end of the notes.
	/// Regular strummed notes are wider than hammer-on/pull-off notes, so there are two
	/// sets of note definitions.</para>
	/// <para>This algorithm is quite accurate at recognizing notes, but not perfect.
	/// In particular, it is confused by the flash of yellow lines at the end of an energy phrase.</para>
	/// </remarks>
	/// <seealso cref="NoteDef2"/>
	class NoteFinder2 : NoteFinder
	{
		// Note definitions
		private const int dxNote = 3;
		private const int dyNote = 3;
		private const int nNoteBrightness = 120;
		private NoteDef2 m_notedefGreenStrum = new NoteDef2(266, 168, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefGreenHopo = new NoteDef2(257, 168, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefRedStrum = new NoteDef2(282, 167, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefRedHopo = new NoteDef2(291, 167, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefYellowStrum = new NoteDef2(339, 166, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefYellowHopo = new NoteDef2(348, 166, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefBlueStrum = new NoteDef2(438, 167, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefBlueHopo = new NoteDef2(405, 167, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefOrangeStrum = new NoteDef2(452, 168, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefOrangeHopo = new NoteDef2(461, 168, dxNote, dyNote, nNoteBrightness);

		override public int NumFramesDelay { get { return 8; } }

		override public void AnalyzeImage(VideoImage image, AnalyzeState state)
		{
			DetectNote(state.Green, m_notedefGreenStrum, m_notedefGreenHopo, image);
			DetectNote(state.Red, m_notedefRedStrum, m_notedefRedHopo, image);
			DetectNote(state.Yellow, m_notedefYellowStrum, m_notedefYellowHopo, image);
			DetectNote(state.Blue, m_notedefBlueStrum, m_notedefBlueHopo, image);
			DetectNote(state.Orange, m_notedefOrangeStrum, m_notedefOrangeHopo, image);

#if false // TODO: This doesn't work. Try something else.
			// Try to ignore the flash of yellow lines when an energy phrase is conpleted.
			// This may cause us to miss a real note occasionally. Oh well.
			if (state.Green.Found && state.Red.Found && state.Yellow.Found
				&& state.Blue.Found && state.Orange.Found)
			{
				state.Green.Found = false;
				state.Red.Found = false;
				state.Yellow.Found = false;
				state.Blue.Found = false;
				state.Orange.Found = false;
			}
#endif

			// DEBUG
			//NoteFinder nfOther = new NoteFinderTest();
			//DelegateTest(nfOther, image, state);
		}

		/// <summary>
		/// Look for a particular type of note in an image.
		/// </summary>
		/// <remarks>
		/// Look for the bright "ends" of the notes.
		/// Look for a regular (strummed) note first, and then for a hammer-on/pull-off note.
		/// </remarks>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedefStrummed">Description of the note being sought</param>
		/// <param name="notedefHopo">Description of the hammer-on version of the note</param>
		/// <param name="image">Image bitmap</param>
		private void DetectNote(NoteState note,
								NoteDef2 notedefStrummed, NoteDef2 notedefHopo,
								VideoImage image)
		{
			note.Strum = false;
			// Look for a regular strummed note.
			DetectNoteHelper(note, notedefStrummed, image);
			if (note.Found)
			{
				note.Strum = true; // found a strummed note
			}
			else
			{
				// Look for a not-strummed note.
				DetectNoteHelper(note, notedefHopo, image);
			}	
		}

		/// <summary>
		/// Check a given image region to see if a note is sitting there.
		/// </summary>
		/// <remarks>
		/// This method checks a small rectangle at the end of the note area.
		/// If there are enough "on" pixels then there is something there!
		/// </remarks>
		/// <seealso cref="DetectNote"/>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="image">Image bitmap</param>
		private unsafe void DetectNoteHelper(NoteState note,
											NoteDef2 notedef,
											VideoImage image)
		{
			note.RValue = 0;
			//note.gValue unused
			//note.bValue unused

			int xStart = notedef.xLeft;
			int yStart = notedef.yTop;
			yStart = image.Height - yStart - notedef.dy; // flip top-bottom
			byte* pbBuf = (byte*)image.ImageData;
			int cb1Pix = image.BytesPerPixel;
			int cbImageStride = image.Stride;
			int ibStart = image.Start + yStart * cbImageStride + xStart * cb1Pix;
			int cbRow = notedef.dx * cb1Pix;
			int nTotal = 0;
			for (int yCur = yStart; yCur < yStart + notedef.dy; yCur++)
			{
				int ibEnd = ibStart + cbRow;
				for (int ib = ibStart; ib < ibEnd; ib++)
				{
					nTotal += pbBuf[ib];
					// DEBUG: Mark checked pixels
					//pbBuf[ib] = 255;
				}
				ibStart += cbImageStride;
			}

			// Were enough "on" pixels found?
			note.Found = nTotal >= (notedef.nBrightnessMin * notedef.dx * notedef.dy * cb1Pix);

			// DEBUG: Return various values used & calculated.
			note.RMin = notedef.nBrightnessMin;
			//note.gMin unused
			//note.bMin unused
			note.RValue = nTotal / (notedef.dx * notedef.dy * cb1Pix);
			//note.gValue unused
			//note.bValue unused
		}
	}
}
