using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	Vector2 Position{get; set;}
	int Direction{get; set;}
	
	float DamageTakenMult{get; set;}
	float KnockbackTakenMult{get; set;}
	float StunTakenMult{get; set;}
	int TeamNumber{get; set;}
	
	bool GettingHit{get; set;}
	IHitter LastHitter{get; set;}
	
	List<Hurtbox> Hurtboxes{get; set;}
	
	InvincibilityManager IFrames{get; set;}
	bool Invincible{get;}
	
	void HandleGettingHit(HitData data);
}
