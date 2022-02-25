using Godot;
using System;

public class DirectionalAirDodgeState : GenericInvincibleState
{
	public DirectionalAirDodgeState() : base() {}
	public DirectionalAirDodgeState(Character link) : base(link) {}
	
	public Vector2 movement;
	
	public override bool IsActionable() => false;
	
	public override int Startup() => ch.directionalAirDodgeStartup;
	public override int InvincibilityLength() => ch.directionalAirDodgeLength;
	public override int Endlag() => ch.directionalAirDodgeEndlag;
	public override int Cooldown() => ch.directionalAirDodgeCooldown;
	public override string ActionName() => "Dodge";
	public override string Animation() => "DirectionAirDodge";
	
	public override void Init()
	{
		base.Init();
		movement = ch.GetInputVector()*ch.directionalAirDodgeSpeed;
	}
	
	protected override void OnIFramesStart()
	{
		ch.vec = movement;
	}
	
	protected override void OnEndlagStart()
	{
		//ch.vec = new Vector2(0, VCF);
	}
	
	protected override void DecideNextState()
	{
		bool turn = ch.TurnConditional();
			
		if(ch.grounded)
		{
			if(Inputs.IsActionJustPressed("player_jump"))
				ch.ChangeState("Jump");
			else if(Inputs.IsActionPressed("player_down") && !ch.onSemiSolid) 
				ch.ChangeState(ch.InputtingHorizontalDirection()?ch.walled?"CrawlWall":"Crawl":"Crouch");
			else if(ch.IsIdle())
				ch.ChangeState("Idle");
			else if(turn)
				ch.ChangeState("WalkTurn");
			else if(ch.InputtingHorizontalDirection())
				ch.ChangeState(ch.walled?"WalkWall":"Walk");
			else
				ch.ChangeState("WalkStop");
		}
		else ch.ChangeState("Air");
	}
}
