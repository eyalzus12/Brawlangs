using Godot;
using System;

public class WallJumpState : WallState
{
	protected bool jumpActive = false;
	
	public WallJumpState() : base() {}
	public WallJumpState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		jump = false;
		jumpActive = false;
		SetupCollisionParamaters();
		AdjustVelocity();
		MarkForDeletion("Jump", true);
		ch.PlayAnimation("WallJumpReady");
		ch.QueueAnimation("WallJump");
	}
	
	protected override void DoMovement() {}
	protected override void DoJump() {}
	
	protected override void RepeatActions()
	{
		if(frameCount < ch.wallJumpSquat) return;
		
		jumpActive = true;
		ch.Turn();
		ch.vec.x = ch.Direction * ch.horizontalWallJump;
		ch.vec.y = -ch.verticalWallJump;
		ch.fastfalling = false;
	}
	
	protected override bool CalcStateChange()
	{
		if(ch.grounded) ch.States.Change("Land");
		else if(jumpActive) ch.States.Change("Air");
		else return false;
		
		ch.ApplySettings("Default");
		return true;
	}
}
