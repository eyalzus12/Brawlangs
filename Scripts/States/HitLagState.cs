using Godot;
using System;

public class HitLagState : State
{
	public HitLagState() : base() {}
	public HitLagState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	public override bool ShouldDrop => ch.DownInput && ch.HoldingRun;
	
	public override void Init()
	{
		ch.ResetVelocity();
		//pause animation
		ch.Fastfalling = false;
		ch.CurrentAttack?.CurrentPart?.Pause();
	}
	
	protected override void DoGravity()
	{
		if(ch.Grounded) ch.vec.y = VCF;
		else if(ch.Fastfalling) ch.vec.y.Towards(ch.FastFallSpeed, ch.FastFallGravity);
	}
	
	public override void SetInputs()
	{
		base.SetInputs();
		SetFastFallInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.HitLagFrames)
		{
			ch.States.Change("Attack");
			ch.CurrentAttack?.CurrentPart?.Resume();
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		if((newState is StunState || newState is HitPauseState) && ch.CurrentAttack != null) ch.CurrentAttack.Active = false;
		ch.HitLagFrames = 0;
	}
}
