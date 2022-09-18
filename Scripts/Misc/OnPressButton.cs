using Godot;
using System;

public partial class OnPressButton : Button
{
	public override void _Ready()
	{
		Connect("pressed",new Callable(this,nameof(OnPress)));
	}
	
	public virtual void OnPress()
	{
		
	}
}
