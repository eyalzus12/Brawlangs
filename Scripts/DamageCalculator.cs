using Godot;
using System;
using System.Linq;

public class DamageCalculator
{
	//public readonly static float[] MULTIPLIERS = {0.02f, 0.008f, 0.012f, 0.005f, 0.004f, 0.0052f};
	//public readonly static float[] MULTIPLIERS = {0.02f, 0.008f, 0.012f, 0.0066f, 0.0066f, 0.0066f};
	//public readonly static float[] MULTIPLIERS = {1f, 0.4f, 0.6f, 0.2f, 0.4f, 0.4f};
	//public readonly static int[] INDEXES = {2, 1, 1, 0, 0, 0};
	
	public readonly static float[][] VALUES = {
		new float[]{1f},
		new float[]{0.4f,0.6f},
		new float[]{0.33f,0.33f,0.33f}
	};
	
	public readonly static int[] ORDER = {2, 1, 0};
	
	public const float MAX_DAMAGE = 300f;
	
	public static int length = VALUES.Aggregate(0,(a,v)=>a+v.Length);
	public static float jump = MAX_DAMAGE/length;
	
	public static Color DamageToColor(float damage)
	{
		Color color = default;
		color.a = 1f;
		
		foreach(var i in ORDER)
		{
			for(int j = 0; j < VALUES[i].Length && damage > 0f; ++j, damage -= jump)
			{
				var addition = VALUES[i][j] * Math.Min(damage/jump, 1f);
				switch(i)
				{
					case 0:
						color.r += addition;
						 break;
					case 1:
						color.g += addition;
						break;
					case 2:
						color.b += addition;
						break;
					default:
						GD.Print("Improper value for color index {INDEXES[i]} at index {i}");
						break;
				}
			}
		}
		
		/*var length = INDEXES.Length;
		for(int i = 0; i < length && damage > 0f; ++i, damage -= jump)
		{
			var addition = MULTIPLIERS[i] * Math.Min(damage/jump, 1f);
			switch(INDEXES[i])
			{
				case 0:
					color.r += addition;
					 break;
				case 1:
					color.g += addition;
					break;
				case 2:
					color.b += addition;
					break;
				default:
					GD.Print("Improper value for color index {INDEXES[i]} at index {i}");
					break;
			}
		}*/
		
		return color.Inverted();
	}
}
