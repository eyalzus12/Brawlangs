using Godot;
using System;

public class LimitedAttack : Attack
{
	public string ResourceName{get; set;}
	public int AmountCanUse{get; set;}
	
	public override void Init()
	{
		ch.Resources.Give(ResourceName, AmountCanUse);
	}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<int>("AmountCanUse", 1);
		LoadExtraProperty<string>("ResourceName", "");
	}
	
	public override void OnStart()
	{
		ch.Resources.Give(ResourceName, -1);
	}
	
	public override bool CanActivate() => ch.Resources.Has(ResourceName);
}
