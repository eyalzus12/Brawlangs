using Godot;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using RawInput_dll;

public class RawInputReceiverForm : Form
{
	public event EventHandler<RawInputEventArg> KeyPressed;
	public RawInput RawInputWindow{get; set;}
	
	public RawInputReceiverForm()
	{
		FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		ShowInTaskbar = false;
		Opacity = 0;
		Size = new System.Drawing.Size(0, 0);
		
		//Window = new RawInputReceiverWindow(this);
		RawInputWindow = new RawInput(this.Handle, false);
		RawInputWindow.AddMessageFilter();
		RawInputWindow.KeyPressed += (sender, e) => KeyPressed?.Invoke(sender, e);
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
