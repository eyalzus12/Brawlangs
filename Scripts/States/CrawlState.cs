using Godot;
using System;

public class CrawlState : BaseCrouchState
{
	public CrawlState(): base() {}
	public CrawlState(Character link): base(link) {}
	
	public override void Init()
	{
		ch.QueueAnimation("Crawl", ch.AnimationLooping, true);
	}
	
	protected override bool CalcStateChange()
	{
		if(base.CalcStateChange()) return true;
		else if(ch.walled)
			ch.States.Change("CrawlWall");
		else if(!ch.InputtingHorizontalDirection)
			ch.States.Change("Crouch");
		else return false;
		
		return true;
	}
}
