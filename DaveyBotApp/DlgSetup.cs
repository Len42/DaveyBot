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
	/// Dialog to set up various application settings - video capture, etc.
	/// </summary>
	/// <remarks>
	/// The video capture settings are updated live while the dialog is up,
	/// in order for the Select Video Input button to work.
	/// Other settings are just edited in this dialog, and managed elsewhere.
	/// </remarks>
    public partial class DlgSetup : Form
    {
		/// <summary>
		/// The video capture object, needed to display and edit the video settings
		/// </summary>
		public Eyes Eyes { get { return m_eyes; } set { m_eyes = value; } }
		private Eyes m_eyes;

		/// <summary>
		/// The serial comm port used to communicate with the hacked game controller
		/// </summary>
		public string CommPort { get { return m_stCommPort; } set { m_stCommPort = value; } }
		private string m_stCommPort = null;

		/// <summary>
		/// The note-detection algorithm to use (a class name)
		/// </summary>
		/// <remarks>Must be a subclass of <see cref="DaveyBot.NoteFinder"/>.</remarks>
		public string NoteFinder { get { return m_stNoteFinder; } set { m_stNoteFinder = value; } }
		private string m_stNoteFinder = null;

		/// <summary>
		/// Image lag adjustment (milliseconds)
		/// </summary>
		/// <remarks>
		/// <para>The video capture can introduce quite a bit of lag. We can accomodate
		/// that without having to mess up the settings in the game.</para>
		/// <para>This value is *subtracted* but we show and store it as a positive number
		/// to avoid confusing users with minus signs.</para>
		/// </remarks>
		public int LagAdjust { get { return m_nLagAdjust; } set { m_nLagAdjust = value; } }
		private int m_nLagAdjust = 0;

		/// <summary>
		/// If true, delay one frame before playing notes.
		/// </summary>
		/// <seealso cref="Brain.m_fDelayStrum"/>
		public bool DelayStrum { get { return m_fDelayStrum; } set { m_fDelayStrum = value; } }
		private bool m_fDelayStrum = false;

		public DlgSetup()
        {
            InitializeComponent();
        }

        private void DlgVideoSetup_Load(object sender, EventArgs e)
        {
			m_eyes.FillCaptureSourceList(comboVideoSource.Items);
			comboVideoSource.SelectedItem = m_eyes.VideoSourceName;

			foreach (string st in System.IO.Ports.SerialPort.GetPortNames())
				comboCommPort.Items.Add(st);
			comboCommPort.Text = m_stCommPort;

			ObjectUtils.FillSubclassNameList(typeof(NoteFinder), comboNoteFinder.Items);
			comboNoteFinder.SelectedItem = m_stNoteFinder;

			textLagAdjust.Text = m_nLagAdjust.ToString();

			chkDelayStrum.Checked = m_fDelayStrum;
        }

		private void comboVideoSource_SelectionChangeCommitted(object sender, EventArgs e)
		{
			// Re-build the capture graph now so that the user can immediately
			// bring up the crossbar dialog to select an input.
			bool fPlaying = m_eyes.IsVideoPlaying;
			m_eyes.VideoSourceName = comboVideoSource.SelectedItem.ToString();
			m_eyes.BuildCaptureGraph();
			if (fPlaying)
				m_eyes.PlayVideo();
		}

		private void btnCrossbar_Click(object sender, EventArgs e)
		{
			m_eyes.DoCrossbarDialog(this);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (!Int32.TryParse(textLagAdjust.Text, out m_nLagAdjust))
			{
				DialogResult = DialogResult.None;
				MessageBox.Show("Invalid number entered for Lag Adjustment");
			}
			m_stCommPort = comboCommPort.Text;
			m_fDelayStrum = chkDelayStrum.Checked;
			m_stNoteFinder = comboNoteFinder.Text;
		}
    }
}
