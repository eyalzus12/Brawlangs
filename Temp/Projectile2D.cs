using Godot;
using System;

public class Projectile2D : Area2D
{
	public int frameCount = 0;
	public int maxLifetime = 600;
	public Vector2 move = default;
	public bool active = false;
	public bool Active
	{
		get => active;
		set
		{
			Monitoring = value;
			Monitorable = value;
			active = value;
			Visible = value;
			
			if(value)
				OnSpawn();
		}
	}
	public Character owner;
	
	public Projectile2D() {}
	public Projectile2D(Character owner)
	{
		this.owner = owner;
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		move = default;
		Init();
		Active = false;
		Connect("body_entered", this, nameof(BodyEntered));
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		++frameCount;
		Loop();
		Position += move;
		
		if(frameCount >= maxLifetime)
			Destruct();
	}
	
	//FIXME: should character collide use AreaEntered? to detect the hurtbox?
	public void BodyEntered(Node body)
	{
		if(body is Character ch) OnCharacterCollide(ch);
		OnCollide(body);
	}
	
	public virtual void Init() {}
	public virtual void OnSpawn() {}
	public virtual void Loop() {}
	public virtual void OnRemove() {}
	public virtual void OnCollide(Node n) {}
	public virtual void OnCharacterCollide(Character ch) {}
	
	public void Destruct()
	{
		OnRemove();
		QueueFree();
	}
}
