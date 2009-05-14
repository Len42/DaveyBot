﻿/*
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
	/// <para>It treats the image as two interlaced images, examining each separately.</para>
	/// <para>This algorithm is quite accurate at recognizing notes, but not perfect.
	/// In particular, it is confused by the flash of yellow lines at the end of an energy phrase.</para>
	/// </remarks>
	/// <seealso cref="NoteDef1"/>
	class NoteFinder3 : NoteFinder
	{
		// Note definitions
		private const int dxNote = 2;
		private const int dyNote = 4; //3;
		private const int nNoteBrightness = 120;
		private NoteDef2 m_notedefGreenStrum = new NoteDef2(277, 146, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefGreenHopo = new NoteDef2(271, 146, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefRedStrum = new NoteDef2(327, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefRedHopo = new NoteDef2(320, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefYellowStrum = new NoteDef2(341, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefYellowHopo = new NoteDef2(348, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefBlueStrum = new NoteDef2(392, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefBlueHopo = new NoteDef2(399, 145, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefOrangeStrum = new NoteDef2(440/*443*/, 146, dxNote, dyNote, nNoteBrightness);
		private NoteDef2 m_notedefOrangeHopo = new NoteDef2(447, 146, dxNote, dyNote, nNoteBrightness);

		override public int NumFramesDelay { get { return 10; } }

		override public void AnalyzeImage(IntPtr buf, int cbBuf,
										int dyImage, int cbImageStride, int cb1Pix,
										AnalyzeState state)
		{
			DetectNote(state.Green, m_notedefGreenStrum, m_notedefGreenHopo,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Red, m_notedefRedStrum, m_notedefRedHopo,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Yellow, m_notedefYellowStrum, m_notedefYellowHopo,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Blue, m_notedefBlueStrum, m_notedefBlueHopo,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);
			DetectNote(state.Orange, m_notedefOrangeStrum, m_notedefOrangeHopo,
						buf, cbBuf,
						dyImage, cbImageStride, cb1Pix);

/* // TODO: This doesn't work. Try something else.
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
*/
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
		/// <param name="buf">Bitmap video image, as a raw byte buffer</param>
		/// <param name="cbBuf">Size of image buffer</param>
		/// <param name="dyImage">Bitmap height in pixels</param>
		/// <param name="cbImageStride">Bitmap raster stride length</param>
		/// <param name="cb1Pix">Number of bytes per pixel (typically 3)</param>
		private void DetectNote(NoteState note,
								NoteDef2 notedefStrummed, NoteDef2 notedefHopo,
								IntPtr buf, int cbBuf,
								int dyImage, int cbImageStride, int cb1Pix)
		{
			note.Strum = false;
			// Look for a regular strummed note.
			DetectNoteInterlaced(note, notedefStrummed, buf, cbBuf, dyImage, cbImageStride, cb1Pix);
			if (note.Found)
			{
				note.Strum = true; // found a strummed note
			}
			else
			{
				// Look for a not-strummed note.
				DetectNoteInterlaced(note, notedefHopo, buf, cbBuf, dyImage, cbImageStride, cb1Pix);
			}
		}

		/// <summary>
		/// Look for a particular type of note in an image.
		/// </summary>
		/// <remarks>
		/// This method treats the image as two interlaced images and analyzes them separately.
		/// That makes the note shapes appear more coherent.
		/// </remarks>
		/// <param name="note">State of the particular note (green, red, etc.)
		/// which will be updated if that note is detected</param>
		/// <param name="notedef">Description of the note being sought</param>
		/// <param name="buf">Bitmap video image, as a raw byte buffer</param>
		/// <param name="cbBuf">Size of image buffer</param>
		/// <param name="dyImage">Bitmap height in pixels</param>
		/// <param name="cbImageStride">Bitmap raster stride length</param>
		/// <param name="cb1Pix">Number of bytes per pixel (typically 3)</param>
		private void DetectNoteInterlaced(NoteState note,
										NoteDef2 notedef,
										IntPtr buf, int cbBuf,
										int dyImage, int cbImageStride, int cb1Pix)
		{
			DetectNoteHelper(note, notedef, buf, cbBuf, 0, dyImage, cbImageStride, cb1Pix);
			if (!note.Found)
				DetectNoteHelper(note, notedef, buf, cbBuf, 1, dyImage, cbImageStride, cb1Pix);
		}

		/// <summary>
		/// Check a given image region to see if a note is sitting there.
		/// </summary>
		/// <remarks>
		/// <para>This method checks a small rectangle at the end of the note area.
		/// If there are enough "on" pixels then there is something there!</para>
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
		private unsafe void DetectNoteHelper(NoteState note,
											NoteDef2 notedef,
											IntPtr buf, int cbBuf, int iInterlace,
											int dyImage, int cbImageStride, int cb1Pix)
		{
			note.RValue = 0;
			//note.gValue unused
			//note.bValue unused

			int xStart = notedef.xLeft;
			int yStart = 2 * notedef.yTop + iInterlace;
			yStart = dyImage - yStart - notedef.dy; // flip top-bottom
			byte* pbBuf = (byte*)buf;
			int ibStart = yStart * cbImageStride + xStart * cb1Pix;
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
				ibStart += 2 * cbImageStride;
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
