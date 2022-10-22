using Godot;
using System;
using System.Collections.Generic;

public class DamageCalculator
{
	public static readonly (float,Color)[] VALUES =
	{
		(0, new Color(1, 1, 1)),
		(50, new Color(1, 1, 0)),
		(100, new Color(1, 0.6f, 0)),
		(150, new Color(1, 0, 0)),
		(200, new Color(0.75f, 0, 0)),// 191/255
		(250, new Color(0.55f, 0, 0)),// 140/255
		(300, new Color(0.3f, 0, 0))// 74/255
	};
	
	private static readonly FloatColorTupleComparer COMPARER = new FloatColorTupleComparer();
	public static Color DamageToColor(float damage)
	{
		if(damage <= VALUES[0].Item1) return VALUES[0].Item2;
		if(damage >= VALUES[VALUES.Length-1].Item1) return VALUES[VALUES.Length-1].Item2;
		
		int idx = Array.BinarySearch<(float,Color)>(VALUES, (damage, new Color()), COMPARER);
		
		if(idx < 0) idx = ~idx;
		
		if(idx == 0) return VALUES[0].Item2;
		if(idx >= VALUES.Length) return VALUES[VALUES.Length-1].Item2;
		
		var prevDamage = VALUES[idx-1].Item1;
		var nextDamage = VALUES[idx].Item1;
		var prevColor = VALUES[idx-1].Item2;
		var nextColor = VALUES[idx].Item2;
		var weight = (damage-prevDamage)/(nextDamage-prevDamage);
		return prevColor.LinearInterpolate(nextColor, weight);
	}
	
	private class FloatColorTupleComparer : IComparer<(float,Color)>
	{
		public int Compare((float,Color) x, (float,Color) y) => x.Item1.CompareTo(y.Item1);
	}
}
