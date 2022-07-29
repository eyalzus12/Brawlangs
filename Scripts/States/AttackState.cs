using Godot;
using System;

public class AttackState : State
{
	public bool touchedWall = false;
	public bool touchedGround = false;
	
	public AttackState() : base() {}
	public AttackState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public Attack att;
	
	public override void Init()
	{
		Unsnap();
		touchedWall = false;
		touchedGround = false;
	}
	
	public override void SetInputs()
	{
		base.SetInputs();
		SetFastFallInput();
	}
	
	protected override void DoMovement()
	{
		if(att?.currentPart is null) return;
		if(ch.InputtingHorizontalDirection) DoInputMovement();
		DoFriction();
	}
	
	protected virtual void DoInputMovement()
	{
		var idir = ch.GetInputDirection();
		if(idir == ch.direction)
			ch.vec.x.Towards(ch.direction * att.currentPart.driftForwardSpeed, att.currentPart.driftForwardAcceleration);
		else
			ch.vec.x.Towards(-ch.direction * att.currentPart.driftBackwardsSpeed, att.currentPart.driftBackwardsAcceleration);
	}
	
	protected virtual void DoFriction()
	{
		var friction = att.attackFriction*(ch.grounded?ch.ffric:1f);
		ch.vec.x *= (1f-friction);
	}
	
	protected override void DoGravity()
	{
		//aerial
		if(!ch.grounded) ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
		//grounded and moves up
		else if((att?.currentPart?.movement.y ?? 0) < 0) Unsnap();
		//grounded and moves down
		else
		{
			ch.vec.y = VCF;
			snapVector = -VCF * ch.fnorm;
		}
	}
	
	protected override void LoopActions()
	{
		SetupCollisionParamaters();
		if(ch.walled && ch.HasResource("Clings") && !touchedWall)
		{
			ch.RestoreOptionsOnWallTouch();
			ch.vec.y *= (1-ch.wallFriction*ch.wfric);
			touchedWall = true;
		}
		
		if(ch.grounded && !touchedGround)
		{
			ch.RestoreOptionsOnGroundTouch();
			touchedGround = true;
		}
	}
	
	protected override void RepeatActions()
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
	
	public void SetEnd(Attack a)
	{
		//remove signal
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
		a.connected = null;
		
		//disable attack
		if(a.active)
		{
			a.active = false;
			//call ending function
			a.OnEnd();
			//stop attack part
			if(a.currentPart != null) a.currentPart.Stop();
			//reset character attack
			ch.ResetCurrentAttack(a);
			//revert attack state
			a.currentPart = null;
		}
		
		//apply enflag
		int endlag = a.GetEndlag();
		if(endlag > 0)//has endlag to apply
		{
			//switch state
			var s = ch.ChangeState<EndlagState>();
			//supply state with endlag
			s.endlag = endlag;
			//supply state with attack
			s.att = a;
		}
		else//no endlag. state transition immedietly
		{
			//turnaroujnd
			ch.TurnConditional();
			
			//transition to appropriate state
			if(ch.grounded)
			{
				if(ch.downHeld)
				{
					ch.Crouch();
					ch.ChangeState(ch.IsIdle?"Crouch":"Crawl");
				}
				else
				{
					ch.Uncrouch();
					ch.ChangeState(ch.IsIdle?"Idle":"Walk");
				}
			}
			else if(ch.walled && touchedWall)
			{
				ch.ApplySettings("Wall");
				ch.ChangeState("Wall");
			}
			else ch.ChangeState("Air");
		}
		
		//forget attack
		att = null;
	}
	
	//handle state change
	public override void OnChange(State newState)
	{
		//got hit
		if(newState is StunState || newState is HitPauseState)
		{
			att.Disconnect("AttackEnds", this, nameof(SetEnd));
			att.connected = null;
			att.active = false;
			att.OnEnd();
			if(att.currentPart != null) att.currentPart.Stop();
			ch.ResetCurrentAttack(att);
			att.currentPart = null;
		}
	}
}
