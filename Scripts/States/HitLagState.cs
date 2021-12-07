using Godot;
using System;

public class HitLagState : State
{
	public int hitLagLength = 0;
	
	public HitLagState() : base() {}
	public HitLagState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.ResetVelocity();
		//pause animation
		ch.fastfalling = false;
		ch.currentAttack.currentPart.Pause();
	}
	
	protected override void DoGravity()
	{
		if(ch.grounded) ch.vec.y = VCF;
		else if(ch.fastfalling) ch.vec.y.Lerp(ch.fastFallSpeed, ch.fastFallGravity);
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
		SetFastFallInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= hitLagLength)
		{
			ch.ChangeState("Attack");
			ch.currentAttack.currentPart.Resume();
			hitLagLength = 0;
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		if(newState is StunState || newState is HitPauseState)
		{
			var att = ch.currentAttack;
			if(att is null) return;
			att.Disconnect("AttackEnds", ch.GetState("Attack"), "SetEnd");
			att.connected = null;
			att.active = false;
			att.OnEnd();
			if(att.currentPart != null) att.currentPart.Stop();
			ch.ResetCurrentAttack(att);
			att.currentPart = null;
		}
	}
}
