using Godot;
using System;

public class ArtemisPart : AttackPart
{
	public float ForwardSpeed = 0f;
	public float BackSpeed = 0f;
	public int MoveLength = 0;
	
	public override void LoadProperties()
	{
		LoadExtraProperty<float>("ForwardSpeed", 0f);
		LoadExtraProperty<float>("BackSpeed", 0f);
		LoadExtraProperty<int>("MoveLength", 0);
	}
	
	public override void Loop()
	{
		if(frameCount < startup) {}
		else if(frameCount < startup + MoveLength)
			ch.vuc.x = ForwardSpeed * ch.direction;
		else if(frameCount < startup + MoveLength + Math.Round(MoveLength*ForwardSpeed/BackSpeed))
			ch.vuc.x = -BackSpeed * ch.direction;
		else ch.vuc = Vector2.Zero;
	}
	
	public override void OnEnd()
	{
		ch.vuc = Vector2.Zero;
	}
}
