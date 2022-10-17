using Godot;
using System;

public class ChargePart : AttackPart
{
	public string ChargeInput{get; set;}
	public float FullChargeDamageMult{get; set;}
	public float FullChargeKnockbackMult{get; set;}
	public int MinimumChargeForBoost{get; set;}
	public string ChargeSound{get; set;}
	
	public override void LoadProperties()
	{
		LoadExtraProperty<string>("ChargeInput", "heavy");
		LoadExtraProperty<float>("FullChargeDamageMult", 1f);
		LoadExtraProperty<float>("FullChargeKnockbackMult", 1f);
		LoadExtraProperty<string>("ChargeSound", "");
	}
	
	public override void OnStart()
	{
		ch.PlaySound(ChargeSound, Position);
	}
	
	public override void Loop()
	{
		if(!ch.Inputs.IsActionPressed(ChargeInput)) ChangeToNext();
	}
	
	public override void OnEnd()
	{
		if(FrameCount < MinimumChargeForBoost) return;
		OwnerAttack.DamageDoneMult *= CalculateDamageMult();
		OwnerAttack.KnockbackDoneMult *= CalculateKnockbackMult();
	}
	
	public virtual float ChargeFraction()
	{
		var timeHeld = Math.Max(0, FrameCount-Startup);
		return (float)timeHeld/(float)Length;
	}
	
	public virtual float CalculateDamageMult() => ChargeFraction()*(FullChargeDamageMult-1f) + 1f;
	public virtual float CalculateKnockbackMult() => ChargeFraction()*(FullChargeKnockbackMult-1f) + 1f;
}
