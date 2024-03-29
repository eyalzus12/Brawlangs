using Godot;
using System;
using System.Collections.Generic;

public interface IHittable
{
	string Name{get; set;}
	
	Vector2 Position{get; set;}
	int Direction{get; set;}
	
	int TeamNumber{get; set;}
	
	bool GettingHit{get; set;}
	IHitter LastHitter{get; set;}
	
	bool FriendlyFire{get; set;}
	
	List<Hurtbox> Hurtboxes{get; set;}
	
	InvincibilityManager IFrames{get; set;}
	bool Invincible{get;}
	
	StateTagsManager Tags{get; set;}
	void UpdateTags();
	
	void HandleGettingHit(HitData data);
	
	bool CanGenerallyBeHitBy(IHitter hitter);
	bool CanGenerallyBeHitBy(IAttacker attacker);
	bool CanBeHitBy(IHitter hitter);
	bool CanBeHitBy(IAttacker attacker);
}
