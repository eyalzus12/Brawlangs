using Godot;
using System;

public static class MathUtils
{
	public static float Lerp(this ref float val, float to, float @by)
	{
		//return (val -= (val-to)*@by);
		if(val.MoreThan(to)) val -= @by;
		else 
		{
			val += @by;
			if(val.MoreThan(to)) val = to;
			return val;
		}
		
		return val;
		//return val = (val += ((to-@from)/@by).Clamp(@from, to))
	}
	
	public static void TruncateIfInsignificant(this ref float i)
	{
		if(Math.Abs(i) < 1f) i = 0f;
	}
	
	public static void TruncateIfInsignificant(this Vector2 v)
	{
		if(Math.Abs(v.x) < 1f) v.x = 0f;
		if(Math.Abs(v.y) < 1f) v.y = 0f;
	}
	
	public static float Sign(this float f) => Math.Sign(f);
	
	public static bool IsInRange(this float i, float min, float max,
		bool incmin=true, bool incmax=true) =>
		(i>min||(incmin&&i==min))&&(i<max||(incmax&&i==max));
		
	public static bool MoreThan(this float i, float j) => (j < 0)?(i < j):(i > j);
	
	//public static float Abs(this ref float i) => (i = Math.Abs(i));
	//public static float ClampMinMax(this ref float i, float min, float max) => (i = Math.Min(Math.Max(i, min), max));
	//public static float ClampMax(this ref float i, float max) => (i = Math.Min(i, max));
	//public static float ClampMin(this ref float i, float min) => (i = Math.Max(i, min));
	
	public static float Bias01(this float v, float b)
	{
		var k = (float)Math.Pow(1f-b, 3f);
		return (v*k)/(v*k-v+1f);
	}
	
	public static int AddIf(this int i, int j, bool predicate) => i + (predicate?0:j);
	public static int SubIf(this int i, int j, bool predicate) => i - (predicate?0:j);
	
	public static Vector2 Abs(this Vector2 v) => new Vector2(Math.Abs(v.x), Math.Abs(v.y));
}
