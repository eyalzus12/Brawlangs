using Godot;
using System;

public class ForwardRollState : RollState
{
	public ForwardRollState() : base() {}
	public ForwardRollState(Character link) : base(link) {}
	
	public override int Startup => ch.ForwardRollStartup;
	public override int IFrames => ch.ForwardRollLength;
	public override int Endlag => ch.ForwardRollEndlag;
	public override string ActionName => "ForwardRoll";
	public override string StateAnimation => "ForwardRoll";
	public override float Speed => ch.ForwardRollSpeed;
}
