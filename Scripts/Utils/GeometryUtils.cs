using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class GeometryUtils
{
	public static Vector2 ClockwisePerpendicular(this Vector2 v) => new Vector2(-v.y, v.x);
	public static Vector2 CounterClockwisePerpendicular(this Vector2 v) => new Vector2(v.y, v.x);
	public static Vector2 AntiClockwisePerpendicular(this Vector2 v) => v.CounterClockwisePerpendicular();
	
	public static Vector2 Round(this Vector2 v, int point = 0) => new Vector2(
			(float)Math.Round(v.x, point),
			(float)Math.Round(v.y, point)
		);
		
	public static Vector3 Round(this Vector3 v, int point = 0) => new Vector3(
			(float)Math.Round(v.x, point),
			(float)Math.Round(v.y, point),
			(float)Math.Round(v.z, point)
		);
		
	public static Quat Round(this Quat q, int point = 0) => new Quat(
			(float)Math.Round(q.x, point),
			(float)Math.Round(q.y, point),
			(float)Math.Round(q.z, point),
			(float)Math.Round(q.w, point)
		);
	
	public static Vector2 TiltToNormal(this Vector2 v, Vector2 normal)
	{
		if(normal == Vector2.Zero) return v;
		
		Vector2 norm = normal.Normalized();
		Vector2 tilt = norm.ClockwisePerpendicular();
		
		return (norm * -v.y) + (tilt * v.x);
	}
	
	public static Vector2 Car2Pol(this Vector2 v)
	{
		float radius = (float)Math.Sqrt((v.x * v.x) + (v.y * v.y));//distance
		float angle = (float)Math.Atan2(v.y, v.x);//this function was literally made for this
		return new Vector2(radius, angle);
	}
	
	public static Vector2 Pol2Car(this Vector2 v)
	{
		float x = v.x * (float)Math.Cos(v.y);//stole from google
		float y = v.x * (float)Math.Sin(v.y);
		return new Vector2(x, y);
	}
	
	public static Vector2 RotByCircle(this Vector2 v, float deg, Vector2 center = default)
	{
		Vector2 dist = v - center;//get relative distance from center
		Vector2 pol = dist.Car2Pol();//turn to polar
		pol.y += deg;//add degrees
		Vector2 car = pol.Pol2Car();//turn back to cartesian
		Vector2 res = car + center;//get new position from relative position
		return res;
	}
	
	public static Vector2 Center(this Rect2 rec) => new Vector2(
		rec.Position.x + rec.Size.x / 2,
		rec.Position.y + rec.Size.y / 2
	);
		
	public static Rect2 Limit(this Rect2 rec, float maxH, float maxV)
	{
		//GD.Print(rec);
		Vector2 pos = rec.Position.Clamp(-maxH, maxH, -maxV, maxV);
		//keep top right cornet inside bounding box
		
		Vector2 end = rec.End.Clamp(-maxH, maxH, -maxV, maxV);
		//keep bottom right corner inside bounding box
		
		Vector2 size = end-pos;
		//get size
		
		return new Rect2(pos, size);
	}
	
	public static Rect2 Zoomed(this Rect2 r, Vector2 zoom) => RectFrom(r.Center(), r.Size*zoom/2f);
	
	public static float Clamp(this float f, float m, float M) => Math.Min(M, Math.Max(m, f));
		
	public static Vector2 Clamp(this Vector2 v, float mx, float Mx,
		float my, float My) =>
	new Vector2(
		v.x.Clamp(mx, Mx),
		v.y.Clamp(my, My)
	);
	
	public static IEnumerable<(Vector2,Vector2)> SplitToParts(Vector2 v1, Vector2 v2, int amount, float margin)
	{
		v1 = v1.MoveToward(v2, margin);
		v2 = v2.MoveToward(v1, margin);
		var length = v1.DistanceTo(v2);
		var partlength = length/amount;
		var leftedge = v1;
		for(int i = 0; i < amount; ++i)
		{
			var rightedge = leftedge.MoveToward(v2, partlength);
			yield return (leftedge, rightedge);
			leftedge = rightedge;
		}
	}
	
	public static Vector2 CenterBetween(Vector2 v1, Vector2 v2) => (v1+v2)/2f;
	
	public static IEnumerable<Vector2> PartCenters(this IEnumerable<(Vector2,Vector2)> vs) => vs.Select(p=>CenterBetween(p.Item1, p.Item2));
	
	public static IEnumerable<Vector2> GetLabelLocations(this int[] playercounts, Vector2 left, Vector2 right, float margin)
	{
		foreach(((Vector2, Vector2) teampart, int playercount) in SplitToParts(left, right, playercounts.Length, margin).Zip(playercounts))
			foreach(var l in SplitToParts(teampart.Item1, teampart.Item2, playercount, margin).PartCenters())
				yield return l;
	}
	
	public static void DrawCapsuleShape(this CanvasItem ci, CapsuleShape2D shape, Vector2 position, float rotation, Color color)
	{
		var height = shape.Height;
		var radius = shape.Radius;
		ci.DrawSetTransform(position, rotation, Vector2.One);
		var middleRect = GeometryUtils.RectFrom(Vector2.Zero, new Vector2(radius, height/2));
		ci.DrawRect(middleRect, color);
		ci.DrawCircle(new Vector2(0, height/2), radius, color);
		ci.DrawCircle(new Vector2(0, -height/2), radius, color);
	}
	
	public static Vector2 SmoothLinearInterpolate(this Vector2 @from, Vector2 to, float weight, float curve) => @from.LinearInterpolate(to, weight*Mathf.Tanh(curve*@from.DistanceSquaredTo(to)));
	
	public static Rect2 RectFrom(Vector2 Center, Vector2 Size) => new Rect2(Center-Size, 2*Size);
}
