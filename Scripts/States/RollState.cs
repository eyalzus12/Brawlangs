using Godot;
using System;

public class RollState : GenericInvincibleState
{
	public RollState() : base() {}
	public RollState(Character link) : base(link) {}
	
	public virtual float Speed() => 0f;
	public float speed => Speed();
	
	protected override void OnIFramesStart()
	{
		ch.vec = new Vector2(ch.direction*speed, VCF);
	}
	
	protected override void OnEndlagStart()
	{
		ch.vec = new Vector2(0, VCF);
	}
	
	protected override void DecideNextState()
	{
		bool turn = ch.TurnConditional();
		
		if(Inputs.IsActionJustPressed("player_jump"))
			ch.ChangeState("Jump");
		else if(Inputs.IsActionPressed("player_down") && !ch.onSemiSolid) 
			ch.ChangeState("Duck");
		else if(turn)
			ch.ChangeState("WalkTurn");
		else if(ch.InputtingHorizontalDirection)
			ch.ChangeState(ch.walled?"WalkWall":"Walk");
		else
			ch.ChangeState("Idle");
	}
}
