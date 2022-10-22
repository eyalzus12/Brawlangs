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
	
	StateTagsManager Tags{get; set;}
	void UpdateTags();
	
	void HandleHitting(HitData data);
	
	bool CanGenerallyHit(IHittable hitObject);
	bool CanHit(IHittable hitObject);
	
	AnimationSprite CharacterSprite{get; set;}
	Color SpriteModulate{get; set;}
	
	Vector2 Momentum{get; set;}
	Vector2 BurstMomentum{get; set;}
	Vector2 Velocity{get;}
}
