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
		MarkForDeletion("jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		jumpActive = false;
		ch.PlayAnimation("AirJumpReady");
		ch.QueueAnimation("AirJump");
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
		if(jumpActive) ch.States.Change("Air");
		return true;
	}
}
