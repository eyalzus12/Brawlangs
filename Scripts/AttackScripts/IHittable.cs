using Godot;
using System;

public interface IHittable
{
	int TeamNumber{get; set;}
	void HandleGettingHit(HitData data);
	
	float DamageTakenMult{get;set;}
	float KnockbackTakenMult{get;set;}
	int StunTakenMult{get;set;}
}
