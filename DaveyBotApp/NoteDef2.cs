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
	/// Defines positions of notes on the screen for various <see cref="NoteFinder"/> subclasses.
	/// </summary>
	/// <remarks>
	/// NoteDef2 defines the bright "ends" of the notes.
	/// Hammer-on/pull-off notes are shorter than regular notes,
	/// so each type will have its own NoteDef2.
	/// </remarks>
	/// <todo>Look at the right-hand end of the note too perhaps?</todo>
	class NoteDef2
	{
		public int xLeft;
		public int yTop;
		public int dx;
		public int dy;
		public int nBrightnessMin;

		public NoteDef2(int xLeft, int yTop, int dx, int dy, int nBrightnessMin)
		{
			this.xLeft = xLeft;
			this.yTop = yTop;
			this.dx = dx;
			this.dy = dy;
			this.nBrightnessMin = nBrightnessMin;
		}
	}
}
