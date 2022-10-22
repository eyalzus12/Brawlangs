using Godot;
using System;

public class LimitedAttack : Attack
{
	public string ResourceName{get; set;}
	public int AmountCanUse{get; set;}
	
	public override void Init()
	{
		if(OwnerObject is IResourceUser ru) ru.Resources.Give(ResourceName, AmountCanUse);
	}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<int>("AmountCanUse", 1);
		LoadExtraProperty<string>("ResourceName", "");
	}
	
	public override void OnStart()
	{
		if(OwnerObject is IResourceUser ru) ru.Resources.Give(ResourceName, -1);
	}
	
	public override bool CanActivate() => OwnerObject is IResourceUser ru && ru.Resources.Has(ResourceName);
}
