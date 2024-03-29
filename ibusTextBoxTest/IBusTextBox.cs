using System;
using System.Linq;
using System.Windows.Forms;
using IBusDotNet;

// Ensure when running this test program on mono that environment variable MONO_WINFORMS_XIM_STYLE="disabled"
// This ensures X input methods aren't running.

namespace Test
{
	public class IBusTextBox : TextBox
	{
		private IBusConnection connection;
		private IInputContext inputContext;
		private InputBus ibus;

		// show the preedit text in here.
		private TextBox preeditTextBox;

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
				ibus = new InputBus(connection);
				inputContext = ibus.CreateInputContext("MyTextBox");
				// Console.WriteLine(inputContext.Introspect());

				inputContext.SetCapabilities(Capabilities.PreeditText |
					Capabilities.Focus | Capabilities.SurroundingText);

				// Engine can be programatically selected by:
				// inputContext.SetEngine("pinyin");

				inputContext.Enable();

				// Handle some important events:
				inputContext.CommitText += CommitTextEventHandler;
				inputContext.UpdatePreeditText += UpdatePreeditTextEventHandler;
				inputContext.ShowPreeditText += ShowPreeditTextEventHandler;
				inputContext.HidePreeditText += HidePreeditTextEventHandler;
				inputContext.ForwardKeyEvent += ForwardKeyEventHandler;
				inputContext.DeleteSurroundingText += DeleteSurroundingText;

				Console.WriteLine($"Input context engine: {inputContext.GetEngine()} Name: {inputContext.GetEngine().Name}");
				Console.WriteLine($"IBus address: {ibus.GetAddress()}");
				Console.WriteLine($"IBus current input context: {ibus.CurrentInputContext()}");
				string engines = string.Join(", ", ibus.ListEngines().Select(e => e.Name));
				string activeEngines = string.Join(", ", ibus.ListActiveEngines().Select(e => e.Name));
				Console.WriteLine($"IBus engines: {ibus.ListEngines().Length}\n{engines}");
				Console.WriteLine($"IBus active engines: {ibus.ListActiveEngines().Length}\n{activeEngines}");
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

			inputContext.SetEngine("table:thai");
			Console.WriteLine($"Switched to input context engine: {inputContext.GetEngine()} Name: {inputContext.GetEngine().Name}");
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
						if (inputContext.ProcessKeyEvent((int)msg.WParam, 0, 0))
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

		void UpdatePreeditTextEventHandler(object text, uint cursorPos, bool visible)
		{
			preeditTextBox.Text = String.Empty;

			if (visible) {
				preeditTextBox.Show();
				IBusText t = IBusText.FromObject(text);
				preeditTextBox.Text = t.Text;
			}
		}

		void DeleteSurroundingText (int offset, uint nChars)
		{
			if (offset < 0)
				offset = Text.Length - (int)nChars;
			if (offset < 0)
			{
				offset = 0;
				nChars = (uint)Text.Length;
			}
			Text.Remove(offset, (int)nChars);
		}

		void CommitTextEventHandler(object text)
		{
			IBusText t = IBusText.FromObject(text);

			Text = t.Text;
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
