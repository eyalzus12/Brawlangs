using Godot;
using System;
using System.Collections.Generic;

public interface IAttacker
{
	string Name{get; set;}
	
	Vector2 Position{get; set;}
	int Direction{get; set;}
	int TeamNumber{get; set;}
	
	float DamageDoneMult{get; set;}
	float KnockbackDoneMult{get; set;}
	float StunDoneMult{get; set;}
	
	bool Hitting{get; set;}
	IHittable LastHitee{get; set;}
	
	bool FriendlyFire{get; set;}
	
	bool Clashing{get; set;}
	HitData ClashData{get; set;}
	void HandleClashing(HitData data);
	
	Attack CurrentAttack{get; set;}
	Dictionary<string, Attack> Attacks{get; set;}
	
	string AudioPrefix{get;}
	AudioManager Audio{get; set;}
	
	AnimationPlayer HitboxAnimator{get; set;}
	
	void HandleHitting(HitData data);
	
	bool CanGenerallyHit(IHittable hitObject);
	bool CanHit(IHittable hitObject);
}
