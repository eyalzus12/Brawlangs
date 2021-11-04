using Godot;
using System;

public class NewTextEditButton : Button
{
	public VBoxContainer ilist;
	
	public override void _Ready()
	{
		ilist = GetParent().GetNode("List") as VBoxContainer;
		Connect("pressed", this, "OnPress");
	}
	
	public void OnPress()
	{
		var le = new LineEdit();
		ilist.AddChild(le);
		le.Connect("text_entered", this, "Enter");
		//le.Connect("focus_entered", this, "SetCurrent");
		///le.Connect("focus_exited", this, "RemoveCurrent");
	}
	
	public void Enter(string new_text)
	{
		OnPress();
	}
}
