using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	Vector2 Position{get; set;}
	int Direction{get; set;}
	
	int TeamNumber{get; set;}
	
	bool GettingHit{get; set;}
	IHitter LastHitter{get; set;}
	
	bool FriendlyFire{get; set;}
	
	List<Hurtbox> Hurtboxes{get; set;}
	
	InvincibilityManager IFrames{get; set;}
	bool Invincible{get;}
	
	void HandleGettingHit(HitData data);
	
	bool CanGenerallyBeHitBy(IHitter hitter);
	bool CanGenerallyBeHitBy(IAttacker attacker);
	bool CanBeHitBy(IHitter hitter);
	bool CanBeHitBy(IAttacker attacker);
}
