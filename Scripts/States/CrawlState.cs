using Godot;
using System;

public class CrawlState : BaseCrouchState
{
	public CrawlState(): base() {}
	public CrawlState(Character link): base(link) {}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.walled)
			ch.ChangeState("CrawlWall");
		else if(!ch.InputingDirection())
			ch.ChangeState("Crouch");
		else return false;
		
		return true;
	}
}
