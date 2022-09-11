using Godot;
using System;

public class HitPauseState : State
{
	public int hitPauseLength = 0;
	public int stunLength = 0;
	public Vector2 force = Vector2.Zero;
	
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
		if(frameCount >= hitPauseLength)
		{
			var s = ch.States.Change<StunState>();
			s.Force = force;
			s.stunLength = stunLength;
			force = Vector2.Zero;
			stunLength = 0;
			hitPauseLength = 0;
		}
		else return false;
		
		return true;
	}
	
	public override void OnChange(State newState)
	{
		ch.CharacterSprite.Resume();
	}
}
