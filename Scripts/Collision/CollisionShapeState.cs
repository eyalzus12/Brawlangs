using Godot;
using System;

public record CollisionShapeState(string Name, Vector2 Extents, Vector2 Position);

/*public readonly struct CollisionShapeState
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
}*/
