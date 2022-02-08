using Godot;
using System;

public readonly struct CollisionShapeState
{
	public readonly string Name;
	public readonly Vector2 Extents;
	public readonly Vector2 Position;
	
	public CollisionShapeState(string name, Vector2 extents, Vector2 pos)
	{
		Name = name;
		Extents = extents;
		Position = pos;
	}
}
