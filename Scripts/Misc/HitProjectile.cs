using Godot;
using System;

public class HitProjectile : Projectile2D
{
	public HitProjectile() : base() {}
	public HitProjectile(Character owner) : base(owner) {}
	
	public Vector2 setKnockback = Vector2.Zero;
	public Vector2 varKnockback = Vector2.Zero;
	public int stun = 0;
	public int hitpause = 0;
	public float damage = 0f;
	
	public float radius = 0f;
	
	public override void OnCharacterCollide(Character ch)
	{
		if(!owner.CanHit(ch)) return;
		ch.ApplyKnockback(Math.Sign(move.x)*setKnockback,
		Math.Sign(move.x)*varKnockback, damage, stun, hitpause);
		GD.Print($"{ch} was hit by a projectile {this.ToString()}");
		Destruct();
	}
	
	public override void _Draw()
	{
		DrawCircle(Vector2.Zero, radius, new Color(1,1,1));
	}
}
