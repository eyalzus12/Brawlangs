using Godot;
using System;

public class RollState : GenericInvincibleState
{
	public RollState() : base() {}
	public RollState(Character link) : base(link) {}
	
	public virtual float Speed{get;}
	
	protected override void OnIFramesStart()
	{
		ch.vec = new Vector2(ch.Direction*Speed, VCF);
	}
	
	protected override void OnEndlagStart()
	{
		ch.vec = new Vector2(0, VCF);
	}
	
	protected override void DecideNextState()
	{
		bool turn = ch.TurnConditional();
		
		if(Inputs.IsActionJustPressed("jump")) ch.States.Change("Jump");
		else if(Inputs.IsActionPressed("down") && !ch.onSemiSolid) ch.States.Change("Duck");
		else if(turn) ch.States.Change("WalkTurn");
		else if(ch.InputtingHorizontalDirection) ch.States.Change(ch.walled?"WalkWall":"Walk");
		else ch.States.Change("Idle");
	}
}
