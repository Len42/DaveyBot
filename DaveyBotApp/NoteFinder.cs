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
	/// Abstract class to analyze a video image and find any notes
	/// that are about to be played.
	/// </summary>
	public abstract class NoteFinder
	{
		/// <summary>
		/// Analyze the given bitmap image and update state to indicate any notes that were found.
		/// </summary>
		/// <remarks>
		/// <para>Subclasses implement a note-detection algorithm here. Different subclasses
		/// are used for different games, and to test different algorithms.</para>
		/// <para>Subclasses typically implement a DetectNote method and call it for each
		/// possible note (green, red, etc.).</para>
		/// </remarks>
		/// <param name="image">Video image bitmap</param>
		/// <param name="state">Current analysis state</param>
		abstract public void AnalyzeImage(VideoImage image, AnalyzeState state);

		/// <summary>
		/// The delay (as a number of video frames) between when a note is detected
		/// and when it hits the strum line.
		/// </summary>
		/// <remarks>
		/// This depends on how high up the screen a particular NoteFinder algorithm detects the notes.
		/// </remarks>
		abstract public int NumFramesDelay { get; }

		protected void DelegateTest(NoteFinder nfOther, VideoImage image, AnalyzeState state)
		{
			nfOther.AnalyzeImage(image, state);
		}
	}
}
