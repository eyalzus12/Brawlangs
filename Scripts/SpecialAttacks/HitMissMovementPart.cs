using Godot;
using System;

public class HitMissMovementPart : AttackPart
{
	public bool hit = false;
	public int ExtraEndlagOnMiss = 0;
	public Vector2 Movement = Vector2.Zero;
	
	public override void Init()
	{
		LoadExtraProperty<int>("ExtraEndlagOnMiss");
		LoadExtraProperty<Vector2>("Movement");
	}
	
	public override void OnStart()
	{
		hit = false;
		ch.vec = Movement * new Vector2(ch.direction, 1);
		if(ch.grounded) ch.vec.y = State.VCF;
	}
	
	public override void CalculateNextPart()
	{
		ChangePart(hit?"Hit":"Miss");
	}
	
	public override void OnHit(Hitbox hitbox, Area2D hurtbox)
	{
		hit = true;
	}
	
	public override int GetEndlag() => endlag + (hit?0:ExtraEndlagOnMiss);
}
