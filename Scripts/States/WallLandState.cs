using Godot;
using System;

public class WallLandState : WallState
{
	public WallLandState() : base() {}
	public WallLandState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		Unsnap();
		ch.vac = Vector2.Zero;
		SetupCollisionParamaters();
		
		ch.vec.y *= (1-ch.wallFriction*ch.wfric);
		ch.vuc.y *= (1-ch.wallFriction*ch.wfric);
		
		//ch.onSemiSolid = false;
		ch.PlayAnimation("WallLand");
		ch.ApplySettings("Wall");
		ch.Tags["Walled"] = StateTag.Starting;
		
		ch.RestoreOptionsOnWallTouch();
	}
	
	protected override bool CalcStateChange()
	{
		if(!ch.walled)
		{
			ch.States.Change("Air");
			ch.ApplySettings("Default");
		}
		else if(frameCount >= ch.wallLand) ch.States.Change("Wall");
		else return false;
		
		return true;
	}
	
	protected override void DoJump() {}
	
	protected override void RepeatActions()
	{
		base.RepeatActions();
		AdjustVelocity();
	}
}
