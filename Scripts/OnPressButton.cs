using Godot;
using System;

public class OnPressButton : Button
{
	public override void _Ready()
	{
		Connect("pressed", this, nameof(OnPress));
	}
	
	public virtual void OnPress()
	{
		
	}
}
