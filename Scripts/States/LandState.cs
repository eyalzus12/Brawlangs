using Godot;
using System;

public class LandState : GroundedState
{
	public LandState() : base() {}
	public LandState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		if(ch.onSemiSolid && Inputs.IsActionPressed("player_down"))
		{
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
			ch.vic.y = VCF;
			return;
		}
		
		ch.vac = Vector2.Zero;
		ch.jumpCounter = 0;
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
			else if(Inputs.IsActionPressed("player_down")) 
				ch.ChangeState("Duck");
			else if(ch.IsIdle())
				ch.ChangeState("Idle");
			else if(turn)
				ch.ChangeState("WalkTurn");
			else if(ch.InputingDirection())
			{
				if(ch.walled)
					ch.ChangeState("WalkWall");
				else
					ch.ChangeState("Walk");
			}
			else
				ch.ChangeState("WalkStop");
			
			return true;
		}
		else return false;
	}
}
