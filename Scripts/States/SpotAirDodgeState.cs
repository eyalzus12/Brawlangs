using Godot;
using System;

public class SpotAirDodgeState : GenericInvincibleState
{
	public SpotAirDodgeState() : base() {}
	public SpotAirDodgeState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override int Startup() => ch.spotAirDodgeStartup;
	public override int InvincibilityLength() => ch.spotAirDodgeLength;
	public override int Endlag() => ch.spotAirDodgeEndlag;
	public override int Cooldown() => ch.spotAirDodgeCooldown;
	public override string ActionName() => "Dodge";
	public override string Animation() => "SpotAirDodge";
	
	protected override void OnIFramesStart()
	{
		ch.vec = Vector2.Zero;
	}
	
	protected override void DecideNextState()
	{
		bool turn = ch.TurnConditional();
		ch.ChangeState("Air");
	}
}
