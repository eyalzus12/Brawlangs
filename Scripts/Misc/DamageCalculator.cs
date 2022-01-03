using Godot;
using System;
using System.Linq;

public class DamageCalculator
{
	public readonly static (float,Color)[] VALUES = 
	{
		(50, new Color(0, 0, 255)),
		(50, new Color(0, 102, 0)),
		(50, new Color(0, 153, 0)),
		(50, new Color(64, 0, 0)),
		(50, new Color(51, 0, 0)),
		(50, new Color(66, 0, 0))
	};
	
	public static Color DamageToColor(float damage)
	{
		var color = new Color(0f, 0f, 0f, 1f);//black
		
		foreach(var val in VALUES)
		{
			if(damage <= 0f) break;
			var step = val.Item1;//how much to go until next addition
			var amount = val.Item2 / 255f;//how much to add by step
			color += amount * Math.Min(damage/step, 1f);//add
			damage -= step;//next addition
		}
		
		return color.Inverted();//make all that addition a reduction
	}
}
