using Godot;
using System;
using System.Windows.Forms;
using Linearstar.Windows.RawInput;

public class RawInputReceiverForm : Form
{
	public event EventHandler<RawInputKeyEvent> KeycodePress;
	public RawInputReceiverWindow Window{get; set;}
	
	public RawInputReceiverForm()
	{
		FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		ShowInTaskbar = false;
		Opacity = 0;
		Size = new System.Drawing.Size(0, 0);
		Window = new RawInputReceiverWindow();
		Window.KeycodePress += (sender, t) => KeycodePress?.Invoke(this, t);
	}
	
	// hiding from Alt+TAB dialog
	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= 0x80;
			return cp;
		}
	}
}
