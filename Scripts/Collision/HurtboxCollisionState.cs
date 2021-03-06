using Godot;
using System;

public readonly struct HurtboxCollisionState
{
	public readonly string Name;
	public readonly float Radius;
	public readonly float Height;
	public readonly Vector2 Position;
	public readonly float Rotation;
	
	public HurtboxCollisionState(string name, float radius, float height, Vector2 pos, float rot)
	{
		Name = name;
		Radius = radius;
		Height = height;
		Position = pos;
		Rotation = rot;
	}
}
