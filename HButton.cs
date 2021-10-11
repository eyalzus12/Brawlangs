using Godot;
using System;

public class HButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this, nameof(OnPress));
	}
	
	public void OnPress()
	{
		GD.Print("h");
	}
}
