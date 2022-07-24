using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	Vector2 Position{get; set;}
	
	int TeamNumber{get; set;}
	void HandleGettingHit(HitData data);
	
	float DamageTakenMult{get;set;}
	float KnockbackTakenMult{get;set;}
	float StunTakenMult{get;set;}
	
	bool IsInvincible();
	
	List<Hurtbox> Hurtboxes{get;set;}
}
