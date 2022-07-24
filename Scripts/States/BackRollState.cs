using Godot;
using System;

public class BackRollState : RollState
{
	public BackRollState() : base() {}
	public BackRollState(Character link) : base(link) {}
	
	public override int Startup() => ch.backRollStartup;
	public override int InvincibilityLength() => ch.backRollLength;
	public override int Endlag() => ch.backRollEndlag;
	//public override int Cooldown() => ch.backRollCooldown;
	public override string ActionName() => "BackRoll";
	public override string Animation() => "BackRoll";
	public override float Speed() => -ch.backRollSpeed;
}
