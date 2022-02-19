using Godot;
using System;

public class ReplenishedLimitedAttack : LimitedAttack
{
	public override void LoadProperties()
	{
		base.LoadProperties();
		ch.Connect("OptionsRestoredFromGroundTouch", this, nameof(GroundTouch));
		ch.Connect("OptionsRestoredFromWallTouch", this, nameof(WallTouch));
		ch.Connect("OptionsRestoredFromHitting", this, nameof(Hitting));
		ch.Connect("OptionsRestoredFromGettingHit", this, nameof(GettingHit));
	}
	
	public virtual void GroundTouch() => amountUsed = 0;
	public virtual void WallTouch() => amountUsed = 0;
	public virtual void Hitting() => amountUsed = 0;
	public virtual void GettingHit() => amountUsed = 0;
}
