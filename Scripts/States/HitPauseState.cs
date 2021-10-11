using Godot;
using System;

public class HitPauseState : State
{
	public int hitPauseLength = 0;
	public int stunLength = 0;
	public Vector2 force = Vector2.Zero;
	
	public HitPauseState() : base() {}
	public HitPauseState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		ch.Uncrouch();
		stunLength = 0;
		hitPauseLength = 0;
		ch.ResetVelocity();
	}
	
	public override void SetInputs()
	{
		SetHorizontalAlternatingInputs();
		SetUpHoldingInput();
		SetDownHoldingInput();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= hitPauseLength)
		{
			ch.ChangeState("Stun");
			var s = ch.currentState as StunState;
			s.Force = force;
			s.stunLength = stunLength;
			force = Vector2.Zero;
		}
		else return false;
		
		return true;
	}
}
