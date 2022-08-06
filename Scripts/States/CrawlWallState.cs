using System;
using Godot;

public class CrawlWallState: BaseCrouchState
{
	public CrawlWallState(): base() {}
	public CrawlWallState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.PlayAnimation("Crawl");
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(!ch.walled)
		{
			if(ch.TurnConditional()) ch.States.Change("Crawl");
			else ch.States.Change("Crouch");
		}
		else return false;
		
		return false;
	}
}
