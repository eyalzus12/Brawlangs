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
		hitLagLength = 0;
		ch.ResetVelocity();
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetDownHoldingInput();
		SetUpHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= hitLagLength) ch.ChangeState("Attack");
		else return false;
		
		return true;
	}
}
