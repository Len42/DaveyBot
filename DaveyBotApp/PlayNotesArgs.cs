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
	/// Arguments for the PlayNotes event of <see cref="DaveyBot.Brain"/> and <see cref="DaveyBot.Fingers"/>.
	/// </summary>
	public class PlayNotesArgs : EventArgs
	{
		/// <summary>Play a Green note</summary>
		public bool PressGreen { get { return m_fPressGreen; } set { m_fPressGreen = value; } }
		private bool m_fPressGreen;

		/// <summary>Play a Red note</summary>
		public bool PressRed { get { return m_fPressRed; } set { m_fPressRed = value; } }
		private bool m_fPressRed;

		/// <summary>Play a Yellow note</summary>
		public bool PressYellow { get { return m_fPressYellow; } set { m_fPressYellow = value; } }
		private bool m_fPressYellow;

		/// <summary>Play a Blue note</summary>
		public bool PressBlue { get { return m_fPressBlue; } set { m_fPressBlue = value; } }
		private bool m_fPressBlue;

		/// <summary>Play a Orange note</summary>
		public bool PressOrange { get { return m_fPressOrange; } set { m_fPressOrange = value; } }
		private bool m_fPressOrange;

		/// <summary>Hit the strum bar</summary>
		public bool Strum { get { return m_fStrum; } set { m_fStrum = value; } }
		private bool m_fStrum;

		/// <summary>The time when the note is to be played</summary>
		public DateTime When { get { return m_tWhen; } set { m_tWhen = value; } }
		private DateTime m_tWhen;

		public PlayNotesArgs(bool fPressGreen, bool fPressRed, bool fPressYellow, bool fPressBlue, bool fPressOrange,
							  bool fStrum, DateTime tWhen)
		{
			m_fPressGreen = fPressGreen;
			m_fPressRed = fPressRed;
			m_fPressYellow = fPressYellow;
			m_fPressBlue = fPressBlue;
			m_fPressOrange = fPressOrange;
			m_fStrum = fStrum;
			m_tWhen = tWhen;
		}
	}
}
