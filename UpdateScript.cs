using Godot;
using System;
using System.Linq;

public class UpdateScript : Node
{
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("toggle_fullscreen"))
			OS.WindowFullscreen = !OS.WindowFullscreen;
	}
	
	public override void _Ready()
	{
		GD.Print(DamageCalculator.DamageToColor(0)*255);
		GD.Print(DamageCalculator.DamageToColor(10)*255);
		GD.Print(DamageCalculator.DamageToColor(50)*255);
		GD.Print(DamageCalculator.DamageToColor(100)*255);
		GD.Print(DamageCalculator.DamageToColor(150)*255);
		GD.Print(DamageCalculator.DamageToColor(300)*255);
		//IterUtils.Flatten({1, 4, 5, 6}, {9, 6, 7, 4}, {"hag", "esah", "agsag"}).Debug();
		//(new int[] {1, 5, 6, 4, 0, 9, 15, 1251, 125, 87}).Indexed().Cast<object>().Debug();
	}
}
