using Godot;
using System;

public class ForwardRollState : RollState
{
	public ForwardRollState() : base() {}
	public ForwardRollState(Character link) : base(link) {}
	
	public override int Startup() => ch.forwardRollStartup;
	public override int InvincibilityLength() => ch.forwardRollLength;
	public override int Endlag() => ch.forwardRollEndlag;
	//public override int Cooldown() => ch.forwardRollCooldown;
	public override string ActionName() => "ForwardRoll";
	public override string Animation() => "ForwardRoll";
	public override float Speed() => ch.forwardRollSpeed;
}
