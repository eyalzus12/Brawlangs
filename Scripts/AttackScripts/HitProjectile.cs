using Godot;
using System;

public class HitProjectile : Hitbox
/*
 this will require a projectile interface
*/
{
	public Vector2 move = default;
	public int lifeTime = 0;
	
	public override void Init()
	{
		LoadExtraProperty<Vector2>("Movement", Vector2.Zero);
		LoadExtraProperty<int>("Lifetime", 0);
	}
	
	public override void UpdateHitboxPosition() {}; //prevent flipping from direction all the time
	
	public override void Loop()
	{
		if(frameCount >= lifeTime)
			QueueFree();
	}
	
	public override void OnHit(Area2D area)
	{
		var hitChar = area.GetParent() as Character;
		var ch = owner as Character;
		if(hitChar is null || ch is null || !ch.CanHit(hitChar)) return;//cant handle non character things
		var kmult = ch.knockbackDoneMult*GetKnockbackMultiplier(hitChar)*knockbackMult;
		var dirvec = KnockbackDir(hitChar)*kmult;
		var skb = dirvec*setKnockback + momentumCarry*ch.GetVelocity();
		var vkb = dirvec*varKnockback;
		var dmult = ch.damageDoneMult*GetDamageMultiplier(hitChar)*damageMult;
		var damage = damage*dmult;
		var smult = ch.stunDoneMult*GetStunMultiplier(hitChar)*stunMult*att.stunMult;
		var stun = stun*smult;
		var data = new HitData(skb, vkb, damage, stun, hitpause, this, area);
			
		hitChar.ApplyKnockback(data);
		GD.Print($"{hitChar} was hit by {Name}");
		ch.HandleHitting(this, area, hitChar);
	}
}
