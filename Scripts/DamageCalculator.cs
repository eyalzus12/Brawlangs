using Godot;
using System;

public class DamageCalculator
{
	public readonly static float[] multipliers = {0.02f, 0.008f, 0.012f, 0.005f, 0.004f, 0.0052f};
	public const float jump = 50f;
	public const int length = 6;
	
	public static Color DamageToColor(float damage)
	{
		Color color = default;
		color.a = 1f;
		for(int i = 0; i < length && damage > 0f; ++i, damage -= jump)
		{
			var reduce = multipliers[i] * Math.Min(damage, jump);
			switch(i)
			{
				case 0:
					color.b += reduce;
					 break;
				case 1:
				case 2:
					color.g += reduce;
					break;
				case 3:
				case 4:
				case 5:
					color.r += reduce;
					break;
				default:
					GD.Print("how did a number from 0 to 5 get detected as anything other than that????");
					break;
			}
		}
		
		return color.Inverted();
	}
}
