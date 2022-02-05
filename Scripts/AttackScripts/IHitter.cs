using Godot;
using System;
using System.Collections.Generic;

public interface IHitter
{
	List<Hitbox> Hitboxes{get;set;}
	HashSet<IHittable> HitIgnoreList{get;set;}
	Dictionary<Hurtbox, Hitbox> HitList{get;set;}
	bool Hit{get;set;}
	IAttacker OwnerObject{get;set;}
	
	void ConnectSignals();
	void HandleInitialHit(Hitbox hitbox, Area2D hurtbox);
	void HandleHits();
	void HitEvent(Hitbox hitbox, Hurtbox hurtbox);
}
