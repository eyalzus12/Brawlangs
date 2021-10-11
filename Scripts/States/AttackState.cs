using Godot;
using System;

public class AttackState : State
{
	public AttackState() : base() {}
	public AttackState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public Attack att;
	public bool ended = false;
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-att.attackFriction*(ch.grounded?ch.ffric:1f));
	}
	
	protected override void DoGravity()
	{
		if(!ch.grounded) ch.vec.y.Lerp(ch.fallSpeed, ch.gravity);
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
	
	protected override bool CalcStateChange()
	{
		if(ended)
		{
			int endlag = att.currentPart.GetEndlag();
			var s = ch.ChangeState("Endlag") as EndlagState;
			s.endlag = endlag;
			s.att = att;
			
			if(!ch.downHeld) ch.Uncrouch();
		}
		else if(ch.currentAttack is null)
		{
			GD.Print("You thought i was a normal attack, but it was me, NULL!");
			if(!ch.downHeld) ch.Uncrouch();
			ch.ChangeState(ch.grounded?(ch.downHeld?"Crawl":"Walk"):ch.walled?"Wall":"Air");
		}
		else return false;
		
		return true;
	}
	
	public void SetEnd(Attack a)
	{
		ended = true;
		a.Disconnect("AttackEnds", this, nameof(SetEnd));
	}
	
	public override void OnChange()
	{
		ended = false;
	}
}
