using Godot;
using System;

public partial class SpotGroundedDodgeState : GenericInvincibleState
{
	public SpotGroundedDodgeState() : base() {}
	public SpotGroundedDodgeState(Character link) : base(link) {}
	
	public override bool Actionable => false;
	
	public override int Startup => ch.spotGroundedDodgeStartup;
	public override int IFrames => ch.spotGroundedDodgeLength;
	public override int Endlag => ch.spotGroundedDodgeEndlag;
	public override string ActionName => "Dodge";
	public override string StateAnimation => "SpotGroundedDodge";
	
	protected override void DoMovement()
	{
		ch.vec.x *= (1f-ch.AppropriateFriction);
		ch.vuc.x *= (1f-ch.AppropriateFriction);
		ch.vuc.y *= (1f-ch.airFriction);
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
