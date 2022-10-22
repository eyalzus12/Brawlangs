
using Godot;
using System;

public static class MathUtils
{
	public static float Towards(this ref float val, float to, float @by)
	{
		if(val > to)
		{
			val -= @by;
			if(val < to) val = to;
		}
		else
		{
			val += @by;
			if(val > to) val = to;
		}
		
		return val;
	}
	
	public static Vector2 Towards(this ref Vector2 v, Vector2 to, Vector2 @by)
	{
		v.x.Towards(to.x, @by.x);
		v.y.Towards(to.y, @by.y);
		return v;
	}
	
	public static void TruncateIfInsignificant(this ref float i)
	{
		if(Math.Abs(i) < 1f) i = 0f;
	}
	
	public static void TruncateIfInsignificant(this ref Vector2 v)
	{
		if(Math.Abs(v.x) < 1f) v.x = 0f;
		if(Math.Abs(v.y) < 1f) v.y = 0f;
	}
	
	public static float Bias01(this float v, float b)
	{
		var k = Mathf.Pow(1f-b, 3f);
		return (v*k)/(v*k-v+1f);
	}
	
	public static Vector2 Abs(this Vector2 v) => new Vector2(Math.Abs(v.x), Math.Abs(v.y));
	public static Vector2 Max(this Vector2 v1, Vector2 v2) => new Vector2(Math.Max(v1.x,v2.x), Math.Max(v1.y,v2.y));
	public static Vector2 Max(this Vector2 v1, float f1, float f2) => new Vector2(Math.Max(v1.x,f1), Math.Max(v1.y,f2));
	public static Vector2 Min(this Vector2 v1, Vector2 v2) => new Vector2(Math.Min(v1.x,v2.x), Math.Min(v1.y,v2.y));
	public static Vector2 Min(this Vector2 v1, float f1, float f2) => new Vector2(Math.Min(v1.x,f1), Math.Min(v1.y,f2));
	
	public static float CopySign(this float f1, float f2) => Mathf.Abs(f1)*Mathf.Sign(f2);
}
