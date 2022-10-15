using Godot;
using System;

public class WallState : State
{
	protected bool jump = false;
	
	public WallState() : base() {}
	public WallState(Character link) : base(link) {}
	
	public override void Init()
	{
		ch.WallClinging = true;
		ch.QueueAnimation("Wall", ch.AnimationLooping, true);
		jump = false;
	}
	
	protected override void DoMovement()
	{
		if(ch.InputtingTurn)
		{
			ch.vec.x = -ch.Direction * ch.AirAcceleration;
			ch.Walled = false;
		}
	}
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(ch.AppropriateFallingSpeed * (2f-ch.WFric), ch.AppropriateGravity * (2f-ch.WFric));
		ch.vuc.y.Towards(0, ch.AppropriateGravity * (2f-ch.WFric));
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
		if(ch.Grounded)
			ch.States.Change("Land");
		else if(ch.TurnConditional() || !ch.Walled)
			ch.States.Change("Air");
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
		if(ch.Ceilinged)
		{
			if(ch.CNorm == Vector2.Down)
				ch.vec *= ch.CeilingBounce;
			else if(ch.vec.y < 0) 
				ch.vec.x = 0;
		}
		
		AdjustVelocity();
	}
	
	protected override void AdjustVelocity()
	{
		if(ch.InputtingTurn) return;
		
		if(ch.MovementDirection * ch.WVel.x > 0)
			ch.vec.x = ch.WVel.x;
		else ch.vec.x = 0;
		
		ch.vec.x += ch.MovementDirection * HCF;
		ch.vac.y = ch.WVel.y;
	}
	
	public override void OnChange(State newState)
	{
		if(!(newState is WallState))
		{
			ch.CharacterSprite.Stop();
			ch.WallClinging = false;
		}
	}
}
