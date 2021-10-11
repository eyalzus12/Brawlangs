using Godot;
using System;

public class AttackState : State
{
	public AttackState() : base() {}
	public AttackState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public Attack att;
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-att.attackFriction*(ch.grounded?ch.ffric:1f));
	}
	
	protected override void DoGravity()
	{
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
		if(ch.walled) ch.jumpCounter = 0;
	}
	
	public void SetEnd(Attack a)
	{
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
		int endlag = a.currentPart.GetEndlag();
		var s = ch.ChangeState("Endlag") as EndlagState;
		s.endlag = endlag;
		s.att = a;
		att = null;
		if(!ch.downHeld) ch.Uncrouch();
	}
}
