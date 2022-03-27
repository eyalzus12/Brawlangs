using Godot;
using System;

public interface IAttacker
{
	float DamageDoneMult{get; set;}
	float KnockbackDoneMult{get; set;}
	float StunDoneMult{get; set;}
	int TeamNumber{get; set;}
	
	bool CanHit(IHittable hitObject);
	void HandleHitting(HitData data);
}
