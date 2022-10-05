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
		if(ch.Grounded) ch.States.Change("Land");
		else if(frameCount >= ch.WallJumpSquat)
		{
			ch.Turn();
			ch.vec.x = ch.Direction * ch.HorizontalWallJump;
			ch.vec.y = -ch.VerticalWallJump;
			ch.Fastfalling = false;
			
			ch.States.Change("Air");
		}
		else return false;
		
		ch.ApplySettings("Default");
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.WallClinging = false;
	}
}
