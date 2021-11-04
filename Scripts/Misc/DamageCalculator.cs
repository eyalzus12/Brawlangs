using Godot;
using System;
using System.Linq;

public class DamageCalculator
{
	public readonly static (float,Color)[] VALUES = 
	{
		(50f, new Color(0f, 0f, 1f)),
		(50f, new Color(0f, 0.4f, 0f)),
		(50f, new Color(0f, 0.6f, 0f)),
		(50f, new Color(64f/255f, 0f, 0f)),
		(50f, new Color(0.2f, 0f, 0f)),
		(50f, new Color(66f/255f, 0f, 0f))
	};
	
	public static Color DamageToColor(float damage)
	{
		var color = new Color(0f, 0f, 0f, 1f);//black
		
		foreach(var val in VALUES)
		{
			if(damage <= 0f) break;
			var step = val.Item1;//how much to go until next addition
			var amount = val.Item2;//how much to add by step
			color += amount * Math.Min(damage/step, 1f);//add
			damage -= step;//next addition
		}
		
		return color.Inverted();//make all that addition a reduction
	}
}
