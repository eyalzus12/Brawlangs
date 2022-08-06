using Godot;
using System;
using System.Collections.Generic;

public interface IAttacker
{
	Vector2 Position{get; set;}
	int Direction{get; set;}
	
	float DamageDoneMult{get; set;}
	float KnockbackDoneMult{get; set;}
	float StunDoneMult{get; set;}
	int TeamNumber{get; set;}
	
	bool Hitting{get; set;}
	IHittable LastHitee{get; set;}
	
	bool Clashing{get; set;}
	HitData ClashData{get; set;}
	void HandleClashing(HitData data);
	
	Attack CurrentAttack{get; set;}
	Dictionary<string, Attack> Attacks{get; set;}
	
	AudioManager Audio{get; set;}
	
	bool CanHit(IHittable hitObject);
	void HandleHitting(HitData data);
}
