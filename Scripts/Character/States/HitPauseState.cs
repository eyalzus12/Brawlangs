using Godot;
using System;

public class HitPauseState : State
{
	public HitPauseState() : base() {}
	public HitPauseState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override void Init()
	{
		ch.Uncrouch();
		ch.ResetVelocity();
		ch.CharacterSprite.Pause();
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.HitPauseFrames)
		{
			ch.States.Change<StunState>();
			ch.HitPauseFrames = 0;
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.CharacterSprite.Resume();
	}
}
