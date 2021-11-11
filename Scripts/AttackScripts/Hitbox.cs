using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Hitbox : Area2D
{
	[Signal]
	public delegate void HitboxHit(Hitbox hitbox, Area2D hurtbox);
	
	[Export]
	public Vector2 setKnockback = Vector2.Zero;
	[Export]
	public Vector2 varKnockback = Vector2.Zero;
	[Export]
	public int stun = 0;
	[Export]
	public int hitpause = 0;
	[Export]
	public int hitlag = 0;
	[Export]
	public float damage = 0f;
	[Export]
	public int hitPriority = 0;
	
	public List<Vector2> activeFrames = new List<Vector2>();
	public Character ch;
	
	private bool active = false;
	
	public Vector2 originalPosition = Vector2.Zero;
	public float originalRotation = 0f;
	
	public bool Active
	{
		get => active;
		set
		{
			active = value;
			Visible = value;
			shape?.SetDeferred("disabled", !active);
		}
	}
	
	public CollisionShape2D shape = new CollisionShape2D();
	
	public override void _Ready()
	{
		//shape.SetDeferred("position",
		//	new Vector2(ch.direction*shape.Position.x, shape.Position.y));
		Init();
		Reload();
				
		Active = false;
		Visible = false;
		
		Connect("area_entered", this, nameof(OnHurtboxEnter));
		
		CollisionLayer = 0b1_0000;
		CollisionMask = 0b0_1000;
	}
	
	public virtual void Init() {}
	
	public virtual void Reload()
	{
		try
		{
			shape = GetChildren().FilterType<CollisionShape2D>().Single();
		}
		catch(InvalidOperationException)
		{
			if(GetChildren().Count >= 2)
			//there's two collision shapes for some reason
				GD.Print("Hitbox {this.ToString()} of character {ch.Name} has more than one collision shape");
			//another option is that there's no collision, probably meaning it was added before its collision
			//the builder should manually call this function after adding collision
		}
		
		originalPosition = shape?.Position ?? default(Vector2);
		originalRotation = shape?.Rotation ?? 0;
	}
	
	public void OnHurtboxEnter(Area2D area)
	{
		OnHit(area);
		EmitSignal(nameof(HitboxHit), this, area);
		//GD.Print("hit");
	}
	
	public virtual void OnHit(Area2D area) {}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		shape?.SetDeferred("position", originalPosition*new Vector2(ch.direction, 1));
		shape?.SetDeferred("rotation", originalRotation*ch.direction);
		Update();
		Loop();
	}
	
	public override void _Draw()
	{
		GeometryUtils.DrawCapsuleShape(this,
			shape.Shape as CapsuleShape2D, //shape
			originalPosition*new Vector2(ch.direction, 1), //position
			originalRotation*ch.direction, //rotation
			new Color(1,1,1)); //color
		
		/*
		var oval = shape.Shape as CapsuleShape2D;
		var height = oval.Height;
		var radius = oval.Radius;
		var color = new Color(1,1,1);
		var position = originalPosition*new Vector2(ch.direction, 1);
		var rotation = originalRotation*ch.direction;
		DrawSetTransform(position, rotation, new Vector2(1,1));
		var middleRect = BlastZone.CalcRect(Vector2.Zero, new Vector2(radius, height/2));
		DrawRect(middleRect, color);
		DrawCircle(new Vector2(0, height/2), radius, color);
		DrawCircle(new Vector2(0, -height/2), radius, color);
		*/
	}
	
	public virtual void Loop() {}
}
