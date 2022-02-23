using Godot;
using System;

public class ForwardRollState : State
{
	public ForwardRollState() : base() {}
	public ForwardRollState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		if(ch.InvincibilityLeft < ch.forwardRollLength) ch.InvincibilityLeft = ch.forwardRollLength;
		ch.vec = new Vector2(ch.direction*ch.forwardRollSpeed, VCF);
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.forwardRollLength)
		{
			ch.vec = new Vector2(0, VCF);
			ch.InvincibilityLeft = 0;
			ch.ChangeState("ForwardRollEndlag");
		}
		else return false;
		
		return true;
	}
}
