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
namespace DaveyBot
{
	/// <summary>
	/// Defines positions of notes on the screen for <see cref="NoteFinder6"/>.
	/// </summary>
	class NoteDef6
	{
		// Position of the note
		public int xLeft;
		public int yTop;
		public int dx;
		public int dy;
		// Red, green & blue thresholds for a note to be detected
		// If zero, ignore that colour
		public int rMin;
		public int gMin;
		public int bMin;
		// Hysteresis: Once a note is detected, it will stay "on" until the
		// colors drop a bit below the "on" threshold.
		public int nHysteresis;

		public NoteDef6(int xLeft, int yTop, int dx, int dy, int rMin, int gMin, int bMin, int nHysteresis)
		{
			this.xLeft = xLeft;
			this.yTop = yTop;
			this.dx = dx;
			this.dy = dy;
			this.rMin = rMin;
			this.gMin = gMin;
			this.bMin = bMin;
			this.nHysteresis = nHysteresis;
		}
	}
}
