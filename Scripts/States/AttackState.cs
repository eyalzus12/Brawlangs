using Godot;
using System;

public class AttackState : State
{
	public bool touched = false;
	
	public AttackState() : base() {}
	public AttackState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public Attack att;
	
	public override void Init()
	{
		Unsnap();
		touched = false;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		
		/*if(ch.grounded)
		{
			if(!ch.crouching && ch.downHeld) ch.Crouch();
			else if(ch.crouching && !ch.downHeld) ch.Uncrouch();
		}*/
	}
	
	protected override void DoMovement()
	{
		if(att is null) return;
		
		var friction = att?.attackFriction ?? 0f;
		ch.vec.x *= (1f-friction*(ch.grounded?ch.ffric:1f));
	}
	
	protected override void DoGravity()
	{
		if(att.currentPart != null && Math.Abs(att.currentPart.movement.y) > 1f) 
		{
			Unsnap();
			return;
		}
		
		if(ch.grounded)
		{
			if(att.currentPart.movement.y >= 0)
			{
				ch.vec.y = VCF;
				snapVector = -VCF * ch.fnorm;
			}
			else Unsnap();
		}
		else ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
	}
	
	protected override void LoopActions()
	{
		SetupCollisionParamaters();
		if(Inputs.IsActionReallyJustPressed("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
		if(Inputs.IsActionReallyJustReleased("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		//SetupCollisionParamaters();
		if(ch.walled && ch.wallJumpCounter < ch.wallJumpNum && !touched)
		{
			ch.jumpCounter = 0;
			ch.wallJumpCounter++;
			touched = true;
		}
		
		if(ch.grounded)
		{
			ch.jumpCounter = 0;
			ch.wallJumpCounter = 0;
			ch.EmitSignal("JumpsRestored");
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
				ch.jumpCounter = 0;
				ch.wallJumpCounter = 0;
				ch.EmitSignal("JumpsRestored");
				
				if(ch.crouching) ch.ChangeState(ch.downHeld?"Crawl":"Getup");
				else ch.ChangeState(ch.downHeld?"Duck":"Walk");
			}
			else if(ch.walled && ch.wallJumpCounter < ch.wallJumpNum)
			{
				if(ch.GetState<AttackState>().touched) ch.wallJumpCounter--;
				ch.ChangeState("WallLand");
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
