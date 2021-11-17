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
}
