using Godot;
using System;

public class LimitedAttack : Attack
{
	public int amountUsed;
	public int AmountCanUse = -1;
	
	public override void LoadProperties()
	{
		LoadExtraProperty<int>("AmountCanUse", -1);
		amountUsed = 0;
	}
	
	public override void OnStart()
	{
		amountUsed++;
	}
	
	public override bool CanActivate() => (AmountCanUse <= -1)||(amountUsed < AmountCanUse);
}
