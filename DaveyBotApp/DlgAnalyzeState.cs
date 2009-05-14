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
using System.Windows.Forms;

namespace DaveyBot
{
	/// <summary>
	/// Dialog to display an <see cref="AnalyzeState"/> object for testing/debugging.
	/// </summary>
	public partial class DlgAnalyzeState : Form
	{
		public DlgAnalyzeState()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Populate the dialog with the values from an <see cref="AnalyzeState"/>.
		/// </summary>
		/// <param name="state"></param>
		public void FillDialog(AnalyzeState state)
		{
			txtFrameNum.Text = String.Format("Frame {0}", state.FrameNum);

			checkGreen.Checked = state.Green.Found;
			txtRGreen.Text = String.Format("{0} / {1}", state.Green.RValue, state.Green.RMin);
			txtGGreen.Text = String.Format("{0} / {1}", state.Green.GValue, state.Green.GMin);
			txtBGreen.Text = String.Format("{0} / {1}", state.Green.BValue, state.Green.BMin);

			checkRed.Checked = state.Red.Found;
			txtRRed.Text = String.Format("{0} / {1}", state.Red.RValue, state.Red.RMin);
			txtGRed.Text = String.Format("{0} / {1}", state.Red.GValue, state.Red.GMin);
			txtBRed.Text = String.Format("{0} / {1}", state.Red.BValue, state.Red.BMin);

			checkYellow.Checked = state.Yellow.Found;
			txtRYellow.Text = String.Format("{0} / {1}", state.Yellow.RValue, state.Yellow.RMin);
			txtGYellow.Text = String.Format("{0} / {1}", state.Yellow.GValue, state.Yellow.GMin);
			txtBYellow.Text = String.Format("{0} / {1}", state.Yellow.BValue, state.Yellow.BMin);

			checkBlue.Checked = state.Blue.Found;
			txtRBlue.Text = String.Format("{0} / {1}", state.Blue.RValue, state.Blue.RMin);
			txtGBlue.Text = String.Format("{0} / {1}", state.Blue.GValue, state.Blue.GMin);
			txtBBlue.Text = String.Format("{0} / {1}", state.Blue.BValue, state.Blue.BMin);

			checkOrange.Checked = state.Orange.Found;
			txtROrange.Text = String.Format("{0} / {1}", state.Orange.RValue, state.Orange.RMin);
			txtGOrange.Text = String.Format("{0} / {1}", state.Orange.GValue, state.Orange.GMin);
			txtBOrange.Text = String.Format("{0} / {1}", state.Orange.BValue, state.Orange.BMin);
		}
	}
}
