using Godot;
using System;

public class ReplenishedLimitedAttack : LimitedAttack
{
	public bool RestoreOnGround{get; set;}
	public bool RestoreOnWall{get; set;}
	public bool RestoreOnHitting{get; set;}
	public bool RestoreOnGettingHit{get; set;}
	
	public override void LoadProperties()
	{
		base.LoadProperties();
		LoadExtraProperty<bool>("RestoreOnGround", false);
		LoadExtraProperty<bool>("RestoreOnWall", false);
		LoadExtraProperty<bool>("RestoreOnHitting", false);
		LoadExtraProperty<bool>("RestoreOnGettingHit", false);
		if(OwnerObject is Node n)
		{
			n.Connect("OptionsRestoredFromGroundTouch", this, nameof(GroundTouch));
			n.Connect("OptionsRestoredFromWallTouch", this, nameof(WallTouch));
			n.Connect("OptionsRestoredFromHitting", this, nameof(Hitting));
			n.Connect("OptionsRestoredFromGettingHit", this, nameof(GettingHit));
		}
	}
	
	public virtual void GroundTouch() {if(OwnerObject is IResourceUser ru && RestoreOnGround) ru.Resources[ResourceName] = AmountCanUse;}
	public virtual void WallTouch() {if(OwnerObject is IResourceUser ru && RestoreOnWall) ru.Resources[ResourceName] = AmountCanUse;}
	public virtual void Hitting() {if(OwnerObject is IResourceUser ru && RestoreOnHitting) ru.Resources[ResourceName] = AmountCanUse;}
	public virtual void GettingHit() {if(OwnerObject is IResourceUser ru && RestoreOnGettingHit) ru.Resources[ResourceName] = AmountCanUse;}
}
