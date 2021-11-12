using Godot;
using System;

public class EndlagState : State
{
	public EndlagState() : base() {}
	public EndlagState(Character link) : base(link) {}
	
	public int endlag = 0;
	public Attack att = null;
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		endlag = 0;
		att = null;
		SetupCollisionParamaters();
		ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
		//ch.PlayAnimation(att.endlagAnimation);
		//figure out a way to set the animation after the attack exists
	}
	
	protected override void DoMovement()
	{
		if(att is null) return;
		var friction = att?.attackFriction ?? 0f;
		ch.vec.x *= 1f-friction*(ch.grounded?ch.ffric:1f);
	}
	
	protected override void RepeatActions()
	{
		if(Inputs.IsActionReallyJustReleased("player_down"))
			ch.SetCollisionMaskBit(DROP_THRU_BIT, true);
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
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= endlag)
		{
			ch.SetCollisionMaskBit(DROP_THRU_BIT, !ch.downHeld);
			if(!ch.downHeld&&ch.grounded) ch.Uncrouch();
			
			if(ch.walled && ch.wallJumpCounter < ch.wallJumpNum)
			{
				if((ch.GetState("Attack") as AttackState).touched) ch.wallJumpCounter--;
				ch.ChangeState("WallLand");
			}
			else ch.ChangeState(ch.grounded?ch.downHeld?"Crawl":"Walk":"Air");
		}
		else return false;
		
		return true;
	}
}
