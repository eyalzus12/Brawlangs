using Godot;
using System;

public readonly struct CollisionSettings
{
	public readonly string Name;
	public readonly Vector2 CollisionExtents;
	public readonly Vector2 CollisionPosition;
	public readonly float HurtboxRadius;
	public readonly float HurtboxHeight;
	public readonly Vector2 HurtboxPosition;
	public readonly float HurtboxRotation;
	//public readonly UInt32 HitMask;
	
	public CollisionSettings(string name, Vector2 colex, Vector2 colpos, float hbrd, float hbhe, Vector2 hbpos, float hrot/*, UInt32 hm*/)
	{
		Name = name;
		CollisionExtents = colex;
		CollisionPosition = colpos;
		HurtboxRadius = hbrd;
		HurtboxHeight = hbhe;
		HurtboxPosition = hbpos;
		HurtboxRotation = hrot;
		//HitMask = hm;
	}
}
