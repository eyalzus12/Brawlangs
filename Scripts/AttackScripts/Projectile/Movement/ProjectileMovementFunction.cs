using Godot;
using System;
using System.Collections.Generic;

public class ProjectileMovementFunction : Node2D
{
	public Dictionary<string, ParamRequest> LoadExtraProperties = new Dictionary<string, ParamRequest>();
	
	public virtual Vector2 GetNext(Projectile proj) => proj.Position;
	
	public virtual void LoadProperties() {}
	
	public void LoadExtraProperty<T>(string s, T @default = default(T))
	{
		var toAdd = new ParamRequest(typeof(T), s, @default);
		LoadExtraProperties.Add(s, toAdd);
	}
}
