using Godot;
using System;

public class JoshNSpecialProjectile : HitProjectile
{
	public CircleShape2D cshape;
	
	public JoshNSpecialProjectile(Character owner) : base(owner) {}
	
	public override void Init()
	{
		setKnockback = new Vector2(100, 0);
		varKnockback = new Vector2(500, 0);
		stun = 5;
		hitpause = 3;
		damage = 10f;
		SelfModulate = (new Color(1,0.1f,0.1f,1));
		GlobalPosition = owner.GlobalPosition;
		move = new Vector2(owner.direction * 5, 0);
		var cs = new CollisionShape2D();
		AddChild(cs);
		var shape = new CircleShape2D();
		shape.Radius = 3f;
		cshape = shape;
		cs.Shape = cshape;
	}
	
	public override void _Draw()
	{
		if(!Active) return;
		DrawCircle(Vector2.Zero, cshape.Radius, new Color(1,1,1));
	}
}
