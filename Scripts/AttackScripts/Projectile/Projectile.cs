using Godot;
using System;
using System.Collections.Generic;

public class Projectile : Node2D
{
	[Signal]
	public delegate void ProjectileDied(Projectile who);
	
	public Vector2 spawningPosition = default;
	public int frameCount = 0;
	public int maxLifetime = 600;
	public ProjectileMovementFunction Movement;
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	public bool active = false;
	public bool Active
	{
		get => active;
		set
		{
			active = value;
			Visible = value;
			if(value) OnSpawn();
		}
	}
	public Node2D owner;
	
	public Projectile() {}
	public Projectile(Node2D owner)
	{
		this.owner = owner;
	}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
	
	public override void _Ready()
	{
		frameCount = 0;
		Position = spawningPosition;
		Init();
		Active = false;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if(!Active) return;
		++frameCount;
		Loop();
		Position = Movement.GetNext(Position);
		
		if(frameCount >= maxLifetime)
			Destruct();
	}
	
	public virtual void Init() {}
	public virtual void OnSpawn() {}
	public virtual void Loop() {}
	public virtual void OnRemove() {}
	
	public void Destruct()
	{
		OnRemove();
		EmitSignal(nameof(ProjectileDied), this);
	}
}
