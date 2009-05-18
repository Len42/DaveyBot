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
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace DaveyBot
{
	/// <summary>
	/// Plays the correct notes at the correct times, as commanded by the <see cref="DaveyBot.Brain"/>.
	/// </summary>
	/// <remarks>
	/// <para>Notes are played by sending commands out a serial port (through a USB-serial converter)
	/// to a hacked Xbox 360 Guitar Hero controller, where they are interpreted by an Arduino
	/// microcontroller that triggers simulated button presses.</para>
	/// <para>The note commands are each a single byte, with each bit representing
	/// the on/off state of a button on the guitar.</para>
	/// </remarks>
	public class Fingers
	{
		/// <summary>
		/// The source of play-note events.
		/// </summary>
		public Brain Brain
		{
			get { return m_brain; }
			set
			{
				// Hook up the event to receive the play-note commands.
				if (m_brain != null)
					m_brain.PlayNotes -= brain_PlayNotes;
				m_brain = value;
				if (m_brain != null)
					m_brain.PlayNotes += brain_PlayNotes;
			}
		}
		private Brain m_brain;

		/// <summary>
		/// Event raised when notes are played.
		/// </summary>
		/// <remarks>
		/// This event is raised when the notes are actually played
		/// (as opposed to when they are detected).
		/// </remarks>
		public event EventHandler<PlayNotesArgs> PlayNotes;
		private void OnPlayNotes(PlayNotesArgs args)
		{
			if (PlayNotes != null)
				PlayNotes(this, args);
		}

		SerialPort m_port = null;

		/// <summary>
		/// Initialize the comm port to send commands to the guitar controller.
		/// </summary>
		private void OpenSerialPort()
		{
			CloseSerialPort();
			m_port = new SerialPort(Properties.Settings.Default.ControllerPort);
			m_port.BaudRate = 57600;
			m_port.DataBits = 8;
			m_port.Parity = Parity.None;
			m_port.StopBits = StopBits.One;
			m_port.Open();
		}

		private void CloseSerialPort()
		{
			if (m_port != null)
			{
				if (m_port.IsOpen)
					m_port.Close();
				m_port = null;
			}
		}

		/// <summary>
		/// Bit definitions for guitar controller commands
		/// </summary>
		[Flags]
		private enum Button : byte
		{
			None = 0,
			Green = 0x01,
			Red = 0x02,
			Yellow = 0x04,
			Blue = 0x08,
			Orange = 0x10,
			Strum = 0x20,
			Tilt = 0x40,
			Whammy = 0x80
		}

		static byte[] rgbWriteByte = new byte[1];

		/// <summary>
		/// Send an all-buttons-off command to the guitar controller.
		/// </summary>
		private void WriteCommandOff()
		{
			// allow to run without m_port, for testing
			if (m_port != null && m_port.IsOpen)
			{
				rgbWriteByte[0] = 0;
				m_port.Write(rgbWriteByte, 0, 1);
			}
		}

		/// <summary>
		/// Send a command out the comm port to press some buttons on the guitar.
		/// </summary>
		/// <remarks>
		/// <para>Most of the buttons maintain their pressed/not-pressed state
		/// until the next command is sent, except the strum button will only be
		/// pressed momentarily.</para>
		/// </remarks>
		/// <todo>The overdrive control and whammy bar are not yet supported.</todo>
		/// <param name="ev">Specifies which buttons to press and release</param>
		private void WriteCommandPlayNotes(PlayNotesArgs ev)
		{
			// allow to run without m_port, for testing
			if (m_port != null && m_port.IsOpen)
			{
				Button b = Button.None;
				if (ev.PressGreen) b |= Button.Green;
				if (ev.PressRed) b |= Button.Red;
				if (ev.PressYellow) b |= Button.Yellow;
				if (ev.PressBlue) b |= Button.Blue;
				if (ev.PressOrange) b |= Button.Orange;
				if (ev.Strum) b |= Button.Strum;
				rgbWriteByte[0] = (byte)b;
				m_port.Write(rgbWriteByte, 0, 1);
			}
		}

		/// <summary>
		/// Queue to store events from the <see cref="DaveyBot.Brain"/> until the time comes to execute them.
		/// </summary>
		private Queue<PlayNotesArgs> m_queueEvents = new Queue<PlayNotesArgs>();
		
		/// <summary>
		/// Event handler that receives play-note commands from the <see cref="DaveyBot.Brain"/>
		/// and queues them up to be played at the proper time.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void brain_PlayNotes(object sender, PlayNotesArgs args)
		{
			// Ignore events if the handler thread isn't running.
			if (m_fIsRunning)
			{
				lock (m_queueEvents)
					m_queueEvents.Enqueue(args);
			}
		}

		/// <summary>Thread to play queued notes at the right time</summary>
		Thread m_thread = null;

		/// <summary>Is the note-playing thread running?</summary>
		public bool IsRunning { get { return m_fIsRunning; } set { m_fIsRunning = value; } }
		private bool m_fIsRunning = false;

		/// <summary>Flag to terminate the note-playing thread</summary>
		private bool m_fAbort = false;

		/// <summary>Milliseconds to sleep when polling the event queue</summary>
		private static TimeSpan dtSleep = new TimeSpan(10 * 10000);
		/// <summary>Handle events that are slightly in the future</summary>
		private static TimeSpan dtAhead = new TimeSpan(dtSleep.Ticks / 4);
		/// <summary>Ignore events that are old and stale</summary>
		private static TimeSpan dtOld = new TimeSpan(200 * 10000);

		/// <summary>Lock variable for Start and Stop methods</summary>
		private object oLockStartStop = new Object();

		/// <summary>
		/// Start the thread that plays notes from the queue.
		/// </summary>
		public void Start()
		{
			lock (oLockStartStop)
			{
				if (!m_fIsRunning)
				{
					// Open the serial port to talk to the controller hardware.
					// NOTE: For testing, we allow it to run if no serial port exists.
					try
					{
						OpenSerialPort();
					}
					catch (Exception)
					{
						MessageBox.Show("Failed to open the serial port");
					}

					// Write an "all notes off" command first.
					WriteCommandOff();

					// Start the background note-playing thread.
					if (m_thread != null)
						throw new InvalidOperationException("Controller thread already exists");
					// Clear out any old leftover events before starting.
					lock (m_queueEvents)
						m_queueEvents.Clear();
					// Create & start thread.
					m_fAbort = false;
					m_thread = new Thread(new ThreadStart(HandleTimedEvents));
					m_thread.Start();
					// Wait for the thread to get going.
					for (int i = 0; !m_fIsRunning; i++)
					{
						if (i >= 1000)
						{
							m_thread = null;
							throw new ApplicationException("Controller thread failed to start");
						}
						Thread.Sleep(1);
					}
				}
			}
		}

		/// <summary>
		/// Stop the thread that is playing notes from the queue.
		/// </summary>
		public void Stop()
		{
			lock (oLockStartStop)
			{
				if (m_fIsRunning)
				{
					if (m_thread == null)
						throw new InvalidOperationException("Controller thread doesn't exist");
					// Signal the thread to quit.
					m_fAbort = true;
					// Wait for the thread to stop.
					if (!m_thread.Join(1000))
						throw new ApplicationException("Controller thread failed to stop");
					m_thread = null;
					// Write an "all notes off" command last.
					WriteCommandOff();
					CloseSerialPort();
				}
			}
		}

		/// <summary>
		/// Background thread proc that removes note events from the queue
		/// and plays the notes.
		/// </summary>
		private void HandleTimedEvents()
		{
			m_fIsRunning = true;
			try
			{
				// Handle events in the event queue, at the appropriate times.
				for (; ; )
				{
					// Handle any events whose time has come.
					// Note that the events are queued in chronological order.
					DateTime tNow = DateTime.Now;
					DateTime tStart = tNow - dtOld;
					DateTime tEnd = tNow + dtAhead;
					PlayNotesArgs playNotes;
					DateTime tStrum;
					for (; ; )
					{
						// If the next event is due to be handled, remove it from the queue.
						// Otherwise we're done for now.
						lock (m_queueEvents)
						{
							if (m_queueEvents.Count <= 0)
								break;	// No more events, for now.
							playNotes = m_queueEvents.Peek();
							tStrum = playNotes.When;
							if (tStrum > tEnd)
								break;	// All events that were due have been handled.
							m_queueEvents.Dequeue();
						}
						if (tStrum < tStart)
						{
							// This event is old & stale - ignore it.
						}
						else
						{
							// Handle the event.
							// Send a command to the hardware controller.
							WriteCommandPlayNotes(playNotes);
							// Raise an event, if anyone's interested.
							playNotes.When = DateTime.Now;
							OnPlayNotes(playNotes);
						}
					}

					// Check if thread exit was requested.
					if (m_fAbort)
						break;

					// Take a short nap to wait for more events.
					Thread.Sleep(dtSleep);
				}
			}
			finally
			{
				m_fIsRunning = false;
			}
		}
	}
}
