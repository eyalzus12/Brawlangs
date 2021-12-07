using Godot;
using System;

public class LimitedAttack : Attack
{
	public int amountUsed;
	public int AmountCanUse = 1;
	
	public override void Init()
	{
		LoadExtraProperty<int>("AmountCanUse");
		amountUsed = 0;
	}
	
	public override void OnStart()
	{
		amountUsed++;
	}
	
	public override bool CanActivate() => (amountUsed < AmountCanUse);
}
