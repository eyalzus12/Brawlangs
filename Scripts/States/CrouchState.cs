using Godot;
using System;

public class CrouchState : BaseCrouchState
{
	public CrouchState(): base() {}
	public CrouchState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.vec.x = 0;
		ch.PlayAnimation("Crouch");
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.InputtingHorizontalDirection) ch.States.Change("Crawl");
		else return false;
		
		return true;
	}
}
