using Godot;
using System;

public class SpotAirDodgeState : GenericInvincibleState
{
	public SpotAirDodgeState() : base() {}
	public SpotAirDodgeState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override int Startup => ch.spotAirDodgeStartup;
	public override int IFrames => ch.spotAirDodgeLength;
	public override int Endlag => ch.spotAirDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "SpotAirDodge";
	
	protected override void DoGravity()
	{
		ch.vec.y.Towards(0, ch.AppropriateGravity);
	}
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
	}
	
	protected override void DecideNextState()
	{
		ch.TurnConditional();
		ch.States.Change("Air");
	}
}
