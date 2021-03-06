using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	int TeamNumber{get; set;}
	void HandleGettingHit(HitData data);
	int InvincibilityLeft{get; set;}
	
	float DamageTakenMult{get;set;}
	float KnockbackTakenMult{get;set;}
	float StunTakenMult{get;set;}
	
	List<Hurtbox> Hurtboxes{get;set;}
}
