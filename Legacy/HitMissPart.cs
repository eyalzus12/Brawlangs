using Godot;
using System;

public class HitMissPart : AttackPart
{
	//public bool hit = false;
	public int ExtraEndlagOnMiss = 0;
	
	public override void Init()
	{
		LoadExtraProperty<int>("ExtraEndlagOnMiss");
	}
	
	public override void OnStart()
	{
		hit = false;
	}
	
	public override void OnHit(Hitbox hitbox, Area2D hurtbox)
	{
		//hit = true;
	}
	
	public override int GetEndlag() => endlag + (hit?0:ExtraEndlagOnMiss);
}
