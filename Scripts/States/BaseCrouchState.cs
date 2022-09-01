using Godot;
using System;

public class BaseCrouchState : GroundedState
{
	public BaseCrouchState(): base() {}
	public BaseCrouchState(Character link): base(link) {}
	
	public override void ForcedInit()
	{
		base.ForcedInit();
		ch.Crouch();
	}
	
	protected override void DoMovement()
	{
		ch.TurnConditional();
		
		if(ch.InputtingHorizontalDirection)
			ch.vec.x.Towards(ch.Direction * ch.crawlSpeed, ch.AppropriateAcceleration);
		else
			ch.vec.x *= (1-ch.AppropriateFriction);
		
		ch.vuc.x *= (1-ch.AppropriateFriction);
	}
	
	protected override bool CalcStateChange()
	{
		if(jump) ch.States.Change("CrouchJump");
		else if(base.CalcStateChange()) return true;
		else if(!ch.downHeld) ch.States.Change("Getup");
		else return false;
		
		return true;
	}
}
