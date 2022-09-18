using Godot;
using System;

public partial class HButton : Button
{
	public override void _Ready()
	{
		Connect("pressed",new Callable(this,nameof(OnPress)));
	}
	
	public void OnPress()
	{
		GD.Print("h");
	}
}
