using Godot;
using System;

public partial class ArtemisPart : AttackPart
{
	public float ForwardSpeed{get; set;}
	public float BackSpeed{get; set;}
	public int MoveLength{get; set;}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<float>("ForwardSpeed", 0f);
		LoadExtraProperty<float>("BackSpeed", 0f);
		LoadExtraProperty<int>("MoveLength", 0);
	}
	
	public override void Loop()
	{
		if(frameCount < Startup) {}
		else if(frameCount < Startup + MoveLength)
			ch.vuc.x = ForwardSpeed * ch.Direction;
		else if(frameCount < Startup + MoveLength + Math.Round(MoveLength*ForwardSpeed/BackSpeed))
			ch.vuc.x = -BackSpeed * ch.Direction;
		else ch.vuc = Vector2.Zero;
	}
	
	public override void OnEnd()
	{
		ch.vuc = Vector2.Zero;
	}
}
