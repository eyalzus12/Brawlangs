using Godot;
using System;

public class WallState : State
{
	protected bool jump = false;
	
	public WallState() : base() {}
	public WallState(Character link) : base(link) {}
	
	public override void Init()
	{
		jump = false;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingTurn)
		{
			ch.vec.x = -ch.Direction * ch.airAcceleration;
			ch.walled = false;
		}
	}
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(ch.AppropriateFallingSpeed * (2-ch.wfric), ch.AppropriateGravity * (2-ch.wfric));
		ch.vuc.y.Towards(0, ch.AppropriateGravity * (2-ch.wfric));
	}
	
	protected override void DoJump()
	{
		jump = true;
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetFastFallInput();
		SetDownHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(ch.grounded) ch.States.Change("Land");
		else if(ch.TurnConditional() || !ch.walled) ch.States.Change("Air");
		else 
		{
			if(jump) ch.States.Change("WallJump");
			else return false;
			return true;
		}
		
		ch.ApplySettings("Default");
		return true;
	}
	
	protected override void RepeatActions()
	{
		if(ch.ceilinged)
		{
			if(ch.cnorm == Vector2.Down)
				ch.vec *= ch.ceilingBounce;
			else if(ch.vec.y < 0) 
				ch.vec.x = 0;
		}
		
		AdjustVelocity();
	}
	
	protected override void AdjustVelocity()
	{
		if(ch.InputtingTurn) return;
		
		if(ch.MovementDirection * ch.wvel.x > 0)
			ch.vec.x = ch.wvel.x;
		else ch.vec.x = 0;
		
		ch.vec.x += ch.MovementDirection * HCF;
		ch.vac.y = ch.wvel.y;
	}
}
