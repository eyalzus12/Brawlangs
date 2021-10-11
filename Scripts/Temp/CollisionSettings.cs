using Godot;
using System;

public readonly struct CollisionSettings
{
	public readonly Vector2 CollisionExtents;
	public readonly Vector2 CollisionPosition;
	public readonly float HurtboxRadius;
	public readonly float HurtboxHeight;
	public readonly Vector2 HurtboxPosition;
	//public readonly UInt32 HitMask;
	
	public CollisionSettings(Vector2 colex, Vector2 colpos, float hbrd, float hbhe, Vector2 hbpos/*, UInt32 hm*/)
	{
		CollisionExtents = colex;
		CollisionPosition = colpos;
		HurtboxRadius = hbrd;
		HurtboxHeight = hbhe;
		HurtboxPosition = hbpos;
		//HitMask = hm;
	}
}
