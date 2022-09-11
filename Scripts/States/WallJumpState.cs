using Godot;
using System;

public class WallJumpState : WallState
{
	public WallJumpState() : base() {}
	public WallJumpState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		jump = false;
		SetupCollisionParamaters();
		AdjustVelocity();
		MarkForDeletion("Jump", true);
		ch.PlayAnimation("WallJumpReady", true);
		ch.QueueAnimation("WallJump", false, false);
	}
	
	protected override void DoMovement() {}
	protected override void DoJump() {}
	
	protected override bool CalcStateChange()
	{
		if(ch.grounded) ch.States.Change("Land");
		else if(frameCount >= ch.wallJumpSquat)
		{
			ch.Turn();
			ch.vec.x = ch.Direction * ch.horizontalWallJump;
			ch.vec.y = -ch.verticalWallJump;
			ch.fastfalling = false;
			
			ch.States.Change("Air");
		}
		else return false;
		
		ch.ApplySettings("Default");
		return true;
	}
}
