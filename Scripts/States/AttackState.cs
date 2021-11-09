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
		if(Inputs.IsActionPressed("player_down") && !ch.grounded)
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
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
		if(att.currentPart != null && att.currentPart.movement != Vector2.Zero) return;
		
		if(!ch.grounded) ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
		else ch.vec.y = VCF;
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
	}
	
	protected override void RepeatActions()
	{
		if(Inputs.IsActionJustPressed("player_down") && !ch.grounded)
			ch.SetCollisionMaskBit(DROP_THRU_BIT, false);
		if(Inputs.IsActionJustReleased("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		
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
	
	public void SetEnd(Attack a)
	{
		ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
		a.connected = null;
		ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
		int endlag = a.currentPart.GetEndlag();
		var s = ch.ChangeState("Endlag") as EndlagState;
		s.endlag = endlag;
		s.att = a;
		att = null;
		if(!ch.downHeld) ch.Uncrouch();
	}
}
