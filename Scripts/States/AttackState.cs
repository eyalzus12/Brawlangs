using Godot;
using System;

public class AttackState : State
{
	public bool touchedWall = false;
	public bool touchedGround = false;
	public override bool ShouldDrop => ch.downHeld && ch.HoldingRun;
	
	public AttackState() : base() {}
	public AttackState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public Attack att;
	
	public override void Init()
	{
		Unsnap();
		ch.Uncrouch();
		touchedWall = false;
		touchedGround = false;
	}
	
	public override void SetInputs()
	{
		base.SetInputs();
		if(!att?.CurrentPart?.FastFallLocked ?? true) SetFastFallInput();
	}
	
	protected override void DoMovement()
	{
		if(att?.CurrentPart is null) return;
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		DoFriction();
	}
	
	protected virtual void DoInputMovement()
	{
		if(ch.InputDirection == ch.Direction)
			ch.vec.x.Towards(ch.Direction * att.CurrentPart.DriftForwardSpeed, att.CurrentPart.DriftForwardAcceleration);
		else
			ch.vec.x.Towards(-ch.Direction * att.CurrentPart.DriftBackwardsSpeed, att.CurrentPart.DriftBackwardsAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vuc.x *= (1f-ch.AppropriateFriction);
	}
	
	protected override void DoGravity()
	{
		//aerial
		if(!ch.grounded)
		{
			ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
			ch.vuc.y.Towards(0, ch.AppropriateGravity);
		}
		//grounded and moves up
		else if((att?.CurrentPart?.Movement.y ?? 0) < 0) Unsnap();
		//grounded and moves down
		else
		{
			ch.vec.y = VCF;
			ch.vuc.y = 0;
			snapVector = -VCF * ch.fnorm;
		}
	}
	
	protected override void LoopActions()
	{
		SetupCollisionParamaters();
		if(ch.walled && ch.Resources.Has("Clings") && !touchedWall)
		{
			ch.RestoreOptionsOnWallTouch();
			if(att?.CurrentPart?.SlowOnWalls ?? true) ch.vec.y *= (1-ch.wallFriction*ch.wfric);
			touchedWall = true;
		}
		
		if(ch.grounded && !touchedGround)
		{
			ch.RestoreOptionsOnGroundTouch();
			touchedGround = true;
		}
	}
	
	public void SetEnd(Attack a)
	{
		//turnaround
		ch.TurnConditional();
		
		//transition to appropriate state
		if(ch.grounded)
		{
			if(ch.downHeld)
			{
				ch.Crouch();
				ch.States.Change(ch.Idle?"Crouch":"Crawl");
			}
			else
			{
				ch.Uncrouch();
				ch.States.Change(ch.Idle?"Idle":"Walk");
			}
		}
		else if(ch.walled && touchedWall)
		{
			ch.ApplySettings("Wall");
			ch.States.Change("Wall");
		}
		else ch.States.Change("Air");
		
		//forget attack
		att = null;
	}
	
	//handle state change
	public override void OnChange(State newState)
	{
		//GD.Print($"{ch} is changing from Attack State");
		//got hit
		if(newState is StunState || newState is HitPauseState) att.Active = false;
	}
}
