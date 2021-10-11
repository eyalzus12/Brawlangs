using Godot;
using System;

public class JumpState : GroundedState
{
	bool jumpActive = false;
	
	public JumpState() : base() {}
	public JumpState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		MarkForDeletion("player_jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		jumpActive = false;
		SetupCollisionParamaters();
		AdjustVelocity();
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.jumpSquat)
		{
			jumpActive = true;
			ch.vec.x *= (1f-Math.Abs(ch.fnorm.x));
			ch.vec.y = -ch.jumpHeight;
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
		if(jumpActive) return base.CalcStateChange();
		else return false;
	}
}
