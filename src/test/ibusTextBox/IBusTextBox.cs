using System;
using System.Windows.Forms;
using IBusDotNet;

// Ensure when running this test program on mono that environment variable MONO_WINFORMS_XIM_STYLE="disabled"
// This ensures X input methods aren't running.

namespace Test
{
	public class IBusTextBox : TextBox
	{
		IBusConnection connection = null;
		InputContext inputContext = null;
		IBusDotNet.InputBusWrapper ibus = null;

		// show the preedit text in here.
		TextBox preeditTextBox;

		public IBusTextBox(TextBox textBox)
		{
			preeditTextBox = textBox;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (connection != null)
					connection.Dispose();
			}
			connection = null;

			base.Dispose(disposing);
		}

		#region overrides of winform methods.
		/// <summary>
		/// When the TextBox Control is created also create connection to ibus.
		/// </summary>
		protected override void OnCreateControl()
		{
			connection = IBusConnectionFactory.Create();
			if (connection != null)
			{
				ibus = new IBusDotNet.InputBusWrapper(connection);
				inputContext = ibus.InputBus.CreateInputContext("MyTextBox");
				// Console.WriteLine(inputContext.Introspect());

				inputContext.SetCapabilities(Capabilities.PreeditText |
											 Capabilities.Focus);

				// Engine can be programatically selected by:
				// inputContext.SetEngine("pinyin");

				inputContext.Enable();

				// Handle some important events:
				inputContext.CommitText += CommitTextEventHandler;
				inputContext.UpdatePreeditText += UpdatePreeditTextEventHandler;
				inputContext.ShowPreeditText += ShowPreeditTextEventHandler;
				inputContext.HidePreeditText += HidePreeditTextEventHandler;
				inputContext.ForwardKeyEvent += ForwardKeyEventHandler;
			}
			else
			{
				MessageBox.Show("Error couldn't connect to IBus. Is Ibus running?");
			}

			base.OnCreateControl();
		}

		protected override void OnGotFocus(EventArgs e)
		{
			if (inputContext != null)
			{
				inputContext.FocusIn();
			}
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (inputContext != null)
			{
				inputContext.FocusOut();
			}
			base.OnLostFocus(e);
		}

		protected override bool IsInputKey(Keys keyData)
		{
			if (inputContext != null)
			{
				// Ensure some keys which dont get sent as WM_CHAR are sent to the inputContext.
				switch (keyData) {
				case Keys.Delete:
				case Keys.Up:
				case Keys.Down:
				case Keys.Left:
				case Keys.Right:
				case Keys.Escape:
					inputContext.Reset();
					return true;
				}
			}

			return base.IsInputKey(keyData);
		}

		const int WM_CHAR = 258;
		const int WM_DESTROY = 2;

		protected override void WndProc(ref Message msg)
		{
			if (inputContext != null)
			{
				switch (msg.Msg) {
				case WM_CHAR:
					if (inputContext.ProcessKeyEvent((uint)msg.WParam, 0, 0))
						return;
					break;
				case WM_DESTROY:
					if (connection != null)
						connection.Close();
					break;
				}
			}

			base.WndProc(ref msg);
		}
		#endregion

		#region Handle ibus signals
		void HidePreeditTextEventHandler()
		{
			preeditTextBox.Text = String.Empty;
			preeditTextBox.Visible = false;

		}

		void ShowPreeditTextEventHandler()
		{
			// In this demo we are putting the preedit text in another box. (as it makes things simpler)
			preeditTextBox.Show();
		}

		void UpdatePreeditTextEventHandler(object text, uint cursor_pos, bool visible)
		{
			preeditTextBox.Text = String.Empty;

			if (visible) {
				preeditTextBox.Show();
				IBusText t = IBusText.FromObject(text);
				preeditTextBox.Text = t.Text;
			}
		}

		void CommitTextEventHandler(object text)
		{
			IBusText t = IBusText.FromObject(text);

			foreach (char c in t.Text)
			{
				Message m = new Message();
				m.Msg = 258;
				m.WParam = new IntPtr(c);
				base.WndProc(ref m);
			}
		}

		void ForwardKeyEventHandler (uint keyval, uint keycode, uint state)
		{

		}

		#endregion
	}

	class MainClass
	{
		public static void Main(string[] args)
		{
			var f = new Form();
			var preeditbox = new TextBox();
			var t = new IBusTextBox(preeditbox);

			preeditbox.ReadOnly = true;
			preeditbox.Dock = DockStyle.Bottom;

			f.Controls.Add(t);
			f.Controls.Add(preeditbox);
			f.ShowDialog();
		}
	}
}
