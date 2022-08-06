using Godot;
using System;
using System.Collections.Generic;

public interface IHitter
{
	Vector2 Position{get; set;}
	int Direction{get; set;}
	
	int TeamNumber{get; set;}
	List<Hitbox> Hitboxes{get;set;}
	HashSet<IHittable> HitIgnoreList{get;set;}
	Dictionary<Hurtbox, Hitbox> HitList{get;set;}
	IAttacker OwnerObject{get;set;}
	
	bool HasHit{get; set;}
	
	void ConnectSignals();
	void HandleInitialHit(Hitbox hitbox, Hurtbox hurtbox);
	void HandleHits();
	void HitEvent(Hitbox hitbox, Hurtbox hurtbox);
	bool CanHit(IHittable h);
}
