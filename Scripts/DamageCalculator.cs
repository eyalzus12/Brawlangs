using Godot;
using System;

public class DamageCalculator
{
	//public readonly static float[] MULTIPLIERS = {0.02f, 0.008f, 0.012f, 0.005f, 0.004f, 0.0052f};
	//public readonly static float[] MULTIPLIERS = {0.02f, 0.008f, 0.012f, 0.0066f, 0.0066f, 0.0066f};
	public readonly static float[] MULTIPLIERS = {1f, 0.4f, 0.06f, 0.33f, 0.33f, 0.33f};
	public readonly static int[] INDEXES = {2, 1, 1, 0, 0, 0};
	//5/250	2/250	3/250	1.25/250	1/250	1.3/250
	//5 2 3 1.25 1.3
	//*50
	//1 -- 0.4 0.6 -- 0.25 0.2 0.26 -- 0.29
	public const float jump = 50f;
	public const int length = 6;
	
	public const float MAX_DAMAGE = 300f;
	
	public static Color DamageToColor(float damage)
	{
		Color color = default;
		color.a = 1f;
		var jump = MAX_DAMAGE/INDEXES.Length;
		for(int i = 0; i < length && damage > 0f; ++i, damage -= jump)
		{
			var reduce = MULTIPLIERS[i] * Math.Min(damage/jump, 1f);
			switch(INDEXES[i])
			{
				case 0:
					color.r += reduce;
					 break;
				case 1:
					color.g += reduce;
					break;
				case 2:
					color.b += reduce;
					break;
				default:
					GD.Print("Improper value for color index {INDEXES[i]} at index {i}");
					break;
			}
		}
		
		return color.Inverted();
	}
}
