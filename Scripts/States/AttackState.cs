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
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		
		/*if(ch.grounded)
		{
			if(!ch.crouching && ch.downHeld) ch.Crouch();
			else if(ch.crouching && !ch.downHeld) ch.Uncrouch();
		}*/
	}
	
	protected override void DoMovement()
	{
		if(att?.currentPart is null) return;
		if(ch.InputtingHorizontalDirection()) DoInputMovement();
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
		if(att?.currentPart is null) return;
		
		if(ch.grounded)
		{
			if(att.currentPart.movement.y >= 0)
			{
				ch.vec.y = VCF;
				snapVector = -VCF * ch.fnorm;
			}
			else Unsnap();
		}
		else
		{
			ch.vec.y.Towards(ch.AppropriateFallingSpeed, ch.AppropriateGravity);
		}
	}
	
	protected override void LoopActions()
	{
		SetupCollisionParamaters();
		if(Inputs.IsActionReallyJustPressed("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
		if(Inputs.IsActionReallyJustReleased("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		//SetupCollisionParamaters();
		if(ch.walled && ch.currentClingsUsed < ch.maxClingsAllowed && !touchedWall)
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
	
	public void SetEnd(Attack a)
	{
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
		a.connected = null;
		
		if(a.active)
		{
			a.active = false;
			a.OnEnd();
			if(a.currentPart != null) a.currentPart.Stop();
			ch.ResetCurrentAttack(a);
			a.currentPart = null;
		}
		
		int endlag = a.GetEndlag();
		if(endlag > 0)
		{
			var s = ch.ChangeState<EndlagState>();
			s.endlag = endlag;
			s.att = a;
		}
		else
		{
			ch.TurnConditional();
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
			
			if(ch.grounded)
			{
				if(ch.downHeld)
				{
					ch.Crouch();
					ch.ChangeState(ch.IsIdle()?"Crouch":"Crawl");
				}
				else
				{
					ch.Uncrouch();
					ch.ChangeState(ch.IsIdle()?"Idle":"Walk");
				}
			}
			else if(ch.walled && touchedWall)
			{
				ch.ApplySettings("Wall");
				ch.ChangeState("Wall");
			}
			else ch.ChangeState("Air");
		}
		
		att = null;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
	
	public override void OnChange(State newState)
	{
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
