using Godot;
using System;

public class SpotGroundedDodgeState : GenericInvincibleState
{
	public SpotGroundedDodgeState() : base() {}
	public SpotGroundedDodgeState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override int Startup => ch.SpotGroundedDodgeStartup;
	public override int IFrames => ch.SpotGroundedDodgeLength;
	public override int Endlag => ch.SpotGroundedDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "SpotGroundedDodge";
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vuc.x *= (1f-ch.AppropriateFriction);
		ch.vuc.y *= (1f-ch.AirFriction);
	}
	
	protected override void DecideNextState()
	{
		var turn = ch.TurnConditional();
		if(turn) ch.States.Change("WalkTurn");
		else if(ch.Idle) ch.States.Change("Idle");
		else if(ch.InputtingHorizontalDirection) ch.States.Change("Walk");
		else ch.States.Change("WalkStop");
	}
}
