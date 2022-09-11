using Godot;
using System;

public class AirJumpState : AirState
{
	bool jumpActive = false;
	
	public AirJumpState() : base() {}
	public AirJumpState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.TurnConditional();
		MarkForDeletion("Jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		jumpActive = false;
		
		ch.PlayAnimation("AirJumpReady", true);
		ch.QueueAnimation("AirJump", false, false);
	}
	
	protected override void LoopActions()
	{
		if(frameCount >= ch.airJumpSquat)
		{
			jumpActive = true;
			ch.vec.y = -ch.doubleJumpHeight;
			ch.Resources.Give("AirJumps", -1);
			ch.fastfalling = false;
			Unsnap();
		}
	}
	
	protected override void DoJump() {}
	
	protected override void DoMovement()
	{
		if(!jumpActive) base.DoMovement();
	}
	
	protected override bool CalcStateChange()
	{
		if(jumpActive)
		{
			if(ch.walled && ch.Resources.Has("Clings")) ch.States.Change("WallLand");
			else ch.States.Change("Air");
		}
		else return false;
		
		return true;
	}
}
