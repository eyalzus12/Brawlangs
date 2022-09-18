using Godot;
using System;

public partial class ReplenishedLimitedAttack : LimitedAttack
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
		ch.Connect("OptionsRestoredFromGroundTouch",new Callable(this,nameof(GroundTouch)));
		ch.Connect("OptionsRestoredFromWallTouch",new Callable(this,nameof(WallTouch)));
		ch.Connect("OptionsRestoredFromHitting",new Callable(this,nameof(Hitting)));
		ch.Connect("OptionsRestoredFromGettingHit",new Callable(this,nameof(GettingHit)));
	}
	
	public virtual void GroundTouch() {if(RestoreOnGround) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void WallTouch() {if(RestoreOnWall) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void Hitting() {if(RestoreOnHitting) ch.Resources[ResourceName] = AmountCanUse;}
	public virtual void GettingHit() {if(RestoreOnGettingHit) ch.Resources[ResourceName] = AmountCanUse;}
}
