using Godot;
using System;
using System.Collections.Generic;

public class BlastZone : Node2D
{
	public readonly static Vector2 DEFAULT_SIZE =  new Vector2(2300, 1100);
	
	[Export]
	public Rect2 Bounds;// = CalcRect(new Vector2(512, 300), new Vector2(2000, 1200));
	
	public List<Character> characters = new List<Character>();
	
	public BlastZone()
	{
		Bounds = CalcRect(Vector2.Zero, DEFAULT_SIZE);
	}
	
	public BlastZone(Vector2 Center, Vector2 Limits)
	{
		Bounds = CalcRect(Center, Limits);
	}
	
	public BlastZone(Rect2 rect)
	{
		Bounds = rect;
	}
	
	public override void _Ready()
	{
		Reset();
	} 
	
	public void Reset()
	{
		foreach(var n in GetParent().GetChildren()) if(n is Character c)
		{
			characters.Add(c);
			c.Connect("Dead", this, nameof(CharacterGone));
		}
	}
	
	public override void _PhysicsProcess(float delta)
	{
		var crs = new List<Character>(characters);
		
		foreach(var c in crs)
		{
			if(
				c.Position.x <= Bounds.Position.x ||//Left
				c.Position.x >= Bounds.End.x  ||//Right
				(c.Position.y <= Bounds.Position.y && c.currentState is StunState) || //Top
				c.Position.y >= Bounds.End.y //Bottom
				)
			{
				c.Die();
			}
		}
	}
	
	public static Rect2 CalcRect(Vector2 Center, Vector2 Lengths)
	{
		var TopLeft = Center-Lengths;
		var BottomRight = Center+Lengths;
		var Size = BottomRight-TopLeft;
		return new Rect2(TopLeft, Size);
	}
	
	public void CharacterGone(Node2D who)
	{
		if(who is Character c) characters.Remove(c);
	}
}
