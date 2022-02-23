using Godot;
using System;

public class BackwardsRollState : State
{
	public BackwardsRollState() : base() {}
	public BackwardsRollState(Character link) : base(link) {}
	
	public override bool IsActionable() => false;
	
	public override void Init()
	{
		if(ch.InvincibilityLeft < ch.backwardsRollLength) ch.InvincibilityLeft = ch.backwardsRollLength;
		ch.vec = new Vector2(-ch.direction*ch.backwardsRollSpeed, VCF);
	}
	
	protected override bool CalcStateChange()
	{
		if(frameCount >= ch.backwardsRollLength)
		{
			ch.vec = new Vector2(0, VCF);
			ch.InvincibilityLeft = 0;
			ch.ChangeState("BackwardsRollEndlag");
		}
		else return false;
		
		return true;
	}
}
