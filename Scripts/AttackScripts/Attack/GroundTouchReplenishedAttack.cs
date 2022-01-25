using Godot;
using System;

public class GroundTouchReplenishedAttack : LimitedAttack
{
	public override void LoadProperties()
	{
		base.LoadProperties();
		ch.Connect("JumpsRestored", this, nameof(GroundTouch));
	}
	
	public virtual void GroundTouch() => amountUsed = 0;
}
