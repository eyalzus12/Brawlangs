using Godot;
using System;
using System.Windows.Forms;
using Linearstar.Windows.RawInput;
using RawKeyboardFlags = Linearstar.Windows.RawInput.Native.RawKeyboardFlags;

public class RawInputReceiverWindow : NativeWindow
{
	public event EventHandler<RawInputKeyEvent> KeycodePress;
	
	public RawInputReceiverWindow(Form parent)
	{
		CreateHandle(new CreateParams{
			X = 0,
			Y = 0,
			Width = 0,
			Height = 0,
			Style = 0x800000,
			Parent = parent.Handle,
		});
	}
	
	const int WM_INPUT = 0x00FF;
	protected override void WndProc(ref Message m)
	{
		base.WndProc(ref m);
		if(m.Msg == WM_INPUT)
		{
			var data = RawInputData.FromHandle(m.LParam);
			if(data is RawInputKeyboardData kdata)
			{
				var keyboard = kdata.Keyboard;
				var header = kdata.Header;
				var handle = header.DeviceHandle;
				var key = keyboard.VirutalKey;
				var pressed = (keyboard.Flags & RawKeyboardFlags.Up) == 0;
				
				var input = new RawInputKeyEvent(key, pressed, handle);
				KeycodePress?.Invoke(this, input);
			}
		}
	}
}
