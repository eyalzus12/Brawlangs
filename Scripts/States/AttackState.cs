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
		touched = false;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		
		/*if(ch.grounded)
		{
			if(!ch.crouching && ch.downHeld) ch.Crouch();
			else if(ch.crouching && !ch.downHeld) ch.Uncrouch();
		}*/
		
		//ch.PlayAnimation(att.animation);
		//figure out way to do it when the attack actually exists
	}
	
	protected override void DoMovement()
	{
		if(att is null/* || (att.currentPart != null && att.currentPart.movement != Vector2.Zero)*/) return;
		
		var friction = att?.attackFriction ?? 0f;
		ch.vec.x *= (1f-friction*(ch.grounded?ch.ffric:1f));
	}
	
	protected override void DoGravity()
	{
		if(att.currentPart != null && Math.Abs(att.currentPart.movement.y) > 1f) return;
		
		if(!ch.grounded) ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
		else
		{
			ch.vec.y = VCF;
			if(att.currentPart.movement.y > 0) snapVector = -VCF * ch.fnorm;
			else Unsnap();
		}
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
	}
	
	protected override void RepeatActions()
	{
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
		}
	}
	
	protected override void LoopActions()
	{
		SetupCollisionParamaters();
	}
	
	public void SetEnd(Attack a)
	{
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
		a.connected = null;
		int endlag = a.currentPart.GetEndlag();
		if(endlag > 0)
		{
			var s = ch.ChangeState("Endlag") as EndlagState;
			s.endlag = endlag;
			s.att = a;
		}
		else
		{
			if(ch.walled && ch.wallJumpCounter < ch.wallJumpNum)
			{
				if(touched) ch.wallJumpCounter--;
				ch.ChangeState("WallLand");
			}
			else if(ch.grounded)
			{
				if(ch.downHeld)
				{
					if(ch.crouching) ch.ChangeState("Crawl");
					else ch.ChangeState("Duck");
				}
				else
				{
					if(ch.crouching) ch.ChangeState("Getup");
					else ch.ChangeState("Walk");
				}
			}
			else ch.ChangeState("Air");
		}
		
		att = null;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
	}
}
