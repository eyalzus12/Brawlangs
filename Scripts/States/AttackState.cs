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
		if(!ch.CurrentAttack?.CurrentPart?.FastFallLocked ?? true) SetFastFallInput();
	}
	
	protected override void DoMovement()
	{
		if(ch.CurrentAttack?.CurrentPart is null) return;
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		DoFriction();
	}
	
	protected virtual void DoInputMovement()
	{
		if(ch.InputDirection == ch.Direction)
			ch.vec.x.Towards(ch.Direction * ch.CurrentAttack.CurrentPart.DriftForwardSpeed, ch.CurrentAttack.CurrentPart.DriftForwardAcceleration);
		else
			ch.vec.x.Towards(-ch.Direction * ch.CurrentAttack.CurrentPart.DriftBackwardsSpeed, ch.CurrentAttack.CurrentPart.DriftBackwardsAcceleration);
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
		else if((ch.CurrentAttack?.CurrentPart?.Movement.y ?? 0) < 0) Unsnap();
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
		//failsafe for getting locked into the attack state
		if(ch.CurrentAttack is null) SetEnd(null);
		
		SetupCollisionParamaters();
		if(ch.walled && ch.Resources.Has("Clings") && !touchedWall)
		{
			ch.RestoreOptionsOnWallTouch();
			if(ch.CurrentAttack?.CurrentPart?.SlowOnWalls ?? true) ch.vec.y *= (1-ch.wallFriction*ch.wfric);
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
	}
	
	//handle state change
	public override void OnChange(State newState)
	{
		#if DEBUG_ATTACKS
		GD.Print($"{ch} is changing from Attack State");
		#endif
		
		//got hit
		if((newState is StunState || newState is HitPauseState) && ch.CurrentAttack != null) ch.CurrentAttack.Active = false;
	}
}
