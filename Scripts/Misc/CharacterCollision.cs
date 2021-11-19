using Godot;
using System;

public class CharacterCollision : CollisionShape2D
{
	public override void _Ready()
	{
		ZIndex = 3;
	}
	
	public override void _PhysicsProcess(float delta)
	{
		Update();
	}
	
	public override void _Draw()
	{
		if(!this.GetRootNode<UpdateScript>("UpdateScript").debugCollision) return;
		DrawSetTransform(Vector2.Zero, Rotation, Vector2.One);
		var rect = BlastZone.CalcRect(Vector2.Zero, (Shape as RectangleShape2D).Extents);
		DrawRect(rect, GetDrawColor(), true);
	}
	
	public virtual Color GetDrawColor() =>  new Color(1,1,0,1);
}
