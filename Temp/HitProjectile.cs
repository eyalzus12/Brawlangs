using Godot;
using System;

public class HitProjectile : Projectile2D
{
	public HitProjectile() : base() {}
	public HitProjectile(Character owner) : base(owner) {}
	
	public List<Hitbox> hitboxes = new List<Hitbox>();
	
	public override void OnCharacterCollide(Character ch)
	{
		if(!owner.CanHit(ch)) return;
		var data = new HitData(Math.Sign(move.x)*setKnockback, Math.Sign(move.x)*varKnockback, damage, stun, hitpause, this, ch.hurtbox);
		ch.ApplyKnockback(data);
		GD.Print($"{ch} was hit by a projectile {this.ToString()}");
		Destruct();
	}
}
