using Godot;
using System;

public class CrouchJumpState : GroundedState
{
	bool jumpActive = false;
	
	public CrouchJumpState() : base() {}
	public CrouchJumpState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		MarkForDeletion("player_jump", true);
		ch.vac = Vector2.Zero;
		jump = false;
		justInit = true;
		jumpActive = false;
		SetupCollisionParamaters();
		AdjustVelocity();
	}
	
	protected override void LoopActions()
	{
		AdjustVelocity();
		
		if(frameCount >= ch.jumpSquat /*+ ch.getupLength*/)
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
		ch.TurnConditional();
		
		if(jumpActive) return;
		
		if(ch.InputingDirection())
			ch.vec.x = ch.direction * ch.crawlSpeed;
		else ch.vec.x = 0;
	}
	
	protected override bool CalcStateChange()
	{
		if(jumpActive) return base.CalcStateChange();
		else return false;
	}
}
