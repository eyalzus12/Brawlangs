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
	
	public virtual void GroundTouch() => ch.SetResource(ResourceName, AmountCanUse);
	public virtual void WallTouch() => ch.SetResource(ResourceName, AmountCanUse);
	public virtual void Hitting() => ch.SetResource(ResourceName, AmountCanUse);
	public virtual void GettingHit() => ch.SetResource(ResourceName, AmountCanUse);
}
