using Godot;
using System;

public class BackRollState : RollState
{
	public BackRollState() : base() {}
	public BackRollState(Character link) : base(link) {}
	
	public override int Startup => ch.BackRollStartup;
	public override int IFrames => ch.BackRollLength;
	public override int Endlag => ch.BackRollEndlag;
	public override string ActionName => "BackRoll";
	public override string StateAnimation => "BackRoll";
	public override float Speed => -ch.BackRollSpeed;
}
