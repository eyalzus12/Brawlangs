using Godot;
using System;

public class WallLandState : WallState
{
	public WallLandState() : base() {}
	public WallLandState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		Unsnap();
		ch.jumpCounter = 0;
		ch.vac = Vector2.Zero;
		SetupCollisionParamaters();
		
		ch.vec.y *= ch.wallFriction * ch.wfric;
			
		//ch.onSemiSolid = false;
		ch.PlayAnimation("WallLand");
		ch.ApplySettings("Wall");
		
		ch.wallJumpCounter++;
	}
	
	protected override bool CalcStateChange()
	{
		if(!ch.walled)
		{
			ch.ChangeState("Air");
			ch.ApplySettings("Normal");
		}
		else if(frameCount >= ch.wallLand) ch.ChangeState("Wall");
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
