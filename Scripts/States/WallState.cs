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
		ch.PlayAnimation("WallIdle");
	}
	
	protected override void DoMovement()
	{
		if(ch.TurnConditional())
			ch.vec.x = ch.direction * ch.airAcceleration;
	}
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(ch.AppropriateFallingSpeed * (2-ch.wfric), ch.AppropriateGravity * (2-ch.wfric));
	}
	
	protected override void DoJump()
	{
		jump = true;
//		ch.Turn();
//		ch.vec.x = ch.direction * ch.horizontalWallJump;
//		ch.vec.y = -ch.verticalWallJump;
//		ch.fastfalling = false;
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetFastFallInput();
		SetDownHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(ch.grounded)
			ch.ChangeState("Land");
		else if(!ch.walled)
			ch.ChangeState("Air");
		else 
		{
			if(jump) ch.ChangeState("WallJump");
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
		if(ch.direction * ch.wvel.x > 0)
			ch.vec.x = ch.wvel.x;
		else ch.vec.x = 0;
		
		ch.vec.x += ch.direction * HCF;
		ch.vac.y = ch.wvel.y;
	}
}
