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
	
	bool IsHitting{get; set;}
	IHittable LastHit{get; set;}
	
	Attack CurrentAttack{get; set;}
	Dictionary<string, Attack> Attacks{get; set;}
	
	AudioManager Audio{get; set;}
	
	bool CanHit(IHittable hitObject);
	void HandleHitting(HitData data);
}
