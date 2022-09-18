using Godot;
using System;

public partial class WallLandState : WallState
{
	public WallLandState() : base() {}
	public WallLandState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		if(ch.Velocity.x != 0) ch.Direction = Math.Sign(ch.Velocity.x);
		Unsnap();
		ch.vac = Vector2.Zero;
		SetupCollisionParamaters();
		
		ch.vec.y *= (1-ch.wallFriction*ch.wfric);
		
		ch.vuc.x = 0;
		ch.vuc.y *= (1-ch.wallFriction*ch.wfric);
		
		//ch.onSemiSolid = false;
		ch.PlayAnimation("WallLand", true);
		ch.QueueAnimation("Wall", false, false);
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
