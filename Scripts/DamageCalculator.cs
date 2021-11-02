using Godot;
using System;
using System.Linq;

public class DamageCalculator
{
	
	public readonly static float[] VALUES = {1f, 0.4f,0.6f,0.33f,0.33f,0.33f};
	public readonly static int[] ORDER = {2, 1, 1, 0, 0, 0};
	
	public const float MAX_DAMAGE = 300f;
	
	public static int length = VALUES.Length;
	public static float jump = MAX_DAMAGE/length;
	
	public static Color DamageToColor(float damage)
	{
		var color = new Color(1f, 1f, 1f, 1f);
		
		for(int i = 0; i < length && damage > 0f; ++i, damage -= jump)
		{
			var reduce = VALUES[i] * Math.Min(damage/jump, 1f);
			switch(ORDER[i])
			{
				case 0: color.r -= reduce; break;
				case 1: color.g -= reduce; break;
				case 2: color.b -= reduce; break;
				default: GD.Print("Improper value for color index {ORDER[i]} at index {i}"); break;
			}
		}
		
		return color;
	}
}
