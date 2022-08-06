using Godot;
using System;

public class ReplenishedLimitedAttack : LimitedAttack
{
	public bool RestoreOnGround;
	public bool RestoreOnWall;
	public bool RestoreOnHitting;
	public bool RestoreOnGettingHit;
	
	public override void LoadProperties()
	{
		base.LoadProperties();
		LoadExtraProperty<bool>("RestoreOnGround", false);
		LoadExtraProperty<bool>("RestoreOnWall", false);
		LoadExtraProperty<bool>("RestoreOnHitting", false);
		LoadExtraProperty<bool>("RestoreOnGettingHit", false);
		ch.Connect("OptionsRestoredFromGroundTouch", this, nameof(GroundTouch));
		ch.Connect("OptionsRestoredFromWallTouch", this, nameof(WallTouch));
		ch.Connect("OptionsRestoredFromHitting", this, nameof(Hitting));
		ch.Connect("OptionsRestoredFromGettingHit", this, nameof(GettingHit));
	}
	
	public virtual void GroundTouch() {if(RestoreOnGround) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void WallTouch() {if(RestoreOnWall) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void Hitting() {if(RestoreOnHitting) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void GettingHit() {if(RestoreOnGettingHit) ch.Resources[ResourceName] = AmountCanUse;}
}
