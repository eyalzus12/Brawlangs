using Godot;
using System;

public class MovementPart : AttackPart
{
	public Vector2 Movement = default;
	
	public override void Init()
	{
		LoadExtraProperty<Vector2>("Movement");
	}
	
	public override void OnStart()
	{
		ch.vec = Movement * new Vector2(ch.direction, 1);
		if(ch.grounded) ch.vec.y = State.VCF;
	}
}
