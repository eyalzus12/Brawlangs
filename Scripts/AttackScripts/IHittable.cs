using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	int TeamNumber{get; set;}
	void HandleGettingHit(HitData data);
	
	float DamageTakenMult{get;set;}
	float KnockbackTakenMult{get;set;}
	int StunTakenMult{get;set;}
	
	List<Hurtbox> Hurtboxes{get;set;}
}
