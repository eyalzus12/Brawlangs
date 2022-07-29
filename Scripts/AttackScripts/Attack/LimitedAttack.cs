using Godot;
using System;

public class LimitedAttack : Attack
{
	public string ResourceName;
	public int AmountCanUse;
	
	public override void Init()
	{
		ch.GiveResource(ResourceName, AmountCanUse);
	}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<int>("AmountCanUse", 1);
		LoadExtraProperty<string>("ResourceName", "");
	}
	
	public override void OnStart()
	{
		ch.GiveResource(ResourceName, -1);
	}
	
	public override bool CanActivate() => ch.HasResource(ResourceName);
}
