using Godot;
using System;

public class ForwardRollEndlagState : State
{
	public ForwardRollEndlagState() : base() {}
	public ForwardRollEndlagState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.forwardRollEndlag)
		{
			bool turn = ch.TurnConditional();
			
			if(Inputs.IsActionJustPressed("player_jump"))
				ch.ChangeState("Jump");
			else if(Inputs.IsActionPressed("player_down") && !ch.onSemiSolid) 
				ch.ChangeState("Duck");
			else if(turn)
				ch.ChangeState("WalkTurn");
			else if(ch.InputingDirection())
				ch.ChangeState(ch.walled?"WalkWall":"Walk");
			else
				ch.ChangeState("Idle");
		}
		else return false;
		
		return true;
	}
}
