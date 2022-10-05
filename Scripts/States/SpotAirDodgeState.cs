using Godot;
using System;

public class SpotAirDodgeState : GenericInvincibleState
{
	public SpotAirDodgeState() : base() {}
	public SpotAirDodgeState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override int Startup => ch.SpotAirDodgeStartup;
	public override int IFrames => ch.SpotAirDodgeLength;
	public override int Endlag => ch.SpotAirDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "SpotAirDodge";
	
	protected override void OnStart()
	{
		ch.ResetVelocity();
	}
	
	protected override void DecideNextState()
	{
		ch.TurnConditional();
		ch.States.Change("Air");
	}
}
