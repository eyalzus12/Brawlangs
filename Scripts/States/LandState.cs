using Godot;
using System;

public class LandState : GroundedState
{
	public LandState() : base() {}
	public LandState(Character link) : base(link) {}
	
	public override void Init()
	{
		if(ch.onSemiSolid && ch.downHeld)
		{
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
			ch.vic.y = VCF;
			SetupCollisionParamaters();
			return;
		}
		
		ch.vac = Vector2.Zero;
		ch.jumpCounter = 0;
		ch.wallJumpCounter = 0;
		ch.fastfalling = false;
			
		AdjustVelocity();
		ch.PlayAnimation("Land");
	}
	
	protected override void DoJump() {}
	
	protected override void DoMovement()
	{
		//if(ch.InputingDirection()) DoInputMovement();
		//else
		//DoFriction();
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		
		if(frameCount >= ch.impactLand)
		{
			bool turn = ch.TurnConditional();
			
			if(Inputs.IsActionJustPressed("player_jump"))
				ch.ChangeState("Jump");
			else if(Inputs.IsActionPressed("player_down") && !ch.onSemiSolid) 
				ch.ChangeState(ch.InputingDirection()?ch.walled?"CrawlWall":"Crawl":"Crouch");
			else if(ch.IsIdle())
				ch.ChangeState("Idle");
			else if(turn)
				ch.ChangeState("WalkTurn");
			else if(ch.InputingDirection())
				ch.ChangeState(ch.walled?"WalkWall":"Walk");
			else
				ch.ChangeState("WalkStop");
			
			return true;
		}
		else return false;
	}
}
