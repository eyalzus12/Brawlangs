using Godot;
using System;

public class GroundTouchReplenishedAttack : LimitedAttack
{
	public override void Init()
	{
		base.Init();
		ch.Connect("JumpsRestored", this, nameof(GroundTouch));
	}
	
	public virtual void GroundTouch() => amountUsed = 0;
}
