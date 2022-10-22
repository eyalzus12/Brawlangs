using Godot;
using System;
using System.Collections.Generic;

public interface IHitter
{
	string Name{get; set;}
	
	Vector2 Position{get; set;}
	int Direction{get; set;}
	int TeamNumber{get; set;}
	
	float DamageDoneMult{get; set;}
	float KnockbackDoneMult{get; set;}
	float StunDoneMult{get; set;}
	
	List<Hitbox> Hitboxes{get; set;}
	HashSet<IHittable> HitIgnoreList{get; set;}
	Dictionary<Hurtbox, Hitbox> HitList{get; set;}
	IAttacker OwnerObject{get; set;}
	
	bool HasHit{get; set;}
	
	bool FriendlyFire{get; set;}
	
	void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox);
	void HandleHits();
	void HitEvent(Hitbox hitbox, Hurtbox hurtbox);
	
	bool CanGenerallyHit(IHittable hitObject);
	bool CanHit(IHittable hitObject);
}
