using Godot;
using System;

public class ForwardRollState : RollState
{
	public ForwardRollState() : base() {}
	public ForwardRollState(Character link) : base(link) {}
	
	public override int Startup => ch.forwardRollStartup;
	public override int IFrames => ch.forwardRollLength;
	public override int Endlag => ch.forwardRollEndlag;
	public override string ActionName => "ForwardRoll";
	public override string StateAnimation => "ForwardRoll";
	public override float Speed => ch.forwardRollSpeed;
}
