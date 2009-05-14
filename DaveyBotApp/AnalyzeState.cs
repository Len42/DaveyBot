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
using System.Collections.Generic;
using System.Text;

namespace DaveyBot
{
	/// <summary>
	/// The current state of one note (on/off, strummed or not, etc.)
	/// </summary>
	/// <seealso cref="AnalyzeState"/>
	public class NoteState
	{
		/// <summary>Has this note been found on the current screen image?</summary>
		public bool Found { get { return fFound; } set { fFound = value; } }
		private bool fFound = false;

		/// <summary>Was this note found on the previous screen image?</summary>
		public bool PrevFound { get { return fPrevFound; } set { fPrevFound = value; } }
		private bool fPrevFound = false;

		/// <summary>Did this note just appear now?</summary>
		public bool TurnedOn { get { return !fPrevFound && fFound; } }

		/// <summary>If a note was found, is it to be strummed or not?</summary>
		public bool Strum { get { return fStrum; } set { fStrum = value; } }
		private bool fStrum = false; // if fFound, should it be strummed or not?

		/// <summary>Is the detected note part of an "energy phrase"?</summary>
		/// <remarks>TODO: currently unused</remarks>
		public bool Energy { get { return fEnergy; } set { fEnergy = value; } }
		private bool fEnergy = false;

		// DEBUG: R,G,B analysis values
		internal int RMin { get { return rMin; } set { rMin = value; } }
		private int rMin = 0;
		internal int GMin { get { return gMin; } set { gMin = value; } }
		private int gMin = 0;
		internal int BMin { get { return bMin; } set { bMin = value; } }
		private int bMin = 0;
		internal int RValue { get { return rValue; } set { rValue = value; } }
		private int rValue = 0;
		internal int GValue { get { return gValue; } set { gValue = value; } }
		private int gValue = 0;
		internal int BValue { get { return bValue; } set { bValue = value; } }
		private int bValue = 0;

		/// <summary>
		/// Render this object's properties as a comma-separated text string, for debugging.
		/// </summary>
		/// <returns>String of comma-separated values describing the object's properties</returns>
		public string AsCSVString()
		{
			return String.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
								fFound ? "True" : "",
								fPrevFound ? "True" : "",
								rMin, gMin, bMin,
								rValue, gValue, bValue);
		}
	}
	
	/// <summary>
	/// The current state of the note analysis.
	/// Keeps track of the state of each of the notes (green, red, etc.).
	/// </summary>
	public class AnalyzeState
	{
		/// <summary>Video frame counter</summary>
		public uint FrameNum { get { return m_iFrame; } }
		private uint m_iFrame = UInt32.MaxValue; // so it will roll over to 0 on the first frame

		/// <summary>Current state of the Green note</summary>
		public NoteState Green { get { return m_noteGreen; } }
		private NoteState m_noteGreen = new NoteState();

		/// <summary>Current state of the Red note</summary>
		public NoteState Red { get { return m_noteRed; } }
		private NoteState m_noteRed = new NoteState();

		/// <summary>Current state of the Yellow note</summary>
		public NoteState Yellow { get { return m_noteYellow; } }
		private NoteState m_noteYellow = new NoteState();

		/// <summary>Current state of the Blue note</summary>
		public NoteState Blue { get { return m_noteBlue; } }
		private NoteState m_noteBlue = new NoteState();

		/// <summary>Current state of the Orange note</summary>
		public NoteState Orange { get { return m_noteOrange; } }
		private NoteState m_noteOrange = new NoteState();

		/// <summary>Was a strummed note detected?</summary>
		public bool Strum
		{
			get
			{
				return m_noteGreen.Strum || m_noteRed.Strum || m_noteYellow.Strum || m_noteBlue.Strum || m_noteOrange.Strum;
			}
		}

		/// <summary>
		/// Initialize the object to handle the next video frame.
		/// </summary>
		public void InitNextFrame()
		{
			m_iFrame++;
			m_noteGreen.PrevFound = m_noteGreen.Found;
			m_noteGreen.Found = false;
			m_noteRed.PrevFound = m_noteRed.Found;
			m_noteRed.Found = false;
			m_noteYellow.PrevFound = m_noteYellow.Found;
			m_noteYellow.Found = false;
			m_noteBlue.PrevFound = m_noteBlue.Found;
			m_noteBlue.Found = false;
			m_noteOrange.PrevFound = m_noteOrange.Found;
			m_noteOrange.Found = false;
		}

		/// <summary>
		/// Display the current state in a pop-up dialog.
		/// </summary>
		/// <param name="wndParent">Dialog's parent window (optional)</param>
		public void ShowDialog(System.Windows.Forms.Control wndParent)
		{
			DlgAnalyzeState dlg = new DlgAnalyzeState();
			dlg.FillDialog(this);
			dlg.ShowDialog(wndParent);
		}
	}
}
